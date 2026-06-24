using DBFileReaderLib;
using SpellWork.Database;
using SpellWork.DBC.Structures;
using SpellWork.DBC.Structures.V7;
using SpellWork.DBC.Structures.V8;
using SpellWork.Extensions;
using SpellWork.GameTables;
using SpellWork.GameTables.Structures;
using SpellWork.Properties;
using SpellWork.Spell;
using SpellWork.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SpellWork.DBC
{
    public static class DBC
    {
        public const string Version10x = "SpellWork 10.2.5 (52902)";
        public const string Version8x  = "SpellWork 8.3.0 (34220)";
        public const string Version7x  = "SpellWork 7.2.5 (24330)";

        public static string Version => Settings.Default.GameVersion switch
        {
            "7.x"  => Version7x,
            "8.x"  => Version8x,
            _      => Version10x
        };

        public static uint MaxLevel => Settings.Default.GameVersion switch
        {
            "7.x"  => 110u,
            "8.x"  => 120u,
            _      => 70u
        };

        public const uint MaxItemLevel = 1300;

        public static Storage<AreaGroupMemberEntry>             AreaGroupMember { get; set; }
        public static Storage<AreaTableEntry>                   AreaTable { get; set; }
        public static Storage<ContentTuningEntry>               ContentTuning { get; set; }
        public static Storage<ContentTuningXExpectedEntry>      ContentTuningXExpected { get; set; }
        public static Storage<CraftingDataEntry>                CraftingData { get; set; }
        public static Storage<DifficultyEntry>                  Difficulty { get; set; }
        public static Storage<ExpectedStatEntry>                ExpectedStat { get; set; }
        public static Storage<ExpectedStatModEntry>             ExpectedStatMod { get; set; }
        public static Storage<MapEntry>                         Map { get; set; }
        public static Storage<MapDifficultyEntry>               MapDifficulty { get; set; }
        public static Storage<OverrideSpellDataEntry>           OverrideSpellData { get; set; }
        public static Storage<ScreenEffectEntry>                ScreenEffect { get; set; }
        public static Storage<SpellCastTimesEntry>              SpellCastTimes { get; set; }
        public static Storage<SpellCategoryEntry>               SpellCategory { get; set; }
        public static Storage<SpellDurationEntry>               SpellDuration { get; set; }
        public static Storage<SpellRadiusEntry>                 SpellRadius { get; set; }
        public static Storage<SpellRangeEntry>                  SpellRange { get; set; }
        public static Storage<RandPropPointsEntry>              RandPropPoints { get; set; }

        public static Storage<SkillLineAbilityEntry>            SkillLineAbility { get; set; }
        public static Storage<SkillLineEntry>                   SkillLine { get; set; }

        public static readonly IDictionary<int, SpellInfo> SpellInfoStore = new ConcurrentDictionary<int, SpellInfo>();
        public static readonly IDictionary<int, ISet<int>> SpellTriggerStore = new Dictionary<int, ISet<int>>();

        private enum Progress
        {
            Hotfix = 0,
            DB2 = 5,
            Stores = 25,
            MySQLSpells = 90,
            GtScaling = 95,
            Completed = 100
        }

        public static async Task Load(Action<int> progressCallback)
        {
            if (Settings.Default.GameVersion == "7.x")
            {
                await LoadV7(progressCallback);
                return;
            }

            if (Settings.Default.GameVersion == "8.x")
            {
                await LoadV8(progressCallback);
                return;
            }

            var progressHandler = new ProgressHandler(progressCallback);

            HotfixReader hotfixReader = null;
            try
            {
                hotfixReader = new HotfixReader(Settings.Default.HotfixCachePath);
            }
            catch (Exception)
            {
                Console.WriteLine(
                    $"Hotfix cache {Settings.Default.HotfixCachePath} cannot be loaded, ignoring!");
            }

            progressHandler.SetProgress((int)Progress.DB2);

            var dbcProperties = typeof(DBC).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var dbcPropertiesFiltered = dbcProperties.Where(dbc =>
                dbc.PropertyType.IsGenericType &&
                dbc.PropertyType.GetGenericTypeDefinition() == typeof(Storage<>));
            progressHandler.StartStepsProgress(dbcPropertiesFiltered.Count(), (int)Progress.Stores);

            Parallel.ForEach(dbcPropertiesFiltered, dbc =>
               {
                   var name = dbc.Name;

                   try
                   {
                       dbc.SetValue(dbc.GetValue(null), CreateInstance(dbc.PropertyType, name, hotfixReader));
                   }
                   catch (DirectoryNotFoundException)
                   {
                   }
                   catch (TargetInvocationException tie)
                   {
                       var realEx = tie.InnerException ?? tie;
                       Console.WriteLine($"[Load10x] Could not load {name}.db2: {realEx.Message}");
                   }
                   finally
                   {
                       progressHandler.IncrementStepsProgress();
                   }
               });

            progressHandler.SetProgress((int)Progress.Stores);

            {
                var spells = CreateInstance<Storage<SpellEntry>>("Spell", hotfixReader);
                var spellNames = CreateInstance<Storage<SpellNameEntry>>("SpellName", hotfixReader);
                foreach (var spell in spellNames)
                    SpellInfoStore[(int) spell.Value.ID] = new SpellInfo(spell.Value.Name, spells.GetValue((int) spell.Value.ID));
            }

            List<Action> storeProcessingActions = new List<Action>
            {
                () =>
                {
                    var spellMiscs = CreateInstance<Storage<SpellMiscEntry>>("SpellMisc", hotfixReader);
                    foreach (var spellMisc in spellMiscs.Values)
                    {
                        if (spellMisc.DifficultyID != 0)
                            continue;

                        if (!SpellInfoStore.TryGetValue(spellMisc.SpellID, out var spell))
                            continue;

                        spell.Misc = spellMisc;

                        if (SpellDuration.TryGetValue(spellMisc.DurationIndex, out var durationEntry))
                            spell.DurationEntry = durationEntry;

                        if (SpellRange.TryGetValue(spellMisc.RangeIndex, out var rangeEntry))
                            spell.Range = rangeEntry;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellEffects = CreateInstance<Storage<SpellEffectEntry>>("SpellEffect", hotfixReader);
                    foreach (var effect in spellEffects.Values)
                    {
                        if (!SpellInfoStore.TryGetValue(effect.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"Spell effect {effect.ID} is referencing unknown spell {effect.SpellID}, ignoring!");
                            continue;
                        }

                        spellInfo.SpellEffectInfoStore.Add(new SpellEffectInfo(effect)); // Helper

                        var triggerId = effect.EffectTriggerSpell;
                        if (triggerId == 0)
                            continue;

                        if (SpellTriggerStore.TryGetValue(triggerId, out var trigger))
                            trigger.Add(effect.SpellID);
                        else
                            SpellTriggerStore.Add(triggerId, new SortedSet<int> { effect.SpellID });
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellTargetRestrictions = CreateInstance<Storage<SpellTargetRestrictionsEntry>>("SpellTargetRestrictions", hotfixReader);
                    foreach (var spellTargetRestriction in spellTargetRestrictions.Values)
                    {
                        if (spellTargetRestriction.DifficultyID != 0)
                            continue;

                        if (!SpellInfoStore.TryGetValue(spellTargetRestriction.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellTargetRestrictions: Unknown spell {spellTargetRestriction.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.TargetRestrictions = spellTargetRestriction;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellXSpellVisuals = CreateInstance<Storage<SpellXSpellVisualEntry>>("SpellXSpellVisual", hotfixReader);
                    foreach (var spellXSpellVisual in spellXSpellVisuals.Values.Where(effect => effect.CasterPlayerConditionID == 0))
                    {
                        if (spellXSpellVisual.DifficultyID != 0)
                            continue;

                        if (!SpellInfoStore.TryGetValue(spellXSpellVisual.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellXSpellVisual: Unknown spell {spellXSpellVisual.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.SpellXSpellVisual = spellXSpellVisual;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellScalings = CreateInstance<Storage<SpellScalingEntry>>("SpellScaling", hotfixReader);
                    foreach (var spellScaling in spellScalings.Values)
                    {
                        if (!SpellInfoStore.TryGetValue(spellScaling.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellScaling: Unknown spell {spellScaling.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.Scaling = spellScaling;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellAuraOptions = CreateInstance<Storage<SpellAuraOptionsEntry>>("SpellAuraOptions", hotfixReader);
                    var spellProcsPerMinutes = CreateInstance<Storage<SpellProcsPerMinuteEntry>>("SpellProcsPerMinute", hotfixReader);
                    foreach (var auraOptions in spellAuraOptions.Values)
                    {
                        if (auraOptions.DifficultyID != 0)
                            continue;

                        if (!SpellInfoStore.TryGetValue(auraOptions.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellAuraOptions: Unknown spell {auraOptions.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.AuraOptions = auraOptions;
                        if (auraOptions.SpellProcsPerMinuteID != 0)
                            SpellInfoStore[auraOptions.SpellID].ProcsPerMinute = spellProcsPerMinutes[auraOptions.SpellProcsPerMinuteID];
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellAuraRestrictions = CreateInstance<Storage<SpellAuraRestrictionsEntry>>("SpellAuraRestrictions", hotfixReader);
                    foreach (var auraRestrictions in spellAuraRestrictions.Values)
                    {
                        if (auraRestrictions.DifficultyID != 0)
                            continue;

                        if (!SpellInfoStore.TryGetValue(auraRestrictions.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellAuraRestrictions: Unknown spell {auraRestrictions.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.AuraRestrictions = auraRestrictions;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellCategories = CreateInstance<Storage<SpellCategoriesEntry>>("SpellCategories", hotfixReader);
                    foreach (var categories in spellCategories.Values)
                    {
                        if (categories.DifficultyID != 0)
                            continue;

                        if (!SpellInfoStore.TryGetValue(categories.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellCategories: Unknown spell {categories.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.Categories = categories;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellCastingRequirements = CreateInstance<Storage<SpellCastingRequirementsEntry>>("SpellCastingRequirements", hotfixReader);
                    foreach (var castingRequirements in spellCastingRequirements.Values)
                    {
                        if (!SpellInfoStore.TryGetValue(castingRequirements.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellCastingRequirements: Unknown spell {castingRequirements.SpellID} referenced, ignoring!");
                            return;
                        }

                        spellInfo.CastingRequirements = castingRequirements;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellClassOptions = CreateInstance<Storage<SpellClassOptionsEntry>>("SpellClassOptions", hotfixReader);
                    foreach (var classOptions in spellClassOptions.Values)
                    {
                        if (!SpellInfoStore.TryGetValue(classOptions.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellClassOptions: Unknown spell {classOptions.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.ClassOptions = classOptions;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellCooldowns = CreateInstance<Storage<SpellCooldownsEntry>>("SpellCooldowns", hotfixReader);
                    foreach (var cooldowns in spellCooldowns.Values)
                    {
                        if (cooldowns.DifficultyID != 0)
                            continue;

                        if (!SpellInfoStore.TryGetValue(cooldowns.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellCooldowns: Unknown spell {cooldowns.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.Cooldowns = cooldowns;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellInterrupts = CreateInstance<Storage<SpellInterruptsEntry>>("SpellInterrupts", hotfixReader);
                    foreach (var interrupt in spellInterrupts)
                    {
                        if (interrupt.Value.DifficultyID != 0)
                            continue;

                        if (!SpellInfoStore.TryGetValue(interrupt.Value.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellInterrupts: Unknown spell {interrupt.Value.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.Interrupts = interrupt.Value;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellEquippedItems = CreateInstance<Storage<SpellEquippedItemsEntry>>("SpellEquippedItems", hotfixReader);
                    foreach (var equippedItems in spellEquippedItems.Values)
                    {
                        if (!SpellInfoStore.TryGetValue(equippedItems.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellEquippedItems: Unknown spell {equippedItems.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.EquippedItems = equippedItems;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellLabels = CreateInstance<Storage<SpellLabelEntry>>("SpellLabel", hotfixReader);
                    foreach (var label in spellLabels.Values)
                    {
                        if (!SpellInfoStore.TryGetValue(label.SpellID, out var spellInfo))
                        {
                            Console.WriteLine($"SpellLabel: Unknown spell {label.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.Labels.Add(label.LabelID);
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellLevels = CreateInstance<Storage<SpellLevelsEntry>>("SpellLevels", hotfixReader);
                    foreach (var levels in spellLevels.Values)
                    {
                        if (levels.DifficultyID != 0)
                            continue;

                        if (!SpellInfoStore.TryGetValue(levels.SpellID, out var spellInfo))
                        {
                            Console.WriteLine($"SpellLevels: Unknown spell {levels.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.Levels = levels;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellPowers = CreateInstance<Storage<SpellPowerEntry>>("SpellPower", hotfixReader);
                    foreach (var power in spellPowers.Values)
                    {
                        if (!SpellInfoStore.TryGetValue(power.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellPower: Unknown spell {power.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.Powers.Add(power);
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellReagents = CreateInstance<Storage<SpellReagentsEntry>>("SpellReagents", hotfixReader);
                    foreach (var reagents in spellReagents.Values)
                    {
                        if (!SpellInfoStore.TryGetValue(reagents.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellReagents: Unknown spell {reagents.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.Reagents = reagents;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellReagentsCurrencies = CreateInstance<Storage<SpellReagentsCurrencyEntry>>("SpellReagentsCurrency", hotfixReader);
                    foreach (var spellReagentsCurrency in spellReagentsCurrencies.Values)
                    {
                        if (!SpellInfoStore.TryGetValue(spellReagentsCurrency.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellReagentsCurrency: Unknown spell {spellReagentsCurrency.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.ReagentsCurrency.Add(spellReagentsCurrency);
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellShapeshifts = CreateInstance<Storage<SpellShapeshiftEntry>>("SpellShapeshift", hotfixReader);
                    foreach (var spellShapeshift in spellShapeshifts.Values)
                    {
                        if (!SpellInfoStore.TryGetValue(spellShapeshift.SpellID, out var spellInfo))
                        {
                            Console.WriteLine(
                                $"SpellShapeshift: Unknown spell {spellShapeshift.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.Shapeshift = spellShapeshift;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellTotems = CreateInstance<Storage<SpellTotemsEntry>>("SpellTotems", hotfixReader);
                    foreach (var spellTotem in spellTotems.Values)
                    {
                        if (!SpellInfoStore.TryGetValue(spellTotem.SpellID, out var spellInfo))
                        {
                            Console.WriteLine($"SpellTotems: Unknown spell {spellTotem.SpellID} referenced, ignoring!");
                            continue;
                        }

                        spellInfo.Totems = spellTotem;
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var spellXDescriptionVariables = CreateInstance<Storage<SpellXDescriptionVariablesEntry>>("SpellXDescriptionVariables", hotfixReader);
                    var spellDescriptionVariables = CreateInstance<Storage<SpellDescriptionVariablesEntry>>("SpellDescriptionVariables", hotfixReader);
                    foreach (var descriptionVariable in spellXDescriptionVariables.Values)
                    {
                        if (!SpellInfoStore.TryGetValue(descriptionVariable.SpellID, out var spellInfo))
                        {
                            Console.WriteLine($"SpellXDescriptionVariables: Unknown spell {descriptionVariable.SpellID} referenced, ignoring!");
                            continue;
                        }
                        spellInfo.DescriptionVariables = spellDescriptionVariables.GetValue(descriptionVariable.SpellDescriptionVariablesID);
                    }
                    progressHandler.IncrementStepsProgress();
                },
                () =>
                {
                    var itemEffects = CreateInstance<Storage<ItemEffectEntry>>("ItemEffect", hotfixReader);
                    var itemSparses = CreateInstance<Storage<ItemSparseEntry>>("ItemSparse", hotfixReader);
                    var itemXItemEffects = CreateInstance<Storage<ItemXItemEffectEntry>>("ItemXItemEffect", hotfixReader);

                    foreach (var itemXItemEffect in itemXItemEffects.Values)
                    {
                        if (!itemEffects.TryGetValue(itemXItemEffect.ItemEffectID, out var itemEffect))
                            continue;

                        if (!SpellInfoStore.ContainsKey(itemEffect.SpellID))
                            continue;

                        itemEffect.ItemID = itemXItemEffect.ItemID;
                        if (itemSparses.TryGetValue(itemXItemEffect.ItemID, out var item))
                            itemEffect.Item = item;

                        SpellInfoStore[itemEffect.SpellID].ItemEffects.Add(itemEffect);
                    }
                    progressHandler.IncrementStepsProgress();
                }
            };

            progressHandler.StartStepsProgress(storeProcessingActions.Count, (int)Progress.MySQLSpells);

            await Task.WhenAll(storeProcessingActions.Select(Task.Run));

            progressHandler.SetProgress((int)Progress.MySQLSpells);

            MySqlConnection.LoadServersideSpells();

            progressHandler.SetProgress((int)Progress.GtScaling);

            GameTable<GtSpellScalingEntry>.Open($@"{Settings.Default.GtPath}\SpellScaling.txt");

            progressHandler.SetProgress((int)Progress.Completed);
        }

        private static dynamic CreateInstance(Type type, string name, HotfixReader hotfixReader)
        {
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Storage<>))
                return default;

            var db2Reader = new DBReader($@"{Settings.Default.DbcPath}\{Settings.Default.Locale}\{name}.db2");

            dynamic storage = Activator.CreateInstance(type, db2Reader);

            if (hotfixReader != null)
                hotfixReader.ApplyHotfixes(storage, db2Reader);

            return storage;
        }

        private static T CreateInstance<T>(string name, HotfixReader hotfixReader)
        {
            return CreateInstance(typeof(T), name, hotfixReader);
        }

        private static async Task LoadV8(Action<int> progressCallback)
        {
            var progressHandler = new ProgressHandler(progressCallback);

            HotfixReader hotfixReader = null;
            try
            {
                hotfixReader = new HotfixReader(Settings.Default.HotfixCachePath);
            }
            catch (Exception)
            {
                Console.WriteLine($"Hotfix cache {Settings.Default.HotfixCachePath} cannot be loaded, ignoring!");
            }

            progressHandler.SetProgress(5);

            // --- Shared tables with identical layout ---
            AreaGroupMember   = LoadV8DB2<AreaGroupMemberEntry>("AreaGroupMember", hotfixReader);
            AreaTable         = LoadV8DB2<AreaTableEntry>("AreaTable", hotfixReader);
            ContentTuning     = LoadV8DB2<ContentTuningEntry>("ContentTuning", hotfixReader);
            ContentTuningXExpected = LoadV8DB2<ContentTuningXExpectedEntry>("ContentTuningXExpected", hotfixReader);
            Difficulty        = LoadV8DB2<DifficultyEntry>("Difficulty", hotfixReader);
            ExpectedStat      = LoadV8DB2<ExpectedStatEntry>("ExpectedStat", hotfixReader);
            ExpectedStatMod   = LoadV8DB2<ExpectedStatModEntry>("ExpectedStatMod", hotfixReader);
            Map               = LoadV8DB2<MapEntry>("Map", hotfixReader);
            MapDifficulty     = LoadV8DB2<MapDifficultyEntry>("MapDifficulty", hotfixReader);
            OverrideSpellData = LoadV8DB2<OverrideSpellDataEntry>("OverrideSpellData", hotfixReader);
            ScreenEffect      = LoadV8DB2<ScreenEffectEntry>("ScreenEffect", hotfixReader);
            SpellCastTimes    = LoadV8DB2<SpellCastTimesEntry>("SpellCastTimes", hotfixReader);
            SpellCategory     = LoadV8DB2<SpellCategoryEntry>("SpellCategory", hotfixReader);
            SpellDuration     = LoadV8DB2<SpellDurationEntry>("SpellDuration", hotfixReader);
            SpellRadius       = LoadV8DB2<SpellRadiusEntry>("SpellRadius", hotfixReader);
            SpellRange        = LoadV8DB2<SpellRangeEntry>("SpellRange", hotfixReader);
            RandPropPoints    = LoadV8DB2<RandPropPointsEntry>("RandPropPoints", hotfixReader);
            SkillLineAbility  = LoadV8DB2<SkillLineAbilityEntry>("SkillLineAbility", hotfixReader);
            SkillLine         = LoadV8DB2<SkillLineEntry>("SkillLine", hotfixReader);
            var spellAuraOptionsV8      = LoadV8DB2<SpellAuraOptionsEntry>("SpellAuraOptions", hotfixReader);
            var spellCastingReqsV8      = LoadV8DB2<SpellCastingRequirementsEntry>("SpellCastingRequirements", hotfixReader);
            var spellCategoriesV8       = LoadV8DB2<SpellCategoriesEntry>("SpellCategories", hotfixReader);
            var spellClassOptionsV8     = LoadV8DB2<SpellClassOptionsEntry>("SpellClassOptions", hotfixReader);
            var spellDescVarsV8         = LoadV8DB2<SpellDescriptionVariablesEntry>("SpellDescriptionVariables", hotfixReader);
            var spellEquippedItemsV8    = LoadV8DB2<SpellEquippedItemsEntry>("SpellEquippedItems", hotfixReader);
            var spellInterruptsV8       = LoadV8DB2<SpellInterruptsEntry>("SpellInterrupts", hotfixReader);
            var spellPowerV8            = LoadV8DB2<SpellPowerEntry>("SpellPower", hotfixReader);
            var spellProcsPerMinV8      = LoadV8DB2<SpellProcsPerMinuteEntry>("SpellProcsPerMinute", hotfixReader);
            var spellReagentsV8         = LoadV8DB2<SpellReagentsEntry>("SpellReagents", hotfixReader);
            var spellReagentsCurrV8     = LoadV8DB2<SpellReagentsCurrencyEntry>("SpellReagentsCurrency", hotfixReader);
            var spellShapeshiftV8       = LoadV8DB2<SpellShapeshiftEntry>("SpellShapeshift", hotfixReader);
            var spellTargetRestV8       = LoadV8DB2<SpellTargetRestrictionsEntry>("SpellTargetRestrictions", hotfixReader);
            var spellTotemsV8           = LoadV8DB2<SpellTotemsEntry>("SpellTotems", hotfixReader);
            var spellXSpellVisualV8     = LoadV8DB2<SpellXSpellVisualEntry>("SpellXSpellVisual", hotfixReader);
            var spellXDescVarsV8        = LoadV8DB2<SpellXDescriptionVariablesEntry>("SpellXDescriptionVariables", hotfixReader);
            var spellLabelV8            = LoadV8DB2<SpellLabelEntry>("SpellLabel", hotfixReader);
            var itemEffectV8            = LoadV8DB2<ItemEffectEntryV8>("ItemEffect", hotfixReader);
            var itemSparseV8            = LoadV8DB2<ItemSparseEntry>("ItemSparse", hotfixReader);

            progressHandler.SetProgress(20);

            // --- V8-specific structs ---
            var spellNameV8       = LoadV8DB2<SpellNameEntry>("SpellName", hotfixReader);
            var spellEntryV8      = LoadV8DB2<SpellEntry>("Spell", hotfixReader);
            var spellMiscsV8      = LoadV8DB2<SpellMiscEntryV8>("SpellMisc", hotfixReader);
            var spellEffectsV8    = LoadV8DB2<SpellEffectEntryV8>("SpellEffect", hotfixReader);
            var spellLevelsV8     = LoadV8DB2<SpellLevelsEntryV8>("SpellLevels", hotfixReader);
            var spellCooldownsV8  = LoadV8DB2<SpellCooldownsEntryV8>("SpellCooldowns", hotfixReader);
            var spellScalingV8    = LoadV8DB2<SpellScalingEntryV8>("SpellScaling", hotfixReader);
            var spellAuraRestV8   = LoadV8DB2<SpellAuraRestrictionsEntryV8>("SpellAuraRestrictions", hotfixReader);

            progressHandler.SetProgress(30);

            // --- Build SpellInfoStore from SpellName (8.x uses separate SpellName table) ---
            if (spellNameV8 == null)
                throw new DirectoryNotFoundException("SpellName.db2 could not be loaded.");

            foreach (var kv in spellNameV8)
            {
                var entry = spellEntryV8?.GetValue((int)kv.Value.ID);
                SpellInfoStore[(int)kv.Value.ID] = new SpellInfo(kv.Value.Name ?? string.Empty, entry);
            }

            progressHandler.SetProgress(40);

            // --- Wire V8 SpellMisc → canonical SpellMiscEntry ---
            foreach (var kv in spellMiscsV8 ?? Enumerable.Empty<KeyValuePair<int, SpellMiscEntryV8>>())
            {
                var m = kv.Value;
                if (m.DifficultyID != 0) continue;
                if (!SpellInfoStore.TryGetValue(m.SpellID, out var spell)) continue;

                var misc = new SpellMiscEntry
                {
                    ID                   = m.ID,
                    DifficultyID         = m.DifficultyID,
                    SpellID              = m.SpellID,
                    CastingTimeIndex     = m.CastingTimeIndex,
                    DurationIndex        = m.DurationIndex,
                    RangeIndex           = m.RangeIndex,
                    SchoolMask           = m.SchoolMask,
                    Speed                = m.Speed,
                    LaunchDelay          = m.LaunchDelay,
                    MinDuration          = m.MinDuration,
                    SpellIconFileDataID  = m.SpellIconFileDataID,
                    ActiveIconFileDataID = m.ActiveIconFileDataID,
                    ContentTuningID      = m.ContentTuningID,
                    Attributes           = new int[15]
                };
                for (int i = 0; i < 14; i++)
                    misc.Attributes[i] = m.Attributes[i];

                spell.Misc = misc;

                if (SpellDuration != null && SpellDuration.TryGetValue(m.DurationIndex, out var dur))
                    spell.DurationEntry = dur;
                if (SpellRange != null && SpellRange.TryGetValue(m.RangeIndex, out var rng))
                    spell.Range = rng;
            }

            progressHandler.SetProgress(50);

            // --- Wire V8 SpellEffect → canonical SpellEffectEntry ---
            foreach (var kv in spellEffectsV8 ?? Enumerable.Empty<KeyValuePair<int, SpellEffectEntryV8>>())
            {
                var e8 = kv.Value;
                if (!SpellInfoStore.TryGetValue(e8.SpellID, out var spellInfo)) continue;

                var eff = new SpellEffectEntry
                {
                    ID                         = e8.ID,
                    SpellID                    = e8.SpellID,
                    Effect                     = (int)e8.Effect,
                    EffectAura                 = e8.EffectAura,
                    DifficultyID               = e8.DifficultyID,
                    EffectIndex                = e8.EffectIndex,
                    EffectAmplitude            = e8.EffectAmplitude,
                    EffectAttributes           = e8.EffectAttributes,
                    EffectAuraPeriod           = e8.EffectAuraPeriod,
                    EffectBonusCoefficient     = e8.EffectBonusCoefficient,
                    EffectChainAmplitude       = e8.EffectChainAmplitude,
                    EffectChainTargets         = e8.EffectChainTargets,
                    EffectItemType             = e8.EffectItemType,
                    EffectMechanic             = e8.EffectMechanic,
                    EffectPointsPerResource    = e8.EffectPointsPerResource,
                    EffectPosFacing            = e8.EffectPosFacing,
                    EffectRealPointsPerLevel   = e8.EffectRealPointsPerLevel,
                    EffectTriggerSpell         = e8.EffectTriggerSpell,
                    BonusCoefficientFromAP     = e8.BonusCoefficientFromAP,
                    PvpMultiplier              = e8.PvpMultiplier,
                    Coefficient                = e8.Coefficient,
                    Variance                   = e8.Variance,
                    ResourceCoefficient        = e8.ResourceCoefficient,
                    GroupSizeBasePointsCoefficient = e8.GroupSizeBasePointsCoefficient,
                    EffectBasePoints           = e8.EffectBasePoints,
                    ScalingClass               = 0,
                    EffectMiscValue            = e8.EffectMiscValue,
                    EffectRadiusIndex          = e8.EffectRadiusIndex,
                    EffectSpellClassMask       = e8.EffectSpellClassMask,
                    ImplicitTarget             = e8.ImplicitTarget
                };

                spellInfo.SpellEffectInfoStore.Add(new SpellEffectInfo(eff));

                if (e8.EffectTriggerSpell != 0)
                {
                    if (SpellTriggerStore.TryGetValue(e8.EffectTriggerSpell, out var set))
                        set.Add(e8.SpellID);
                    else
                        SpellTriggerStore.Add(e8.EffectTriggerSpell, new SortedSet<int> { e8.SpellID });
                }
            }

            progressHandler.SetProgress(55);

            // --- Wire V8 SpellLevels → canonical SpellLevelsEntry ---
            await Task.WhenAll(
                Task.Run(() =>
                {
                    foreach (var kv in spellLevelsV8 ?? Enumerable.Empty<KeyValuePair<int, SpellLevelsEntryV8>>())
                    {
                        var l = kv.Value;
                        if (l.DifficultyID != 0) continue;
                        if (!SpellInfoStore.TryGetValue(l.SpellID, out var spell)) continue;
                        spell.Levels = new SpellLevelsEntry
                        {
                            ID                  = l.ID,
                            DifficultyID        = l.DifficultyID,
                            SpellID             = l.SpellID,
                            BaseLevel           = l.BaseLevel,
                            MaxLevel            = l.MaxLevel,
                            SpellLevel          = l.SpellLevel,
                            MaxPassiveAuraLevel = l.MaxPassiveAuraLevel
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellCooldownsV8 ?? Enumerable.Empty<KeyValuePair<int, SpellCooldownsEntryV8>>())
                    {
                        var c = kv.Value;
                        if (c.DifficultyID != 0) continue;
                        if (!SpellInfoStore.TryGetValue(c.SpellID, out var spell)) continue;
                        spell.Cooldowns = new SpellCooldownsEntry
                        {
                            ID                   = c.ID,
                            DifficultyID         = c.DifficultyID,
                            SpellID              = c.SpellID,
                            CategoryRecoveryTime = c.CategoryRecoveryTime,
                            RecoveryTime         = c.RecoveryTime,
                            StartRecoveryTime    = c.StartRecoveryTime,
                            AuraSpellID          = 0
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellScalingV8 ?? Enumerable.Empty<KeyValuePair<int, SpellScalingEntryV8>>())
                    {
                        var s = kv.Value;
                        if (!SpellInfoStore.TryGetValue(s.SpellID, out var spell)) continue;
                        spell.Scaling = new SpellScalingEntry
                        {
                            ID                  = s.ID,
                            SpellID             = s.SpellID,
                            MinScalingLevel     = s.MinScalingLevel,
                            MaxScalingLevel     = s.MaxScalingLevel,
                            ScalesFromItemLevel = s.ScalesFromItemLevel
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellAuraRestV8 ?? Enumerable.Empty<KeyValuePair<int, SpellAuraRestrictionsEntryV8>>())
                    {
                        var r = kv.Value;
                        if (r.DifficultyID != 0) continue;
                        if (!SpellInfoStore.TryGetValue(r.SpellID, out var spell)) continue;
                        spell.AuraRestrictions = new SpellAuraRestrictionsEntry
                        {
                            ID                     = r.ID,
                            DifficultyID           = r.DifficultyID,
                            SpellID                = r.SpellID,
                            CasterAuraState        = r.CasterAuraState,
                            TargetAuraState        = r.TargetAuraState,
                            ExcludeCasterAuraState = r.ExcludeCasterAuraState,
                            ExcludeTargetAuraState = r.ExcludeTargetAuraState,
                            CasterAuraSpell        = r.CasterAuraSpell,
                            TargetAuraSpell        = r.TargetAuraSpell,
                            ExcludeCasterAuraSpell = r.ExcludeCasterAuraSpell,
                            ExcludeTargetAuraSpell = r.ExcludeTargetAuraSpell
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellAuraOptionsV8 ?? Enumerable.Empty<KeyValuePair<int, SpellAuraOptionsEntry>>())
                    {
                        if (kv.Value.DifficultyID != 0) continue;
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.AuraOptions = kv.Value;
                        if (kv.Value.SpellProcsPerMinuteID != 0 && spellProcsPerMinV8 != null &&
                            spellProcsPerMinV8.TryGetValue(kv.Value.SpellProcsPerMinuteID, out var ppm))
                            spell.ProcsPerMinute = ppm;
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellCategoriesV8 ?? Enumerable.Empty<KeyValuePair<int, SpellCategoriesEntry>>())
                    {
                        if (kv.Value.DifficultyID != 0) continue;
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.Categories = kv.Value;
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellCastingReqsV8 ?? Enumerable.Empty<KeyValuePair<int, SpellCastingRequirementsEntry>>())
                    {
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.CastingRequirements = kv.Value;
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellClassOptionsV8 ?? Enumerable.Empty<KeyValuePair<int, SpellClassOptionsEntry>>())
                    {
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.ClassOptions = kv.Value;
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellEquippedItemsV8 ?? Enumerable.Empty<KeyValuePair<int, SpellEquippedItemsEntry>>())
                    {
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.EquippedItems = kv.Value;
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellInterruptsV8 ?? Enumerable.Empty<KeyValuePair<int, SpellInterruptsEntry>>())
                    {
                        if (kv.Value.DifficultyID != 0) continue;
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.Interrupts = kv.Value;
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellPowerV8 ?? Enumerable.Empty<KeyValuePair<int, SpellPowerEntry>>())
                    {
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.Powers.Add(kv.Value);
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellReagentsV8 ?? Enumerable.Empty<KeyValuePair<int, SpellReagentsEntry>>())
                    {
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.Reagents = kv.Value;
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellReagentsCurrV8 ?? Enumerable.Empty<KeyValuePair<int, SpellReagentsCurrencyEntry>>())
                    {
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.ReagentsCurrency.Add(kv.Value);
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellShapeshiftV8 ?? Enumerable.Empty<KeyValuePair<int, SpellShapeshiftEntry>>())
                    {
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.Shapeshift = kv.Value;
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellTargetRestV8 ?? Enumerable.Empty<KeyValuePair<int, SpellTargetRestrictionsEntry>>())
                    {
                        if (kv.Value.DifficultyID != 0) continue;
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.TargetRestrictions = kv.Value;
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellTotemsV8 ?? Enumerable.Empty<KeyValuePair<int, SpellTotemsEntry>>())
                    {
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.Totems = kv.Value;
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellXSpellVisualV8 ?? Enumerable.Empty<KeyValuePair<int, SpellXSpellVisualEntry>>())
                    {
                        if (kv.Value.DifficultyID != 0 || kv.Value.CasterPlayerConditionID != 0) continue;
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.SpellXSpellVisual = kv.Value;
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellLabelV8 ?? Enumerable.Empty<KeyValuePair<int, SpellLabelEntry>>())
                    {
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.Labels.Add(kv.Value.LabelID);
                    }
                }),
                Task.Run(() =>
                {
                    if (spellXDescVarsV8 == null || spellDescVarsV8 == null) return;
                    foreach (var kv in spellXDescVarsV8)
                    {
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.DescriptionVariables = spellDescVarsV8.GetValue(kv.Value.SpellDescriptionVariablesID);
                    }
                }),
                Task.Run(() =>
                {
                    if (itemEffectV8 == null) return;
                    foreach (var kv in itemEffectV8)
                    {
                        var e8 = kv.Value;
                        if (!SpellInfoStore.ContainsKey(e8.SpellID)) continue;
                        var canonical = new ItemEffectEntry
                        {
                            ID                   = e8.ID,
                            LegacySlotIndex      = e8.LegacySlotIndex,
                            TriggerType          = e8.TriggerType,
                            Charges              = e8.Charges,
                            CoolDownMSec         = e8.CoolDownMSec,
                            CategoryCoolDownMSec = e8.CategoryCoolDownMSec,
                            SpellCategoryID      = e8.SpellCategoryID,
                            SpellID              = e8.SpellID,
                            ChrSpecializationID  = e8.ChrSpecializationID,
                            ItemID               = e8.ParentItemID
                        };
                        if (itemSparseV8 != null && itemSparseV8.TryGetValue(e8.ParentItemID, out var item))
                            canonical.Item = item;
                        SpellInfoStore[e8.SpellID].ItemEffects.Add(canonical);
                    }
                })
            );

            progressHandler.SetProgress(90);

            MySqlConnection.LoadServersideSpells();

            progressHandler.SetProgress(95);

            GameTable<GtSpellScalingEntry>.Open($@"{Settings.Default.GtPath}\SpellScaling.txt");

            progressHandler.SetProgress(100);
        }

        private static Storage<T> LoadV7DB2<T>(string name) where T : class, new()
        {
            try
            {
                var path = $@"{Settings.Default.DbcPath}\{Settings.Default.Locale}\{name}.db2";
                var reader = new DBReader(path);
                return new Storage<T>(reader);
            }
            catch (Exception ex)
            {
                var msg = ex is System.Reflection.TargetInvocationException tie ? (tie.InnerException?.Message ?? ex.Message) : ex.Message;
                Console.WriteLine($"[LoadV7] Could not load {name}.db2: {msg}");
                return null;
            }
        }

        private static Storage<T> LoadV8DB2<T>(string name, HotfixReader hotfixReader) where T : class, new()
        {
            try
            {
                return CreateInstance<Storage<T>>(name, hotfixReader);
            }
            catch (Exception ex)
            {
                var msg = ex is System.Reflection.TargetInvocationException tie ? (tie.InnerException?.Message ?? ex.Message) : ex.Message;
                Console.WriteLine($"[LoadV8] Could not load {name}.db2: {msg}");
                return null;
            }
        }

        private static async Task LoadV7(Action<int> progressCallback)
        {
            var progressHandler = new ProgressHandler(progressCallback);
            progressHandler.SetProgress(5);

            // --- Load shared tables that have identical layout in 7.x ---
            AreaGroupMember   = LoadV7DB2<AreaGroupMemberEntry>("AreaGroupMember");
            AreaTable         = LoadV7DB2<AreaTableEntry>("AreaTable");
            OverrideSpellData = LoadV7DB2<OverrideSpellDataEntry>("OverrideSpellData");
            ScreenEffect      = LoadV7DB2<ScreenEffectEntry>("ScreenEffect");
            SpellCastTimes    = LoadV7DB2<SpellCastTimesEntry>("SpellCastTimes");
            SpellDuration     = LoadV7DB2<SpellDurationEntry>("SpellDuration");
            SpellRadius       = LoadV7DB2<SpellRadiusEntry>("SpellRadius");
            SpellRange        = LoadV7DB2<SpellRangeEntry>("SpellRange");
            RandPropPoints    = LoadV7DB2<RandPropPointsEntry>("RandPropPoints");
            SkillLineAbility  = LoadV7DB2<SkillLineAbilityEntry>("SkillLineAbility");
            SkillLine         = LoadV7DB2<SkillLineEntry>("SkillLine");
            SpellCategory     = LoadV7DB2<SpellCategoryEntry>("SpellCategory");

            progressHandler.SetProgress(25);

            // --- Load V7-specific structured tables ---
            var spellsV7              = LoadV7DB2<SpellEntryV7>("Spell");
            var spellMiscsV7          = LoadV7DB2<SpellMiscEntryV7>("SpellMisc");
            var spellEffectsV7        = LoadV7DB2<SpellEffectEntryV7>("SpellEffect");
            var spellAuraOptionsV7    = LoadV7DB2<SpellAuraOptionsEntryV7>("SpellAuraOptions");
            var spellAuraRestrictV7   = LoadV7DB2<SpellAuraRestrictionsEntryV7>("SpellAuraRestrictions");
            var spellCategoriesV7     = LoadV7DB2<SpellCategoriesEntryV7>("SpellCategories");
            var spellCooldownsV7      = LoadV7DB2<SpellCooldownsEntryV7>("SpellCooldowns");
            var spellLevelsV7         = LoadV7DB2<SpellLevelsEntryV7>("SpellLevels");
            var spellTargetRestrictV7 = LoadV7DB2<SpellTargetRestrictionsEntryV7>("SpellTargetRestrictions");
            var spellClassOptionsV7   = LoadV7DB2<SpellClassOptionsEntryV7>("SpellClassOptions");
            var spellCastingReqsV7    = LoadV7DB2<SpellCastingRequirementsEntryV7>("SpellCastingRequirements");
            var spellEquippedItemsV7  = LoadV7DB2<SpellEquippedItemsEntryV7>("SpellEquippedItems");
            var spellInterruptsV7     = LoadV7DB2<SpellInterruptsEntryV7>("SpellInterrupts");
            var spellPowersV7         = LoadV7DB2<SpellPowerEntryV7>("SpellPower");
            var spellProcsPerMinV7    = LoadV7DB2<SpellProcsPerMinuteEntryV7>("SpellProcsPerMinute");
            var spellReagentsV7       = LoadV7DB2<SpellReagentsEntryV7>("SpellReagents");
            var spellReagentsCurrV7   = LoadV7DB2<SpellReagentsCurrencyEntryV7>("SpellReagentsCurrency");
            var spellScalingV7        = LoadV7DB2<SpellScalingEntryV7>("SpellScaling");
            var spellShapeshiftV7     = LoadV7DB2<SpellShapeshiftEntryV7>("SpellShapeshift");
            var spellTotemsV7         = LoadV7DB2<SpellTotemsEntryV7>("SpellTotems");
            var spellXSpellVisualsV7  = LoadV7DB2<SpellXSpellVisualEntryV7>("SpellXSpellVisual");
            var spellLabelsV7         = LoadV7DB2<SpellLabelEntry>("SpellLabel");
            var itemEffectsV7         = LoadV7DB2<ItemEffectEntryV7>("ItemEffect");
            var itemSparsesV7         = LoadV7DB2<ItemSparseEntryV7>("ItemSparse");
            var spellDescVarsV7       = LoadV7DB2<SpellDescriptionVariablesEntryV7>("SpellDescriptionVariables");

            progressHandler.SetProgress(40);

            if (spellsV7 == null)
                throw new DirectoryNotFoundException("Spell.db2 could not be loaded - check your DB2 path and locale.");

            // --- Build SpellInfoStore from V7 SpellEntry (Name is inline) ---
            // Also build a lookup from SpellID → MiscID
            var spellIdToMiscId = new Dictionary<int, int>();
            foreach (var kv in spellsV7)
            {
                var s = kv.Value;
                SpellInfoStore[s.ID] = new SpellInfo(
                    s.Name ?? string.Empty,
                    new SpellEntry
                    {
                        ID            = (uint)s.ID,
                        NameSubtext   = s.NameSubtext,
                        Description   = s.Description,
                        AuraDescription = s.AuraDescription
                    });
                if (s.MiscID != 0)
                    spellIdToMiscId[s.ID] = s.MiscID;
            }

            progressHandler.SetProgress(50);

            // --- Convert and wire V7 SpellMisc (linked via SpellEntry.MiscID, not SpellMisc.SpellID) ---
            foreach (var kv in spellMiscsV7 ?? Enumerable.Empty<KeyValuePair<int, SpellMiscEntryV7>>())
            {
                var m = kv.Value;
                // In 7.x, SpellMisc is keyed by ID. SpellEntry.MiscID references this ID.
                // Find the spell that references this MiscID.
                var spellId = spellIdToMiscId.FirstOrDefault(kvp => kvp.Value == m.ID).Key;
                if (spellId == 0) continue;
                if (!SpellInfoStore.TryGetValue(spellId, out var spell)) continue;

                var misc = new SpellMiscEntry
                {
                    ID              = (uint)m.ID,
                    SpellID         = spellId,
                    DifficultyID    = 0,
                    CastingTimeIndex = m.CastingTimeIndex,
                    DurationIndex   = m.DurationIndex,
                    RangeIndex      = m.RangeIndex,
                    SchoolMask      = m.SchoolMask,
                    Speed           = m.Speed,
                    SpellIconFileDataID    = (int)m.IconFileDataID,
                    ActiveIconFileDataID   = (int)m.ActiveIconFileDataID,
                    Attributes      = new int[15]
                };
                for (int i = 0; i < 14; i++)
                    misc.Attributes[i] = (int)m.Attributes[i];

                spell.Misc = misc;

                if (SpellDuration != null && SpellDuration.TryGetValue((int)m.DurationIndex, out var dur))
                    spell.DurationEntry = dur;
                if (SpellRange != null && SpellRange.TryGetValue((int)m.RangeIndex, out var rng))
                    spell.Range = rng;
            }

            progressHandler.SetProgress(55);

            // --- Convert V7 SpellEffect → SpellEffectEntry and build SpellEffectInfoStore ---
            foreach (var kv in spellEffectsV7 ?? Enumerable.Empty<KeyValuePair<int, SpellEffectEntryV7>>())
            {
                var e7 = kv.Value;
                if (!SpellInfoStore.TryGetValue(e7.SpellID, out var spellInfo))
                    continue;

                var eff = new SpellEffectEntry
                {
                    ID                          = e7.ID,
                    SpellID                     = e7.SpellID,
                    Effect                      = (int)e7.Effect,
                    EffectAura                  = (short)e7.EffectAura,
                    EffectIndex                 = (int)e7.EffectIndex,
                    EffectBasePoints            = e7.EffectBasePoints,
                    DifficultyID                = (int)e7.DifficultyID,
                    EffectAmplitude             = e7.EffectAmplitude,
                    EffectAuraPeriod            = (int)e7.EffectAuraPeriod,
                    EffectBonusCoefficient      = e7.EffectBonusCoefficient,
                    EffectChainAmplitude        = e7.EffectChainAmplitude,
                    EffectChainTargets          = (int)e7.EffectChainTargets,
                    EffectItemType              = (int)e7.EffectItemType,
                    EffectMechanic              = (int)e7.EffectMechanic,
                    EffectPointsPerResource     = e7.EffectPointsPerResource,
                    EffectRealPointsPerLevel    = e7.EffectRealPointsPerLevel,
                    EffectTriggerSpell          = (int)e7.EffectTriggerSpell,
                    EffectPosFacing             = e7.EffectPosFacing,
                    EffectAttributes            = (int)e7.EffectAttributes,
                    BonusCoefficientFromAP      = e7.BonusCoefficientFromAP,
                    PvpMultiplier               = e7.PvPMultiplier,
                    Coefficient                 = 0,
                    Variance                    = 0,
                    ResourceCoefficient         = 0,
                    EffectMiscValue             = new int[2] { e7.EffectMiscValues[0], e7.EffectMiscValues[1] },
                    EffectRadiusIndex           = new uint[2] { e7.EffectRadiusIndex[0], e7.EffectRadiusIndex[1] },
                    EffectSpellClassMask        = new int[4] { (int)e7.EffectSpellClassMask[0], (int)e7.EffectSpellClassMask[1], (int)e7.EffectSpellClassMask[2], (int)e7.EffectSpellClassMask[3] },
                    ImplicitTarget              = new short[2] { (short)e7.ImplicitTarget[0], (short)e7.ImplicitTarget[1] },
                    ScalingClass                = 0,
                    GroupSizeBasePointsCoefficient = 0
                };

                spellInfo.SpellEffectInfoStore.Add(new SpellEffectInfo(eff));

                var triggerId = (int)e7.EffectTriggerSpell;
                if (triggerId != 0)
                {
                    if (SpellTriggerStore.TryGetValue(triggerId, out var trigger))
                        trigger.Add(e7.SpellID);
                    else
                        SpellTriggerStore.Add(triggerId, new SortedSet<int> { e7.SpellID });
                }
            }

            progressHandler.SetProgress(60);

            // --- Wire remaining V7 stores using conversion helpers ---
            await Task.WhenAll(
                Task.Run(() =>
                {
                    foreach (var kv in spellAuraOptionsV7 ?? Enumerable.Empty<KeyValuePair<int, SpellAuraOptionsEntryV7>>())
                    {
                        var a = kv.Value;
                        if (!SpellInfoStore.TryGetValue(a.SpellID, out var spell)) continue;
                        spell.AuraOptions = new SpellAuraOptionsEntry
                        {
                            ID                    = (uint)a.ID,
                            SpellID               = a.SpellID,
                            DifficultyID          = a.DifficultyID,
                            CumulativeAura        = a.CumulativeAura,
                            ProcCategoryRecovery  = (int)a.ProcCategoryRecovery,
                            ProcChance            = a.ProcChance,
                            ProcCharges           = (int)a.ProcCharges,
                            SpellProcsPerMinuteID = (ushort)a.SpellProcsPerMinuteID,
                            ProcTypeMask          = new int[2] { (int)a.ProcTypeMask, 0 }
                        };
                        if (a.SpellProcsPerMinuteID != 0 && spellProcsPerMinV7 != null &&
                            spellProcsPerMinV7.TryGetValue(a.SpellProcsPerMinuteID, out var ppmV7))
                            spell.ProcsPerMinute = new SpellProcsPerMinuteEntry
                            {
                                ID          = (uint)ppmV7.ID,
                                BaseProcRate = ppmV7.BaseProcRate,
                                Flags       = ppmV7.Flags
                            };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellAuraRestrictV7 ?? Enumerable.Empty<KeyValuePair<int, SpellAuraRestrictionsEntryV7>>())
                    {
                        var r = kv.Value;
                        if (!SpellInfoStore.TryGetValue(r.SpellID, out var spell)) continue;
                        spell.AuraRestrictions = new SpellAuraRestrictionsEntry
                        {
                            ID                       = (uint)r.ID,
                            SpellID                  = r.SpellID,
                            DifficultyID             = r.DifficultyID,
                            CasterAuraSpell          = (int)r.CasterAuraSpell,
                            TargetAuraSpell          = (int)r.TargetAuraSpell,
                            ExcludeCasterAuraSpell   = (int)r.ExcludeCasterAuraSpell,
                            ExcludeTargetAuraSpell   = (int)r.ExcludeTargetAuraSpell,
                            CasterAuraState          = r.CasterAuraState,
                            TargetAuraState          = r.TargetAuraState,
                            ExcludeCasterAuraState   = r.ExcludeCasterAuraState,
                            ExcludeTargetAuraState   = r.ExcludeTargetAuraState,
                            CasterAuraType           = 0,
                            TargetAuraType           = 0,
                            ExcludeCasterAuraType    = 0,
                            ExcludeTargetAuraType    = 0
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellCategoriesV7 ?? Enumerable.Empty<KeyValuePair<int, SpellCategoriesEntryV7>>())
                    {
                        var c = kv.Value;
                        if (!SpellInfoStore.TryGetValue(c.SpellID, out var spell)) continue;
                        spell.Categories = new SpellCategoriesEntry
                        {
                            ID                    = (uint)c.ID,
                            SpellID               = c.SpellID,
                            DifficultyID          = c.DifficultyID,
                            Category              = (short)c.Category,
                            StartRecoveryCategory = (short)c.StartRecoveryCategory,
                            ChargeCategory        = (short)c.ChargeCategory,
                            DefenseType           = (sbyte)c.DefenseType,
                            DispelType            = (sbyte)c.DispelType,
                            Mechanic              = (sbyte)c.Mechanic,
                            PreventionType        = (sbyte)c.PreventionType
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellCooldownsV7 ?? Enumerable.Empty<KeyValuePair<int, SpellCooldownsEntryV7>>())
                    {
                        var c = kv.Value;
                        if (!SpellInfoStore.TryGetValue(c.SpellID, out var spell)) continue;
                        spell.Cooldowns = new SpellCooldownsEntry
                        {
                            ID                   = (uint)c.ID,
                            SpellID              = c.SpellID,
                            DifficultyID         = c.DifficultyID,
                            CategoryRecoveryTime = (int)c.CategoryRecoveryTime,
                            RecoveryTime         = (int)c.RecoveryTime,
                            StartRecoveryTime    = (int)c.StartRecoveryTime,
                            AuraSpellID          = 0
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellLevelsV7 ?? Enumerable.Empty<KeyValuePair<int, SpellLevelsEntryV7>>())
                    {
                        var l = kv.Value;
                        if (l.DifficultyID != 0) continue;
                        if (!SpellInfoStore.TryGetValue(l.SpellID, out var spell)) continue;
                        spell.Levels = new SpellLevelsEntry
                        {
                            ID                 = (uint)l.ID,
                            SpellID            = l.SpellID,
                            DifficultyID       = l.DifficultyID,
                            BaseLevel          = l.BaseLevel,
                            MaxLevel           = (short)l.MaxLevel,
                            SpellLevel         = l.SpellLevel,
                            MaxPassiveAuraLevel = l.MaxUsableLevel
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellTargetRestrictV7 ?? Enumerable.Empty<KeyValuePair<int, SpellTargetRestrictionsEntryV7>>())
                    {
                        var t = kv.Value;
                        if (!SpellInfoStore.TryGetValue(t.SpellID, out var spell)) continue;
                        spell.TargetRestrictions = new SpellTargetRestrictionsEntry
                        {
                            ID                  = (uint)t.ID,
                            SpellID             = t.SpellID,
                            DifficultyID        = t.DifficultyID,
                            ConeDegrees         = t.ConeAngle,
                            Width               = t.Width,
                            Targets             = (int)t.Targets,
                            TargetCreatureType  = (short)t.TargetCreatureType,
                            MaxTargets          = t.MaxAffectedTargets,
                            MaxTargetLevel      = t.MaxTargetLevel
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellClassOptionsV7 ?? Enumerable.Empty<KeyValuePair<int, SpellClassOptionsEntryV7>>())
                    {
                        var c = kv.Value;
                        if (!SpellInfoStore.TryGetValue(c.SpellID, out var spell)) continue;
                        spell.ClassOptions = new SpellClassOptionsEntry
                        {
                            ID             = (uint)c.ID,
                            SpellID        = c.SpellID,
                            ModalNextSpell = c.ModalNextSpell,
                            SpellClassSet  = c.SpellClassSet,
                            SpellClassMask = new int[4]
                            {
                                (int)c.SpellFamilyFlags[0],
                                (int)c.SpellFamilyFlags[1],
                                (int)c.SpellFamilyFlags[2],
                                (int)c.SpellFamilyFlags[3]
                            }
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellCastingReqsV7 ?? Enumerable.Empty<KeyValuePair<int, SpellCastingRequirementsEntryV7>>())
                    {
                        var c = kv.Value;
                        if (!SpellInfoStore.TryGetValue(c.SpellID, out var spell)) continue;
                        spell.CastingRequirements = new SpellCastingRequirementsEntry
                        {
                            ID                 = (uint)c.ID,
                            SpellID            = c.SpellID,
                            FacingCasterFlags  = c.FacingCasterFlags,
                            MinFactionID       = c.MinFactionID,
                            MinReputation      = c.MinReputation,
                            RequiredAreasID    = c.RequiredAreasID,
                            RequiredAuraVision = c.RequiredAuraVision,
                            RequiresSpellFocus = c.RequiresSpellFocus
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellEquippedItemsV7 ?? Enumerable.Empty<KeyValuePair<int, SpellEquippedItemsEntryV7>>())
                    {
                        var e = kv.Value;
                        if (!SpellInfoStore.TryGetValue(e.SpellID, out var spell)) continue;
                        spell.EquippedItems = new SpellEquippedItemsEntry
                        {
                            ID                  = (uint)e.ID,
                            SpellID             = e.SpellID,
                            EquippedItemClass   = (sbyte)e.EquippedItemClass,
                            EquippedItemInvTypes = (int)e.EquippedItemInventoryTypeMask,
                            EquippedItemSubclass = (int)e.EquippedItemSubClassMask
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellInterruptsV7 ?? Enumerable.Empty<KeyValuePair<int, SpellInterruptsEntryV7>>())
                    {
                        var i = kv.Value;
                        if (i.DifficultyID != 0) continue;
                        if (!SpellInfoStore.TryGetValue(i.SpellID, out var spell)) continue;
                        spell.Interrupts = new SpellInterruptsEntry
                        {
                            ID                   = (uint)i.ID,
                            SpellID              = i.SpellID,
                            DifficultyID         = i.DifficultyID,
                            InterruptFlags       = (short)i.InterruptFlags,
                            AuraInterruptFlags   = new int[2] { (int)i.AuraInterruptFlags[0], (int)i.AuraInterruptFlags[1] },
                            ChannelInterruptFlags = new int[2] { (int)i.ChannelInterruptFlags[0], (int)i.ChannelInterruptFlags[1] }
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellPowersV7 ?? Enumerable.Empty<KeyValuePair<int, SpellPowerEntryV7>>())
                    {
                        var p = kv.Value;
                        if (!SpellInfoStore.TryGetValue((int)p.SpellID, out var spell)) continue;
                        spell.Powers.Add(new SpellPowerEntry
                        {
                            ID                  = p.ID,
                            OrderIndex          = p.PowerIndex,
                            ManaCost            = (int)p.ManaCost,
                            ManaCostPerLevel    = (int)p.ManaCostPerLevel,
                            ManaPerSecond       = (int)p.ManaCostPerSecond,
                            PowerDisplayID      = p.PowerDisplayID,
                            AltPowerBarID       = (int)p.UnitPowerBarID,
                            PowerCostPct        = p.ManaCostPercentage,
                            PowerCostMaxPct     = 0,
                            OptionalCostPct     = 0,
                            PowerPctPerSecond   = p.ManaCostPercentagePerSecond,
                            PowerType           = (sbyte)p.PowerType,
                            RequiredAuraSpellID = (int)p.RequiredAura,
                            OptionalCost        = 0,
                            SpellID             = (int)p.SpellID
                        });
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellReagentsV7 ?? Enumerable.Empty<KeyValuePair<int, SpellReagentsEntryV7>>())
                    {
                        var r = kv.Value;
                        if (!SpellInfoStore.TryGetValue(r.SpellID, out var spell)) continue;
                        var reagent = new int[8];
                        var reagentCount = new short[8];
                        for (int i = 0; i < 8; i++)
                        {
                            reagent[i] = (int)r.Reagent[i];
                            reagentCount[i] = (short)r.ReagentCount[i];
                        }
                        spell.Reagents = new SpellReagentsEntry
                        {
                            ID        = (uint)r.ID,
                            SpellID   = r.SpellID,
                            Reagent   = reagent,
                            ReagentCount = reagentCount,
                            ReagentRecraftCount = new short[8],
                            ReagentSource = new byte[8]
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellReagentsCurrV7 ?? Enumerable.Empty<KeyValuePair<int, SpellReagentsCurrencyEntryV7>>())
                    {
                        var r = kv.Value;
                        if (!SpellInfoStore.TryGetValue(r.SpellID, out var spell)) continue;
                        spell.ReagentsCurrency.Add(new SpellReagentsCurrencyEntry
                        {
                            ID              = (uint)r.ID,
                            SpellID         = r.SpellID,
                            CurrencyTypesID = r.CurrencyTypeID,
                            CurrencyCount   = r.CurrencyCount
                        });
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellScalingV7 ?? Enumerable.Empty<KeyValuePair<int, SpellScalingEntryV7>>())
                    {
                        var s = kv.Value;
                        if (!SpellInfoStore.TryGetValue(s.SpellID, out var spell)) continue;
                        spell.Scaling = new SpellScalingEntry
                        {
                            ID                  = (uint)s.ID,
                            SpellID             = s.SpellID,
                            MinScalingLevel     = s.MinScalingLevel,
                            MaxScalingLevel     = s.MaxScalingLevel,
                            ScalesFromItemLevel = (short)s.ScalesFromItemLevel
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellShapeshiftV7 ?? Enumerable.Empty<KeyValuePair<int, SpellShapeshiftEntryV7>>())
                    {
                        var s = kv.Value;
                        if (!SpellInfoStore.TryGetValue(s.SpellID, out var spell)) continue;
                        spell.Shapeshift = new SpellShapeshiftEntry
                        {
                            ID               = (uint)s.ID,
                            SpellID          = s.SpellID,
                            StanceBarOrder   = (sbyte)s.StanceBarOrder,
                            ShapeshiftExclude = new int[2] { (int)s.ShapeshiftExclude[0], (int)s.ShapeshiftExclude[1] },
                            ShapeshiftMask   = new int[2] { (int)s.ShapeshiftMask[0], (int)s.ShapeshiftMask[1] }
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellTotemsV7 ?? Enumerable.Empty<KeyValuePair<int, SpellTotemsEntryV7>>())
                    {
                        var t = kv.Value;
                        if (!SpellInfoStore.TryGetValue(t.SpellID, out var spell)) continue;
                        spell.Totems = new SpellTotemsEntry
                        {
                            ID                    = (uint)t.ID,
                            SpellID               = t.SpellID,
                            RequiredTotemCategoryID = new ushort[2] { t.RequiredTotemCategoryID[0], t.RequiredTotemCategoryID[1] },
                            Totem                 = new int[2] { (int)t.Totem[0], (int)t.Totem[1] }
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellXSpellVisualsV7 ?? Enumerable.Empty<KeyValuePair<int, SpellXSpellVisualEntryV7>>())
                    {
                        var v = kv.Value;
                        if (v.DifficultyID != 0 || v.CasterPlayerConditionID != 0) continue;
                        if (!SpellInfoStore.TryGetValue(v.SpellID, out var spell)) continue;
                        spell.SpellXSpellVisual = new SpellXSpellVisualEntry
                        {
                            ID                    = v.ID,
                            DifficultyID          = v.DifficultyID,
                            SpellVisualID         = v.SpellVisualID,
                            Probability           = v.Chance,
                            Flags                 = v.Flags,
                            Priority              = v.Priority,
                            SpellIconFileID       = (int)v.IconFileDataID,
                            ActiveIconFileID      = (int)v.ActiveIconFileDataID,
                            ViewerUnitConditionID = v.UnitConditionID,
                            ViewerPlayerConditionID = 0,
                            CasterUnitConditionID = v.CasterUnitConditionID,
                            CasterPlayerConditionID = v.CasterPlayerConditionID,
                            SpellID               = v.SpellID
                        };
                    }
                }),
                Task.Run(() =>
                {
                    foreach (var kv in spellLabelsV7 ?? Enumerable.Empty<KeyValuePair<int, SpellLabelEntry>>())
                    {
                        if (!SpellInfoStore.TryGetValue(kv.Value.SpellID, out var spell)) continue;
                        spell.Labels.Add(kv.Value.LabelID);
                    }
                }),
                Task.Run(() =>
                {
                    if (itemEffectsV7 == null) return;
                    foreach (var kv in itemEffectsV7)
                    {
                        var e7 = kv.Value;
                        if (!SpellInfoStore.ContainsKey((int)e7.SpellID)) continue;
                        var canonical = new ItemEffectEntry
                        {
                            ID                   = (uint)e7.ID,
                            LegacySlotIndex      = e7.OrderIndex,
                            TriggerType          = (sbyte)e7.Trigger,
                            Charges              = e7.Charges,
                            CoolDownMSec         = e7.Cooldown,
                            CategoryCoolDownMSec = e7.CategoryCooldown,
                            SpellCategoryID      = e7.Category,
                            SpellID              = (int)e7.SpellID,
                            ChrSpecializationID  = e7.ChrSpecializationID,
                            ItemID               = (int)e7.ItemID
                        };
                        if (itemSparsesV7 != null && itemSparsesV7.TryGetValue((int)e7.ItemID, out var itemV7))
                            canonical.Item = new ItemSparseEntry { ID = (uint)itemV7.ID, Description = itemV7.Description, Display = itemV7.Name, Display1 = itemV7.Name2, Display2 = itemV7.Name3, Display3 = itemV7.Name4, ItemLevel = itemV7.ItemLevel, RequiredLevel = itemV7.RequiredLevel, OverallQualityID = itemV7.Quality, InventoryType = itemV7.InventoryType };
                        SpellInfoStore[(int)e7.SpellID].ItemEffects.Add(canonical);
                    }
                }),
                Task.Run(() =>
                {
                    if (spellDescVarsV7 == null) return;
                    foreach (var kv in spellsV7)
                    {
                        if (kv.Value.DescriptionVariablesID == 0) continue;
                        if (!SpellInfoStore.TryGetValue(kv.Value.ID, out var spell)) continue;
                        if (spellDescVarsV7.TryGetValue(kv.Value.DescriptionVariablesID, out var descVar))
                            spell.DescriptionVariables = new SpellDescriptionVariablesEntry
                            {
                                ID       = (uint)descVar.ID,
                                Variables = descVar.Variables
                            };
                    }
                })
            );

            progressHandler.SetProgress(90);

            MySqlConnection.LoadServersideSpells();

            progressHandler.SetProgress(95);

            GameTable<GtSpellScalingEntry>.Open($@"{Settings.Default.GtPath}\SpellScaling.txt");

            progressHandler.SetProgress(100);
        }

        public static uint SelectedLevel;
        public static uint SelectedItemLevel = 475;
        public static MapDifficultyEntry SelectedMapDifficulty;

        static DBC()
        {
            SelectedLevel = MaxLevel;
        }
    }
}
