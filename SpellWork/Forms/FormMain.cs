using SpellWork.Database;
using SpellWork.Extensions;
using SpellWork.Filtering;
using SpellWork.Spell;
using SpellWork.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SpellWork.Forms
{
    public sealed partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            splitContainer3.SplitterDistance = 200;

            Text = DBC.DBC.Version;

            _cbSpellFamilyName.SetEnumValues<SpellFamilyNames>("SpellFamilyName");
            _cbSpellAura.SetEnumValues<AuraType>("Aura");
            _cbSpellEffect.SetEnumValues<SpellEffects>("Effect");
            _cbTarget1.SetEnumValues<Targets>("Target A");
            _cbTarget2.SetEnumValues<Targets>("Target B");

            _cbProcSpellFamilyName.SetEnumValues<SpellFamilyNames>("SpellFamilyName");
            _cbProcSpellAura.SetEnumValues<AuraType>("Aura");
            _cbProcSpellEffect.SetEnumValues<SpellEffects>("Effect");
            _cbProcTarget1.SetEnumValues<Targets>("Target A");
            _cbProcTarget2.SetEnumValues<Targets>("Target B");

            _cbProcSpellFamilyTree.SetEnumValues<SpellFamilyNames>("SpellFamilyTree");
            _cbProcFitstSpellFamily.SetEnumValues<SpellFamilyNames>("SpellFamilyName");

            _clbSchools.SetFlags(new[]
            {
                SpellSchoolMask.SPELL_SCHOOL_MASK_NORMAL,
                SpellSchoolMask.SPELL_SCHOOL_MASK_HOLY,
                SpellSchoolMask.SPELL_SCHOOL_MASK_FIRE,
                SpellSchoolMask.SPELL_SCHOOL_MASK_NATURE,
                SpellSchoolMask.SPELL_SCHOOL_MASK_FROST,
                SpellSchoolMask.SPELL_SCHOOL_MASK_SHADOW,
                SpellSchoolMask.SPELL_SCHOOL_MASK_ARCANE
            }, "SPELL_SCHOOL_MASK_");
            _clbProcFlags.SetFlags<ProcFlags>("PROC_FLAG_");
            _clbProcFlags.AddFlags<ProcFlags2>("PROC_FLAG_2_", 1);
            _clbSpellTypeMask.SetFlags(new[]
            {
                ProcFlagsSpellType.PROC_SPELL_TYPE_DAMAGE,
                ProcFlagsSpellType.PROC_SPELL_TYPE_HEAL,
                ProcFlagsSpellType.PROC_SPELL_TYPE_NO_DMG_HEAL
            }, "PROC_SPELL_TYPE_");
            _clbSpellPhaseMask.SetFlags(new[]
            {
                ProcFlagsSpellPhase.PROC_SPELL_PHASE_CAST,
                ProcFlagsSpellPhase.PROC_SPELL_PHASE_HIT,
                ProcFlagsSpellPhase.PROC_SPELL_PHASE_FINISH
            }, "PROC_SPELL_PHASE_");
            _clbProcFlagHit.SetFlags<ProcFlagsHit>("PROC_HIT_");
            _clbProcAttributes.SetFlags<ProcAttributes>("PROC_ATTR_");

            _cbSqlSpellFamily.SetEnumValues<SpellFamilyNames>("SpellFamilyName");

            _cbAdvancedFilter1.SetStructFields<SpellInfo>();
            _cbAdvancedFilter2.SetStructFields<SpellInfo>();

            _cbAdvancedEffectFilter1.SetStructFields<SpellEffectInfo>();
            _cbAdvancedEffectFilter2.SetStructFields<SpellEffectInfo>();

            _cbAdvancedFilter1CompareType.SetEnumValuesDirect<CompareType>(true);
            _cbAdvancedFilter2CompareType.SetEnumValuesDirect<CompareType>(true);

            _cbAdvancedEffectFilter1CompareType.SetEnumValuesDirect<CompareType>(true);
            _cbAdvancedEffectFilter2CompareType.SetEnumValuesDirect<CompareType>(true);

            RefreshConnectionStatus();
        }

        #region FORM

        private void ExitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void TabControl1SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedIndex = ((TabControl)sender).SelectedIndex;
            _cbProcFlag.Visible = _bWrite.Visible = selectedIndex == 1;
            _bSpellScript.Visible = selectedIndex == 0;
            _bCopySpellInfo.Visible = selectedIndex == 0;
        }

        private void SettingsClick(object sender, EventArgs e)
        {
            var frm = new FormSettings();
            frm.ShowDialog(this);
            RefreshConnectionStatus();
        }

        private void ChangeSetupClick(object sender, EventArgs e)
        {
            var setup = new FormSetup();
            if (setup.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show(
                    "Settings saved. The application will now restart to apply changes.",
                    "Restart Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                System.Windows.Forms.Application.Restart();
            }
        }

        private void FormMainResize(object sender, EventArgs e)
        {
            try
            {
                _scCompareRoot.SplitterDistance = (((Form)sender).Size.Width / 2) - 25;
                _chDescription.Width = (((Form)sender).Size.Width - 306);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        private void RefreshConnectionStatus()
        {
            MySqlConnection.TestConnect();

            if (MySqlConnection.Connected)
            {
                _dbConnect.Text = @"Connection successful.";
                _dbConnect.ForeColor = Color.Green;
            }
            else
            {
                _dbConnect.Text = @"No DB Connected";
                _dbConnect.ForeColor = Color.Red;
            }
        }

        private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!((char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void LevelScalingClick(object sender, EventArgs e)
        {
            var scalingForm = new FormSpellScaling();
            var ret = scalingForm.ShowDialog(this);
            if (ret == DialogResult.OK)
            {
                DBC.DBC.SelectedLevel = scalingForm.SelectedLevel;
                DBC.DBC.SelectedItemLevel = scalingForm.SelectedItemLevel;
                DBC.DBC.SelectedMapDifficulty = scalingForm.SelectedMapDifficulty;
                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        LvSpellListSelectedIndexChanged(null, null);
                        break;
                    case 1:
                        LvProcSpellListSelectedIndexChanged(null, null);
                        break;
                    case 2:
                        break;
                    case 3:
                        CompareFilterSpellTextChanged(null, null);
                        break;
                }
            }
        }

        private void SpellScriptClick(object sender, EventArgs e)
        {
            if (_lvSpellList.SelectedIndices.Count <= 0)
            {
                MessageBox.Show(@"Please select a spell first.", @"SpellScript", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var spell = _spellList[_lvSpellList.SelectedIndices[0]];
            var generatedScript = BuildSpellScriptTemplate(spell);

            using (var previewForm = new Form())
            using (var scriptTextBox = new TextBox())
            {
                var gameVersion = Settings.Default.GameVersion ?? "10.x";
                previewForm.Text = $@"Generated SpellScript ({gameVersion}) - {spell.ID}";
                previewForm.StartPosition = FormStartPosition.CenterParent;
                previewForm.Size = new Size(1000, 700);
                previewForm.MinimumSize = new Size(700, 500);

                scriptTextBox.Dock = DockStyle.Fill;
                scriptTextBox.Multiline = true;
                scriptTextBox.ReadOnly = true;
                scriptTextBox.ScrollBars = ScrollBars.Both;
                scriptTextBox.WordWrap = false;
                scriptTextBox.Font = new Font("Consolas", 10);
                scriptTextBox.Text = generatedScript;

                previewForm.Controls.Add(scriptTextBox);
                previewForm.ShowDialog(this);
            }
        }

        private void CopySpellInfoClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_rtSpellInfo.Text))
            {
                MessageBox.Show(@"No spell info available to copy.", @"Copy Spell Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Clipboard.SetText(_rtSpellInfo.Text);
            _status.Text = @"Spell info copied to clipboard.";
        }

        private static string BuildSpellScriptTemplate(SpellInfo spell)
        {
            var gameVersion = Settings.Default.GameVersion;
            if (gameVersion == "7.x" || gameVersion == "8.x")
                return BuildSpellScriptTemplateV8(spell, gameVersion);
            return BuildSpellScriptTemplateV10(spell);
        }

        private static string BuildSpellScriptTemplateV10(SpellInfo spell)
        {
            var scriptToken = BuildScriptToken(spell.Name);
            var className = $"spell_custom_{scriptToken}";
            var scriptName = $"spell_custom_{scriptToken}";

            var effects = spell.SpellEffectInfoStore
                .Where(e => e.SpellEffect != null)
                .OrderBy(e => e.EffectIndex)
                .ToList();

            var triggeredSpellIds = effects
                .Select(e => e.EffectTriggerSpell)
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            var hasAuraEffects = effects.Any(e => e.EffectAura > 0);
            var sb = new StringBuilder();
            sb.AppendLine("// Example generated from SpellWork for TrinityCore 10.x branch.");
            sb.AppendLine("// Review targets, effect types and guard checks before committing.");
            sb.AppendLine($"// {spell.ID} - {spell.NameAndSubname}");
            sb.AppendLine($"class {className} : public SpellScript");
            sb.AppendLine("{");
            sb.AppendLine($"    PrepareSpellScript({className});");
            sb.AppendLine();
            sb.AppendLine("    bool Load() override");
            sb.AppendLine("    {");
            sb.AppendLine("        return GetCaster() != nullptr;");
            sb.AppendLine("    }");
            sb.AppendLine();

            if (triggeredSpellIds.Count > 0)
            {
                sb.AppendLine("    bool Validate(SpellInfo const* /*spellInfo*/) override");
                sb.AppendLine("    {");
                sb.AppendLine("        return ValidateSpellInfo(");
                sb.AppendLine("            {");
                for (var i = 0; i < triggeredSpellIds.Count; i++)
                {
                    var separator = i + 1 == triggeredSpellIds.Count ? string.Empty : ",";
                    sb.AppendLine($"                {triggeredSpellIds[i]}{separator}");
                }
                sb.AppendLine("            });");
                sb.AppendLine("    }");
                sb.AppendLine();
            }

            if (effects.Count == 0)
            {
                sb.AppendLine("    void HandleDummy(SpellEffIndex /*effIndex*/)");
                sb.AppendLine("    {");
                sb.AppendLine("        // Spell has no explicit effects in loaded data; use this as fallback logic.");
                sb.AppendLine("    }");
                sb.AppendLine();
                sb.AppendLine("    void Register() override");
                sb.AppendLine("    {");
                sb.AppendLine("        OnEffectHitTarget += SpellEffectFn(" + className + "::HandleDummy, EFFECT_0, SPELL_EFFECT_DUMMY);");
                sb.AppendLine("    }");
            }
            else
            {
                foreach (var effect in effects)
                {
                    var effectName = GetSpellEffectToken(effect.Effect);
                    var auraName = ((AuraType)effect.EffectAura).ToString();
                    var methodName = $"HandleEffect{effect.EffectIndex}";
                    var hook = GetSpellHookForEffect(effect.Effect);

                    sb.AppendLine($"    // Effect {effect.EffectIndex}: {effectName}, Aura: {auraName}");
                    sb.AppendLine($"    void {methodName}(SpellEffIndex /*effIndex*/)");
                    sb.AppendLine("    {");
                    sb.AppendLine("        Unit* caster = GetCaster();");
                    sb.AppendLine("        if (!caster)");
                    sb.AppendLine("            return;");
                    sb.AppendLine();
                    sb.AppendLine("        Unit* target = GetHitUnit();");
                    sb.AppendLine("        if (!target)");
                    sb.AppendLine("            return;");
                    sb.AppendLine();

                    if (effect.EffectTriggerSpell > 0)
                    {
                        sb.AppendLine($"        // Example trigger from dbc: {effect.EffectTriggerSpell}");
                        sb.AppendLine($"        caster->CastSpell(target, {effect.EffectTriggerSpell}, true);");
                    }
                    else
                    {
                        sb.AppendLine("        // Example custom logic:");
                        sb.AppendLine("        // caster->CastSpell(target, <spellId>, true);");
                        sb.AppendLine("        // target->RemoveAurasDueToSpell(<spellId>);");
                        sb.AppendLine("        // SetHitDamage(CalculatePct(GetHitDamage(), <percent>));");
                    }

                    if (hook == "OnCheckCast")
                    {
                        sb.AppendLine("        // For cast checks, return SpellCastResult instead of using target logic.");
                    }

                    sb.AppendLine("    }");
                    sb.AppendLine();
                }

                sb.AppendLine("    SpellCastResult CheckCast()");
                sb.AppendLine("    {");
                sb.AppendLine("        Unit* caster = GetCaster();");
                sb.AppendLine("        if (!caster)");
                sb.AppendLine("            return SPELL_FAILED_DONT_REPORT;");
                sb.AppendLine();
                sb.AppendLine("        // Example condition:");
                sb.AppendLine("        // if (!caster->IsInCombat())");
                sb.AppendLine("        //     return SPELL_FAILED_CASTER_AURASTATE;");
                sb.AppendLine();
                sb.AppendLine("        return SPELL_CAST_OK;");
                sb.AppendLine("    }");
                sb.AppendLine();

                sb.AppendLine("    void Register() override");
                sb.AppendLine("    {");
                sb.AppendLine($"        OnCheckCast += SpellCheckCastFn({className}::CheckCast);");
                foreach (var effect in effects)
                {
                    var effectName = GetSpellEffectToken(effect.Effect);
                    var hook = GetSpellHookForEffect(effect.Effect);
                    sb.AppendLine($"        {hook} += SpellEffectFn({className}::HandleEffect{effect.EffectIndex}, {GetEffectIndexToken(effect.EffectIndex)}, {effectName});");
                }
                sb.AppendLine("    }");
            }

            sb.AppendLine("};");
            sb.AppendLine();

            if (hasAuraEffects)
            {
                var auraClassName = $"{className}_aura";
                sb.AppendLine($"class {auraClassName} : public AuraScript");
                sb.AppendLine("{");
                sb.AppendLine($"    PrepareAuraScript({auraClassName});");
                sb.AppendLine();
                sb.AppendLine("    bool Validate(SpellInfo const* /*spellInfo*/) override");
                sb.AppendLine("    {");
                sb.AppendLine($"        return ValidateSpellInfo({{{spell.ID}}});");
                sb.AppendLine("    }");
                sb.AppendLine();
                sb.AppendLine("    void OnApply(AuraEffect const* /*aurEff*/, AuraEffectHandleModes /*mode*/)");
                sb.AppendLine("    {");
                sb.AppendLine("        Unit* target = GetTarget();");
                sb.AppendLine("        if (!target)");
                sb.AppendLine("            return;");
                sb.AppendLine();
                sb.AppendLine("        // Example apply logic for aura effects.");
                sb.AppendLine("    }");
                sb.AppendLine();
                sb.AppendLine("    void OnRemove(AuraEffect const* /*aurEff*/, AuraEffectHandleModes /*mode*/)");
                sb.AppendLine("    {");
                sb.AppendLine("        // Example cleanup logic.");
                sb.AppendLine("    }");
                sb.AppendLine();
                sb.AppendLine("    void Register() override");
                sb.AppendLine("    {");
                foreach (var auraEffect in effects.Where(e => e.EffectAura > 0))
                {
                    sb.AppendLine($"        OnEffectApply += AuraEffectApplyFn({auraClassName}::OnApply, {GetEffectIndexToken(auraEffect.EffectIndex)}, {GetAuraTypeToken(auraEffect.EffectAura)}, AURA_EFFECT_HANDLE_REAL);");
                    sb.AppendLine($"        OnEffectRemove += AuraEffectRemoveFn({auraClassName}::OnRemove, {GetEffectIndexToken(auraEffect.EffectIndex)}, {GetAuraTypeToken(auraEffect.EffectAura)}, AURA_EFFECT_HANDLE_REAL);");
                }
                sb.AppendLine("    }");
                sb.AppendLine("};");
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("void AddSC_custom_spell_scripts()");
            sb.AppendLine("{");
            sb.AppendLine($"    RegisterSpellScript({className}); // {scriptName}");
            if (hasAuraEffects)
                sb.AppendLine($"    RegisterAuraScript({className}_aura);");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string BuildSpellScriptTemplateV8(SpellInfo spell, string gameVersion)
        {
            var scriptToken = BuildScriptToken(spell.Name);
            var loaderName = $"spell_custom_{scriptToken}";
            var spellScriptName = $"{loaderName}_SpellScript";
            var auraScriptName = $"{loaderName}_AuraScript";

            var effects = spell.SpellEffectInfoStore
                .Where(e => e.SpellEffect != null)
                .OrderBy(e => e.EffectIndex)
                .ToList();

            var triggeredSpellIds = effects
                .Select(e => e.EffectTriggerSpell)
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            var hasAuraEffects = effects.Any(e => e.EffectAura > 0);
            var hasSpellEffects = effects.Count > 0 && effects.Any(e => e.EffectAura == 0);
            var sb = new StringBuilder();

            var versionLabel = gameVersion == "7.x" ? "7.x" : "8.x";
            sb.AppendLine($"// Example generated from SpellWork for TrinityCore/AzeriteCore {versionLabel} branch.");
            sb.AppendLine("// Review targets, effect types and guard checks before committing.");
            sb.AppendLine($"// {spell.ID} - {spell.NameAndSubname}");
            sb.AppendLine();

            // --- SpellScriptLoader wrapper ---
            sb.AppendLine($"class {loaderName} : public SpellScriptLoader");
            sb.AppendLine("{");
            sb.AppendLine("    public:");
            sb.AppendLine($"        {loaderName}() : SpellScriptLoader(\"{loaderName}\") {{ }}");
            sb.AppendLine();

            // --- Inner SpellScript ---
            if (hasSpellEffects || effects.Count == 0)
            {
                sb.AppendLine($"        class {spellScriptName} : public SpellScript");
                sb.AppendLine("        {");
                sb.AppendLine($"            PrepareSpellScript({spellScriptName});");
                sb.AppendLine();
                sb.AppendLine("            bool Load() override");
                sb.AppendLine("            {");
                sb.AppendLine("                return GetCaster() != nullptr;");
                sb.AppendLine("            }");
                sb.AppendLine();

                if (triggeredSpellIds.Count > 0)
                {
                    sb.AppendLine("            bool Validate(SpellInfo const* /*spellInfo*/) override");
                    sb.AppendLine("            {");
                    sb.AppendLine("                return ValidateSpellInfo(");
                    sb.AppendLine("                    {");
                    for (var i = 0; i < triggeredSpellIds.Count; i++)
                    {
                        var separator = i + 1 == triggeredSpellIds.Count ? string.Empty : ",";
                        sb.AppendLine($"                        {triggeredSpellIds[i]}{separator}");
                    }
                    sb.AppendLine("                    });");
                    sb.AppendLine("            }");
                    sb.AppendLine();
                }

                if (effects.Count == 0)
                {
                    sb.AppendLine("            void HandleDummy(SpellEffIndex /*effIndex*/)");
                    sb.AppendLine("            {");
                    sb.AppendLine("                // Spell has no explicit effects in loaded data; use this as fallback logic.");
                    sb.AppendLine("            }");
                    sb.AppendLine();
                    sb.AppendLine("            void Register() override");
                    sb.AppendLine("            {");
                    sb.AppendLine($"                OnEffectHitTarget += SpellEffectFn({spellScriptName}::HandleDummy, EFFECT_0, SPELL_EFFECT_DUMMY);");
                    sb.AppendLine("            }");
                }
                else
                {
                    foreach (var effect in effects.Where(e => e.EffectAura == 0))
                    {
                        var effectName = GetSpellEffectToken(effect.Effect);
                        var auraName = ((AuraType)effect.EffectAura).ToString();
                        var methodName = $"HandleEffect{effect.EffectIndex}";
                        var hook = GetSpellHookForEffect(effect.Effect);

                        sb.AppendLine($"            // Effect {effect.EffectIndex}: {effectName}, Aura: {auraName}");
                        sb.AppendLine($"            void {methodName}(SpellEffIndex /*effIndex*/)");
                        sb.AppendLine("            {");
                        sb.AppendLine("                Unit* caster = GetCaster();");
                        sb.AppendLine("                if (!caster)");
                        sb.AppendLine("                    return;");
                        sb.AppendLine();
                        sb.AppendLine("                Unit* target = GetHitUnit();");
                        sb.AppendLine("                if (!target)");
                        sb.AppendLine("                    return;");
                        sb.AppendLine();

                        if (effect.EffectTriggerSpell > 0)
                        {
                            sb.AppendLine($"                // Example trigger from dbc: {effect.EffectTriggerSpell}");
                            sb.AppendLine($"                caster->CastSpell(target, {effect.EffectTriggerSpell}, true);");
                        }
                        else
                        {
                            sb.AppendLine("                // Example custom logic:");
                            sb.AppendLine("                // caster->CastSpell(target, <spellId>, true);");
                            sb.AppendLine("                // target->RemoveAurasDueToSpell(<spellId>);");
                            sb.AppendLine("                // SetHitDamage(CalculatePct(GetHitDamage(), <percent>));");
                        }

                        sb.AppendLine("            }");
                        sb.AppendLine();
                    }

                    sb.AppendLine("            SpellCastResult CheckCast()");
                    sb.AppendLine("            {");
                    sb.AppendLine("                Unit* caster = GetCaster();");
                    sb.AppendLine("                if (!caster)");
                    sb.AppendLine("                    return SPELL_FAILED_DONT_REPORT;");
                    sb.AppendLine();
                    sb.AppendLine("                // Example condition:");
                    sb.AppendLine("                // if (!caster->IsInCombat())");
                    sb.AppendLine("                //     return SPELL_FAILED_CASTER_AURASTATE;");
                    sb.AppendLine();
                    sb.AppendLine("                return SPELL_CAST_OK;");
                    sb.AppendLine("            }");
                    sb.AppendLine();

                    sb.AppendLine("            void Register() override");
                    sb.AppendLine("            {");
                    sb.AppendLine($"                OnCheckCast += SpellCheckCastFn({spellScriptName}::CheckCast);");
                    foreach (var effect in effects.Where(e => e.EffectAura == 0))
                    {
                        var effectName = GetSpellEffectToken(effect.Effect);
                        var hook = GetSpellHookForEffect(effect.Effect);
                        sb.AppendLine($"                {hook} += SpellEffectFn({spellScriptName}::HandleEffect{effect.EffectIndex}, {GetEffectIndexToken(effect.EffectIndex)}, {effectName});");
                    }
                    sb.AppendLine("            }");
                }

                sb.AppendLine("        };");
                sb.AppendLine();
                sb.AppendLine("        SpellScript* GetSpellScript() const override");
                sb.AppendLine("        {");
                sb.AppendLine($"            return new {spellScriptName}();");
                sb.AppendLine("        }");
                sb.AppendLine();
            }

            // --- Inner AuraScript ---
            if (hasAuraEffects)
            {
                sb.AppendLine($"        class {auraScriptName} : public AuraScript");
                sb.AppendLine("        {");
                sb.AppendLine($"            PrepareAuraScript({auraScriptName});");
                sb.AppendLine();
                sb.AppendLine("            bool Validate(SpellInfo const* /*spellInfo*/) override");
                sb.AppendLine("            {");
                sb.AppendLine($"                return ValidateSpellInfo({{{spell.ID}}});");
                sb.AppendLine("            }");
                sb.AppendLine();
                sb.AppendLine("            void OnApply(AuraEffect const* /*aurEff*/, AuraEffectHandleModes /*mode*/)");
                sb.AppendLine("            {");
                sb.AppendLine("                Unit* target = GetTarget();");
                sb.AppendLine("                if (!target)");
                sb.AppendLine("                    return;");
                sb.AppendLine();
                sb.AppendLine("                // Example apply logic for aura effects.");
                sb.AppendLine("            }");
                sb.AppendLine();
                sb.AppendLine("            void OnRemove(AuraEffect const* /*aurEff*/, AuraEffectHandleModes /*mode*/)");
                sb.AppendLine("            {");
                sb.AppendLine("                // Example cleanup logic.");
                sb.AppendLine("            }");
                sb.AppendLine();
                sb.AppendLine("            void Register() override");
                sb.AppendLine("            {");
                foreach (var auraEffect in effects.Where(e => e.EffectAura > 0))
                {
                    sb.AppendLine($"                OnEffectApply += AuraEffectApplyFn({auraScriptName}::OnApply, {GetEffectIndexToken(auraEffect.EffectIndex)}, {GetAuraTypeToken(auraEffect.EffectAura)}, AURA_EFFECT_HANDLE_REAL);");
                    sb.AppendLine($"                OnEffectRemove += AuraEffectRemoveFn({auraScriptName}::OnRemove, {GetEffectIndexToken(auraEffect.EffectIndex)}, {GetAuraTypeToken(auraEffect.EffectAura)}, AURA_EFFECT_HANDLE_REAL);");
                }
                sb.AppendLine("            }");
                sb.AppendLine("        };");
                sb.AppendLine();
                sb.AppendLine("        AuraScript* GetAuraScript() const override");
                sb.AppendLine("        {");
                sb.AppendLine($"            return new {auraScriptName}();");
                sb.AppendLine("        }");
                sb.AppendLine();
            }

            // --- Close loader class ---
            sb.AppendLine("};");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("void AddSC_custom_spell_scripts()");
            sb.AppendLine("{");
            sb.AppendLine($"    new {loaderName}();");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string BuildScriptToken(string name)
        {
            var token = name.ToLowerInvariant();
            var chars = token.Select(ch => char.IsLetterOrDigit(ch) ? ch : '_').ToArray();
            token = new string(chars);

            while (token.Contains("__"))
                token = token.Replace("__", "_");

            token = token.Trim('_');
            return string.IsNullOrEmpty(token) ? "generated" : token;
        }

        private static string GetEffectIndexToken(int effectIndex)
        {
            return effectIndex switch
            {
                0 => "EFFECT_0",
                1 => "EFFECT_1",
                2 => "EFFECT_2",
                _ => $"(SpellEffIndex){effectIndex}"
            };
        }

        private static string GetSpellEffectToken(int effect)
        {
            if (Enum.IsDefined(typeof(SpellEffects), effect))
                return ((SpellEffects)effect).ToString();

            return $"(SpellEffects){effect}";
        }

        private static string GetAuraTypeToken(int auraType)
        {
            if (Enum.IsDefined(typeof(AuraType), auraType))
                return ((AuraType)auraType).ToString();

            return $"(AuraType){auraType}";
        }

        private static string GetSpellHookForEffect(int effect)
        {
            if (!Enum.IsDefined(typeof(SpellEffects), effect))
                return "OnEffectHitTarget";

            switch ((SpellEffects)effect)
            {
                case SpellEffects.SPELL_EFFECT_APPLY_AURA:
                case SpellEffects.SPELL_EFFECT_APPLY_AREA_AURA_PARTY:
                case SpellEffects.SPELL_EFFECT_APPLY_AREA_AURA_RAID:
                case SpellEffects.SPELL_EFFECT_APPLY_AREA_AURA_ENEMY:
                case SpellEffects.SPELL_EFFECT_APPLY_AREA_AURA_PET:
                case SpellEffects.SPELL_EFFECT_APPLY_AREA_AURA_FRIEND:
                    return "OnEffectHitTarget";
                case SpellEffects.SPELL_EFFECT_TRIGGER_SPELL:
                case SpellEffects.SPELL_EFFECT_TRIGGER_MISSILE:
                case SpellEffects.SPELL_EFFECT_TRIGGER_SPELL_WITH_VALUE:
                    return "OnEffectHitTarget";
                default:
                    return "OnEffectHitTarget";
            }
        }

        #endregion

        #region SPELL INFO PAGE

        private void LvSpellListSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lvSpellList.SelectedIndices.Count > 0)
                _spellList[_lvSpellList.SelectedIndices[0]].Write(_rtSpellInfo);
        }

        private void TbSearchIdKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                AdvancedSearch();
        }

        private void BSearchClick(object sender, EventArgs e)
        {
            AdvancedSearch();
        }

        private void CbSpellFamilyNamesSelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex != 0)
                AdvancedFilter();
        }

        private void TbAdvansedFilterValKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                AdvancedFilter();
        }

        private void AdvancedSearch()
        {
            var name = _tbSearchId.Text;
            var id = name.ToUInt32();
            var ic = _tbSearchIcon.Text.ToUInt32();
            var at = _tbSearchAttributes.Text.ToUInt32();

            _spellList = (from spellInfo in DBC.DBC.SpellInfoStore.Values
                          where
                              ((id == 0 || spellInfo.ID == id) && (ic == 0 || spellInfo.SpellIconFileDataID == ic) &&
                               (at == 0 || (spellInfo.Attributes & at) != 0 || (spellInfo.AttributesEx & at) != 0 ||
                                (spellInfo.AttributesEx2 & at) != 0 || (spellInfo.AttributesEx3 & at) != 0 ||
                                (spellInfo.AttributesEx4 & at) != 0 || (spellInfo.AttributesEx5 & at) != 0 ||
                                (spellInfo.AttributesEx6 & at) != 0 || (spellInfo.AttributesEx7 & at) != 0 ||
                                (spellInfo.AttributesEx8 & at) != 0 || (spellInfo.AttributesEx9 & at) != 0 ||
                                (spellInfo.AttributesEx10 & at) != 0 || (spellInfo.AttributesEx11 & at) != 0 ||
                                (spellInfo.AttributesEx12 & at) != 0 || (spellInfo.AttributesEx13 & at) != 0 ||
                                (spellInfo.AttributesEx14 & at) != 0)) && ((id != 0 || ic != 0 && at != 0) ||
                                spellInfo.Name.ContainsText(name))
                          orderby spellInfo.ID
                          select spellInfo).ToList();

            _lvSpellList.VirtualListSize = _spellList.Count;
            if (_lvSpellList.SelectedIndices.Count > 0)
                _lvSpellList.Items[_lvSpellList.SelectedIndices[0]].Selected = false;

            _lvSpellList.Invalidate();
        }

        private void AdvancedFilter()
        {
            var bFamilyNames = _cbSpellFamilyName.SelectedIndex != 0;
            var fFamilyNames = _cbSpellFamilyName.SelectedValue.ToInt32();

            var bSpellAura = _cbSpellAura.SelectedIndex != 0;
            var fSpellAura = _cbSpellAura.SelectedValue.ToInt32();

            var bSpellEffect = _cbSpellEffect.SelectedIndex != 0;
            var fSpellEffect = _cbSpellEffect.SelectedValue.ToInt32();

            var bTarget1 = _cbTarget1.SelectedIndex != 0;
            var fTarget1 = _cbTarget1.SelectedValue.ToInt32();

            var bTarget2 = _cbTarget2.SelectedIndex != 0;
            var fTarget2 = _cbTarget2.SelectedValue.ToInt32();

            // additional spell effect filters
            var advEffectVal1 = _tbAdvancedEffectFilter1Val.Text;
            var advEffectVal2 = _tbAdvancedEffectFilter2Val.Text;

            var fieldEffect1 = (MemberInfo)_cbAdvancedEffectFilter1.SelectedValue;
            var fieldEffect2 = (MemberInfo)_cbAdvancedEffectFilter2.SelectedValue;

            var use1EffectVal = !string.IsNullOrEmpty(advEffectVal1);
            var use2EffectVal = !string.IsNullOrEmpty(advEffectVal2);

            var fieldEffect1Ct = (CompareType)_cbAdvancedEffectFilter1CompareType.SelectedIndex;
            var fieldEffect2Ct = (CompareType)_cbAdvancedEffectFilter2CompareType.SelectedIndex;

            var filterValEffectFn1 = FilterFactory.CreateFilterFunc<SpellEffectInfo>(fieldEffect1, advEffectVal1, fieldEffect1Ct);
            var filterValEffectFn2 = FilterFactory.CreateFilterFunc<SpellEffectInfo>(fieldEffect2, advEffectVal2, fieldEffect2Ct);

            // additional filters
            var advVal1 = _tbAdvancedFilter1Val.Text;
            var advVal2 = _tbAdvancedFilter2Val.Text;

            var field1 = (MemberInfo)_cbAdvancedFilter1.SelectedValue;
            var field2 = (MemberInfo)_cbAdvancedFilter2.SelectedValue;

            var use1Val = !string.IsNullOrEmpty(advVal1);
            var use2Val = !string.IsNullOrEmpty(advVal2);

            var field1Ct = (CompareType)_cbAdvancedFilter1CompareType.SelectedIndex;
            var field2Ct = (CompareType)_cbAdvancedFilter2CompareType.SelectedIndex;

            var filterValFn1 = FilterFactory.CreateFilterFunc<SpellInfo>(field1, advVal1, field1Ct);
            var filterValFn2 = FilterFactory.CreateFilterFunc<SpellInfo>(field2, advVal2, field2Ct);

            _spellList = DBC.DBC.SpellInfoStore.Values.Where(
                spell => (!bFamilyNames || spell.SpellFamilyName == fFamilyNames) &&
                         (!bSpellEffect || spell.HasEffect((SpellEffects)fSpellEffect)) &&
                         (!bSpellAura || spell.HasAura((AuraType)fSpellAura)) &&
                         (!bTarget1 || spell.HasTargetA((Targets)fTarget1)) &&
                         (!bTarget2 || spell.HasTargetB((Targets)fTarget2)) &&
                         (!use1Val || filterValFn1(spell)) &&
                         (!use2Val || filterValFn2(spell)) &&
                         ((!use1EffectVal && !use2EffectVal) || spell.SpellEffectInfoStore.Any(effect =>
                         (!use1EffectVal || filterValEffectFn1(effect)) &&
                         (!use2EffectVal || filterValEffectFn2(effect)))))
                .OrderBy(spell => spell.ID)
                .ToList();

            _lvSpellList.VirtualListSize = _spellList.Count;
            if (_lvSpellList.SelectedIndices.Count > 0)
                _lvSpellList.Items[_lvSpellList.SelectedIndices[0]].Selected = false;

            _lvSpellList.Invalidate();
        }

        #endregion

        #region SPELL PROC INFO PAGE

        private void NewProcSpellIdClick(object sender, EventArgs e)
        {
            var spellId = int.Parse(_tbNewProcSpellId.Text);
            var spell = DBC.DBC.SpellInfoStore[spellId];
            var proc = new SpellProcEntry()
            {
                SpellId = spellId,
                SpellName = spell.NameAndSubname,
                SpellFamilyName = (SpellFamilyNames)spell.SpellFamilyName,
                SpellFamilyMask = new uint[4],
            };
            ProcParse(proc);
        }

        private void LoadProcFromDBClick(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
            _tbLoadProcSpellId.Text = _tbNewProcSpellId.Text;
        }

        private void NewProcSpellIdTextChanged(object sender, EventArgs e)
        {
            _bNewProcSpellId.Enabled = int.TryParse(((TextBox)sender).Text, out var spellId) && DBC.DBC.SpellInfoStore.ContainsKey(spellId);
        }

        private void CbProcSpellFamilyNameSelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex > 0)
                ProcFilter();
        }

        private void CbProcFlagCheckedChanged(object sender, EventArgs e)
        {
            splitContainer3.SplitterDistance = ((CheckBox)sender).Checked ? 300 : 200;
        }

        private void TvFamilyTreeAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level > 0)
                SetProcAttribute(DBC.DBC.SpellInfoStore[e.Node.Name.ToInt32()]);
        }

        private void LvProcSpellListSelectedIndexChanged(object sender, EventArgs e)
        {
            var lv = (ListView)sender;
            if (lv.SelectedIndices.Count <= 0)
                return;
            SetProcAttribute(_spellProcList[lv.SelectedIndices[0]]);
            _lvProcAdditionalInfo.Items.Clear();
        }

        private void LvProcAdditionalInfoSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lvProcAdditionalInfo.SelectedIndices.Count > 0)
                SetProcAttribute(DBC.DBC.SpellInfoStore[_lvProcAdditionalInfo.SelectedItems[0].SubItems[0].Text.ToInt32()]);
        }

        private void ClbSchoolsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProcInfo.SpellProc == null || ProcInfo.SpellProc.ID == 0)
                return;
            _bWrite.Enabled = true;
            GetProcAttribute(ProcInfo.SpellProc);
        }

        private void TbCooldownTextChanged(object sender, EventArgs e)
        {
            if (ProcInfo.SpellProc == null || ProcInfo.SpellProc.ID == 0)
                return;
            _bWrite.Enabled = true;
            GetProcAttribute(ProcInfo.SpellProc);
        }

        private void BProcSearchClick(object sender, EventArgs e)
        {
            Search();
        }

        private void TbSearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Search();
        }

        private void TvFamilyTreeSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedValue = ((ComboBox)sender).SelectedValue.ToInt32();
            if (selectedValue == -1)
                return;
            _tvFamilyTree.Nodes.Clear();

            // SPELLFAMILY_GENERIC proc records ignore SpellFamilyMask
            // so we don't populate TreeView for performance
            if (selectedValue == (int)SpellFamilyNames.SPELLFAMILY_GENERIC)
                return;

            ProcInfo.Fill(_tvFamilyTree, (SpellFamilyNames)selectedValue);
            PopulateProcAdditionalInfo();
        }

        private void SetProcAttribute(SpellInfo spell)
        {
            spell.Write(_rtbProcSpellInfo);
        }

        private void GetProcAttribute(SpellInfo spell)
        {
            var SpellFamilyFlags = _tvFamilyTree.GetMask();
            _lProcHeader.Text = $"Spell ({spell.ID}) {spell.NameAndSubname} ==> SpellFamily {_cbProcFitstSpellFamily.SelectedValue}, 0x{SpellFamilyFlags[0]:X8} {SpellFamilyFlags[1]:X8} {SpellFamilyFlags[2]:X8} {SpellFamilyFlags[3]:X8}";
        }

        private void Search()
        {
            var id = _tbProcSeach.Text.ToUInt32();

            _spellProcList = (from spell in DBC.DBC.SpellInfoStore.Values
                              where
                                  (id == 0 || spell.ID == id) &&
                                  (id != 0 || spell.Name.ContainsText(_tbProcSeach.Text))
                              select spell).ToList();

            _lvProcSpellList.VirtualListSize = _spellProcList.Count;
            if (_lvProcSpellList.SelectedIndices.Count > 0)
                _lvProcSpellList.Items[_lvProcSpellList.SelectedIndices[0]].Selected = false;
        }

        private void ProcFilter()
        {
            var bFamilyNames = _cbProcSpellFamilyName.SelectedIndex != 0;
            var fFamilyNames = _cbProcSpellFamilyName.SelectedValue.ToInt32();

            var bSpellAura = _cbProcSpellAura.SelectedIndex != 0;
            var fSpellAura = _cbProcSpellAura.SelectedValue.ToInt32();

            var bSpellEffect = _cbProcSpellEffect.SelectedIndex != 0;
            var fSpellEffect = _cbProcSpellEffect.SelectedValue.ToInt32();

            var bTarget1 = _cbProcTarget1.SelectedIndex != 0;
            var fTarget1 = _cbProcTarget1.SelectedValue.ToInt32();

            var bTarget2 = _cbProcTarget2.SelectedIndex != 0;
            var fTarget2 = _cbProcTarget2.SelectedValue.ToInt32();

            _spellProcList = (from spell in DBC.DBC.SpellInfoStore.Values
                              where
                                  (!bFamilyNames || spell.SpellFamilyName == fFamilyNames) &&
                                  (!bSpellEffect || spell.HasEffect((SpellEffects)fSpellEffect)) &&
                                  (!bSpellAura || spell.HasAura((AuraType)fSpellAura)) &&
                                  (!bTarget1 || spell.HasTargetA((Targets)fTarget1)) &&
                                  (!bTarget2 || spell.HasTargetB((Targets)fTarget2))
                              orderby spell.ID
                              select spell).ToList();

            _lvProcSpellList.VirtualListSize = _spellProcList.Count();
            if (_lvProcSpellList.SelectedIndices.Count > 0)
                _lvProcSpellList.Items[_lvProcSpellList.SelectedIndices[0]].Selected = false;
        }

        private void FamilyTreeAfterCheck(object sender, TreeViewEventArgs e)
        {
            if (ProcInfo.SpellProc == null || !ProcInfo.Update)
                return;

            _bWrite.Enabled = true;

            PopulateProcAdditionalInfo();

            GetProcAttribute(ProcInfo.SpellProc);
        }

        private void PopulateProcAdditionalInfo()
        {
            _lvProcAdditionalInfo.Items.Clear();
            _lvProcAdditionalInfo.Items.AddRange(
                _tvFamilyTree.Nodes.Cast<TreeNode>()
                    .Where(familyBitNode => familyBitNode.Checked)
                    .SelectMany(familyBitNode => familyBitNode.Nodes.Cast<TreeNode>())
                    .Distinct()
                    .Select(familySpellNode =>
                    {
                        var spell = DBC.DBC.SpellInfoStore[familySpellNode.Name.ToInt32()];

                        return new ListViewItem(new[] { familySpellNode.Name, spell.NameAndSubname, spell.Description })
                        {
                            ImageKey = familySpellNode.ImageKey
                        };
                    })
                    .OrderBy(listViewItem => listViewItem.SubItems[0].Text.ToInt32())
                    .ToArray());
        }

        #endregion

        #region COMPARE PAGE

        private void CompareFilterSpellTextChanged(object sender, EventArgs e)
        {
            var spell1 = _tbCompareFilterSpell1.Text.ToInt32();
            var spell2 = _tbCompareFilterSpell2.Text.ToInt32();

            if (DBC.DBC.SpellInfoStore.ContainsKey(spell1) && DBC.DBC.SpellInfoStore.ContainsKey(spell2))
                SpellCompare.Compare(_rtbCompareSpell1, _rtbCompareSpell2, DBC.DBC.SpellInfoStore[spell1], DBC.DBC.SpellInfoStore[spell2]);
        }

        private void CompareSearch1Click(object sender, EventArgs e)
        {
            var form = new FormSearch();
            form.ShowDialog(this);
            if (form.DialogResult == DialogResult.OK)
                _tbCompareFilterSpell1.Text = form.Spell.ID.ToString();
            form.Dispose();
        }

        private void CompareSearch2Click(object sender, EventArgs e)
        {
            var form = new FormSearch();
            form.ShowDialog(this);
            if (form.DialogResult == DialogResult.OK)
                _tbCompareFilterSpell2.Text = form.Spell.ID.ToString();
            form.Dispose();
        }

        #endregion

        #region SQL PAGE

        private void SqlDataListMouseDoubleClick(object sender, MouseEventArgs e)
        {
            ProcParse(sender);
        }

        private void SqlDataListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ProcParse(sender);
        }

        private void SqlToBaseClick(object sender, EventArgs e)
        {
            if (MySqlConnection.Connected)
                MySqlConnection.Insert(_rtbSqlLog.Text);
            else
                MessageBox.Show(@"Can't connect to database!", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SqlSaveClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_rtbSqlLog.Text))
                return;

            var sd = new SaveFileDialog { Filter = @"SQL files|*.sql" };
            if (sd.ShowDialog() == DialogResult.OK)
                using (var sw = new StreamWriter(sd.FileName, false, Encoding.UTF8))
                    sw.Write(_rtbSqlLog.Text);
        }

        private void CalcProcFlagsClick(object sender, EventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "_bSqlSchool":
                {
                    var val = _tbSqlSchool.Text.ToUInt32();
                    var form = new FormCalculateFlags(typeof(SpellSchools), val, string.Empty);
                    form.ShowDialog(this);
                    if (form.DialogResult == DialogResult.OK)
                        _tbSqlSchool.Text = form.Flags.ToString();
                    break;
                }
                case "_bSqlProc":
                {
                    var val = _tbSqlProc.Text.ToUInt32();
                    var form = new FormCalculateFlags(typeof(ProcFlags), val, "PROC_FLAG_");
                    form.ShowDialog(this);
                    if (form.DialogResult == DialogResult.OK)
                        _tbSqlProc.Text = form.Flags.ToString();
                    break;
                }
                case "_bSqlProcFlagsHit":
                {
                    var val = _tbSqlProcFlagsHit.Text.ToUInt32();
                    var form = new FormCalculateFlags(typeof(ProcFlagsHit), val, "PROC_HIT_");
                    form.ShowDialog(this);
                    if (form.DialogResult == DialogResult.OK)
                        _tbSqlProcFlagsHit.Text = form.Flags.ToString();
                    break;
                }
            }
        }

        private void SelectClick(object sender, EventArgs e)
        {
            if (!MySqlConnection.Connected)
            {
                MessageBox.Show(@"Can't connect to database!", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var compare = _cbBinaryCompare.Checked ? "&" : "=";

            var conditions = new List<string>();
            if (DBC.DBC.SpellInfoStore.ContainsKey(_tbLoadProcSpellId.Text.ToInt32()))
                conditions.Add($"SpellId = {_tbLoadProcSpellId.Text.ToInt32()}");

            if (_cbSqlSpellFamily.SelectedValue.ToInt32() != -1)
                conditions.Add($"SpellFamilyName = {_cbSqlSpellFamily.SelectedValue.ToInt32()}");

            if (_tbSqlSchool.Text.ToUInt32() != 0)
                conditions.Add($"SchoolMask {compare} {_tbSqlSchool.Text.ToUInt32()}");

            if (_tbSqlProc.Text.ToUInt32() != 0)
                conditions.Add($"ProcFlags {compare} {_tbSqlProc.Text.ToUInt32()}");

            if (_tbSqlProcFlagsHit.Text.ToUInt32() != 0)
                conditions.Add($"HitMask {compare} {_tbSqlProcFlagsHit.Text.ToUInt32()}");

            var subquery = "WHERE 1=1";
            if (conditions.Count > 0)
                subquery += conditions.Aggregate("", (sql, condition) => sql + " && " + condition);
            else if (!string.IsNullOrEmpty(_tbSqlManual.Text))
                subquery += " && " + _tbSqlManual.Text;

            var query = "SELECT SpellId, SchoolMask, SpellFamilyName, SpellFamilyMask0, SpellFamilyMask1, SpellFamilyMask2, SpellFamilyMask3, "
                        + $"ProcFlags, SpellTypeMask, SpellPhaseMask, HitMask, AttributesMask, DisableEffectsMask, ProcsPerMinute, Chance, Cooldown, Charges FROM `spell_proc` {subquery} ORDER BY SpellId";
            try
            {
                MySqlConnection.SelectProc(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            _lvDataList.VirtualListSize = MySqlConnection.SpellProcEvent.Count;
            if (_lvDataList.SelectedIndices.Count > 0)
                _lvDataList.Items[_lvDataList.SelectedIndices[0]].Selected = false;

            // check bad spell and drop
            foreach (var str in MySqlConnection.Dropped)
                _rtbSqlLog.AppendText(str);

            if (MySqlConnection.Dropped.Count != 0)
                _rtbSqlLog.AppendLine();

            _rtbSqlLog.ColorizeCode();
        }

        private void WriteClick(object sender, EventArgs e)
        {
            if (ProcInfo.SpellProc == null)
                return;

            var spellFamilyFlags = _tvFamilyTree.GetMask();

            // spell comment
            var comment = $" -- {ProcInfo.SpellProc.NameAndSubname}";

            // drop query
            var drop = $"DELETE FROM `spell_proc` WHERE `SpellId` IN ({ProcInfo.SpellProc.ID});";

            // insert query
            var procFlags = _clbProcFlags.GetFlagsValue(0) != ProcInfo.SpellProc.ProcFlags ? _clbProcFlags.GetFlagsValue(0) : 0;
            var procFlags2 = _clbProcFlags.GetFlagsValue(1) != ProcInfo.SpellProc.ProcFlagsEx ? _clbProcFlags.GetFlagsValue(1) : 0;
            var procPPM = _tbPPM.Text.Replace(',', '.') != ProcInfo.SpellProc.BaseProcRate.ToString(CultureInfo.InvariantCulture) ? _tbPPM.Text.Replace(',', '.') : "0";
            var procChance = _tbChance.Text.Replace(',', '.') != ProcInfo.SpellProc.ProcChance.ToString() ? _tbChance.Text.Replace(',', '.') : "0";
            var procCooldown = _tbCooldown.Text.ToUInt32() != ProcInfo.SpellProc.ProcCooldown ? _tbCooldown.Text.ToUInt32() : 0;
            var procCharges = _tbProcCharges.Text.ToInt32() != ProcInfo.SpellProc.ProcCharges ? _tbProcCharges.Text.ToInt32() : 0;

            var insert = "INSERT INTO `spell_proc` (`SpellId`,`SchoolMask`,`SpellFamilyName`,`SpellFamilyMask0`,`SpellFamilyMask1`,`SpellFamilyMask2`,`SpellFamilyMask3`,`ProcFlags`,`ProcFlags2`,`SpellTypeMask`,`SpellPhaseMask`,`HitMask`,`AttributesMask`,`DisableEffectsMask`,`ProcsPerMinute`,`Chance`,`Cooldown`,`Charges`) VALUES\r\n"
                + $"({ProcInfo.SpellProc.ID},0x{_clbSchools.GetFlagsValue():X2},"
                + $"{_cbProcFitstSpellFamily.SelectedValue.ToUInt32()},0x{spellFamilyFlags[0]:X8},0x{spellFamilyFlags[1]:X8},0x{spellFamilyFlags[2]:X8},0x{spellFamilyFlags[3]:X8},"
                + $"0x{procFlags:X},0x{procFlags2:X},0x{_clbSpellTypeMask.GetFlagsValue():X},0x{_clbSpellPhaseMask.GetFlagsValue():X},0x{_clbProcFlagHit.GetFlagsValue():X},0x{_clbProcAttributes.GetFlagsValue():X},0x0,{procPPM},{procChance},{procCooldown},{procCharges});";

            _rtbSqlLog.AppendText(drop + "\r\n" + insert + comment + "\r\n\r\n");
            _rtbSqlLog.ColorizeCode();
            if (MySqlConnection.Connected)
                MySqlConnection.Insert(drop + insert);

            ((Button)sender).Enabled = false;
        }

        private void ProcParse(object sender)
        {
            ProcParse(MySqlConnection.SpellProcEvent[((ListView)sender).SelectedIndices[0]]);
        }

        private void ProcParse(SpellProcEntry proc)
        {
            var spell = DBC.DBC.SpellInfoStore[Math.Abs(proc.SpellId)];
            ProcInfo.SpellProc = spell;

            spell.Write(_rtbProcSpellInfo);

            _tbNewProcSpellId.Text = proc.SpellId.ToString();

            _clbSchools.SetCheckedItemFromFlag((uint)proc.SchoolMask);
            _clbProcFlags.SetCheckedItemFromFlag((uint)proc.ProcFlags, 0);
            _clbProcFlags.SetCheckedItemFromFlag((uint)proc.ProcFlags2, 1);
            _clbSpellTypeMask.SetCheckedItemFromFlag((uint)proc.SpellTypeMask);
            _clbSpellPhaseMask.SetCheckedItemFromFlag((uint)proc.SpellPhaseMask);
            _clbProcFlagHit.SetCheckedItemFromFlag((uint)proc.HitMask);
            _clbProcAttributes.SetCheckedItemFromFlag((uint)proc.AttributesMask);

            _cbProcSpellFamilyTree.SelectedValue = (uint)proc.SpellFamilyName;
            _cbProcFitstSpellFamily.SelectedValue = (uint)proc.SpellFamilyName;

            _tbPPM.Text = proc.ProcsPerMinute.ToString(CultureInfo.InvariantCulture);
            _tbChance.Text = proc.Chance.ToString(CultureInfo.InvariantCulture);
            _tbCooldown.Text = proc.Cooldown.ToString();
            _tbProcCharges.Text = proc.Charges.ToString();

            _tvFamilyTree.SetMask(proc.SpellFamilyMask);
            PopulateProcAdditionalInfo();

            tabControl1.SelectedIndex = 1;
        }

        #endregion

        #region VIRTUAL MODE

        private List<SpellInfo> _spellList = new List<SpellInfo>();

        private List<SpellInfo> _spellProcList = new List<SpellInfo>();

        private void LvSpellListRetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item =
                new ListViewItem(new[] { _spellList[e.ItemIndex].ID.ToString(), _spellList[e.ItemIndex].NameAndSubname, (_spellList[e.ItemIndex].Misc?.ID ?? 0).ToString() });
        }

        private void LvProcSpellListRetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item =
                new ListViewItem(new[] { _spellProcList[e.ItemIndex].ID.ToString(), _spellProcList[e.ItemIndex].NameAndSubname });
        }

        private void LvSqlDataRetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem(MySqlConnection.SpellProcEvent[e.ItemIndex].ToArray());
        }

        #endregion

        public void Unblock()
        {
            tabControl1.Enabled = true;
            _bLevelScaling.Enabled = true;
            _bSpellScript.Enabled = true;
            _bCopySpellInfo.Enabled = true;
        }

        public void SetLoadingProgress(int progress)
        {
            loadingProgressBar1.Value = progress;
            if (progress == 100)
            {
                loadingProgressBar1.Enabled = false;
                loadingProgressBar1.Visible = false;
                loadingProgressLabel1.Visible = false;
            }
        }
    }
}
