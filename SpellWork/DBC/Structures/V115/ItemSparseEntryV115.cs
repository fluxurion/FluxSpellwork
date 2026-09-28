using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class ItemSparseEntryV115 : IConvertsTo<ItemSparseEntry>
    {
        [Index(true)]
        public int ID;
        public string Description_lang;
        public string Display3_lang;
        public string Display2_lang;
        public string Display1_lang;
        public string Display_lang;
        public float DmgVariance;
        public uint DurationInInventory;
        public float QualityModifier;
        public uint BagFamily;
        public int StartQuestID;
        public float ItemRange;
        [Cardinality(10)]
        public float[] StatPercentageOfSocket = new float[10];
        [Cardinality(10)]
        public int[] StatPercentEditor = new int[10];
        [Cardinality(10)]
        public int[] Field_1_15_3_55112_014 = new int[10];
        [Cardinality(10)]
        public int[] StatModifier_bonusStat = new int[10];
        public int Stackable;
        public int MaxCount;
        public int MinReputation;
        public uint RequiredAbility;
        [Cardinality(2)]
        public int[] AllowableRace = new int[2];
        public uint SellPrice;
        public uint BuyPrice;
        public uint VendorStackCount;
        public float PriceVariance;
        public float PriceRandomValue;
        [Cardinality(5)]
        public int[] Flags = new int[5];
        public int OppositeFactionItemID;
        public int ModifiedCraftingReagentItemID;
        public int ContentTuningID;
        public int PlayerLevelToItemLevelCurveID;
        public uint MaxDurability;
        public ushort ItemNameDescriptionID;
        public ushort RequiredTransmogHoliday;
        public ushort RequiredHoliday;
        public ushort LimitCategory;
        public ushort Gem_properties;
        public ushort Socket_match_enchantment_ID;
        public ushort TotemCategoryID;
        public ushort InstanceBound;
        [Cardinality(2)]
        public ushort[] ZoneBound = new ushort[2];
        public ushort ItemSet;
        public ushort LockID;
        public ushort PageID;
        public ushort ItemDelay;
        public ushort MinFactionID;
        public ushort RequiredSkillRank;
        public ushort RequiredSkill;
        public ushort ItemLevel;
        public short AllowableClass;
        public ushort ItemRandomSuffixGroupID;
        public ushort RandomSelect;
        [Cardinality(5)]
        public ushort[] MinDamage = new ushort[5];
        [Cardinality(5)]
        public ushort[] MaxDamage = new ushort[5];
        [Cardinality(7)]
        public short[] Resistances = new short[7];
        public ushort Field_1_15_7_59706_054;
        [Cardinality(10)]
        public short[] StatModifier_bonusAmount = new short[10];
        public byte ExpansionID;
        public byte ArtifactID;
        public byte SpellWeight;
        public byte SpellWeightCategory;
        [Cardinality(3)]
        public byte[] SocketType = new byte[3];
        public byte SheatheType;
        public byte Material;
        public byte PageMaterialID;
        public byte LanguageID;
        public byte Bonding;
        public byte DamageType;
        public byte ContainerSlots;
        public byte RequiredPVPMedal;
        public sbyte RequiredPVPRank;
        public sbyte InventoryType;
        public sbyte OverallQualityID;
        public byte AmmunitionType;
        public sbyte RequiredLevel;

        public ItemSparseEntry ToCanonical()
        {
            var e = new ItemSparseEntry
            {
                ID = (uint)ID,
                Description = Description_lang,
                Display3 = Display3_lang,
                Display2 = Display2_lang,
                Display1 = Display1_lang,
                Display = Display_lang,
                DmgVariance = DmgVariance,
                DurationInInventory = DurationInInventory,
                QualityModifier = QualityModifier,
                BagFamily = BagFamily,
                StartQuestID = (ushort)StartQuestID,
                ItemRange = ItemRange,
                StatPercentageOfSocket = StatPercentageOfSocket,
                StatPercentEditor = StatPercentEditor,
                StatModifierBonusStat = new sbyte[] { (sbyte)StatModifier_bonusStat[0], (sbyte)StatModifier_bonusStat[1], (sbyte)StatModifier_bonusStat[2], (sbyte)StatModifier_bonusStat[3], (sbyte)StatModifier_bonusStat[4], (sbyte)StatModifier_bonusStat[5], (sbyte)StatModifier_bonusStat[6], (sbyte)StatModifier_bonusStat[7], (sbyte)StatModifier_bonusStat[8], (sbyte)StatModifier_bonusStat[9] },
                Stackable = Stackable,
                MaxCount = MaxCount,
                MinReputation = MinReputation,
                RequiredAbility = RequiredAbility,
                AllowableRace = (long)((AllowableRace[0] & 0xFFFFFFFFL) | ((long)AllowableRace[1] << 32)),
                SellPrice = SellPrice,
                BuyPrice = BuyPrice,
                VendorStackCount = VendorStackCount,
                PriceVariance = PriceVariance,
                PriceRandomValue = PriceRandomValue,
                FactionRelated = OppositeFactionItemID,
                ModifiedCraftingReagentItemID = ModifiedCraftingReagentItemID,
                ContentTuningID = ContentTuningID,
                PlayerLevelToItemLevelCurveID = PlayerLevelToItemLevelCurveID,
                ItemNameDescriptionID = ItemNameDescriptionID,
                RequiredTransmogHoliday = RequiredTransmogHoliday,
                RequiredHoliday = RequiredHoliday,
                LimitCategory = LimitCategory,
                GemProperties = Gem_properties,
                SocketMatchEnchantmentId = Socket_match_enchantment_ID,
                TotemCategoryID = TotemCategoryID,
                InstanceBound = (int)InstanceBound,
                ZoneBound = ZoneBound,
                ItemSet = ItemSet,
                LockID = LockID,
                PageID = PageID,
                ItemDelay = ItemDelay,
                MinFactionID = MinFactionID,
                RequiredSkillRank = RequiredSkillRank,
                RequiredSkill = RequiredSkill,
                ItemLevel = ItemLevel,
                AllowableClass = AllowableClass,
                ExpansionID = (int)ExpansionID,
                ArtifactID = ArtifactID,
                SpellWeight = SpellWeight,
                SpellWeightCategory = SpellWeightCategory,
                SocketType = SocketType,
                SheatheType = SheatheType,
                Material = Material,
                PageMaterialID = PageMaterialID,
                LanguageID = LanguageID,
                Bonding = Bonding,
                DamageDamageType = DamageType,
                ContainerSlots = ContainerSlots,
                RequiredPVPMedal = RequiredPVPMedal,
                RequiredPVPRank = (byte)RequiredPVPRank,
                InventoryType = (byte)InventoryType,
                OverallQualityID = (byte)OverallQualityID,
                RequiredLevel = RequiredLevel,
            };
            System.Array.Copy(Flags, e.Flags, 4);
            return e;
        }
    }
}
