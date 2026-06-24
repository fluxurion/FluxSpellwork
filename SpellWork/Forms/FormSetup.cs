using SpellWork.Properties;
using System;
using System.IO;
using System.Windows.Forms;

namespace SpellWork.Forms
{
    public partial class FormSetup : Form
    {
        private int _step = 1;

        public FormSetup()
        {
            InitializeComponent();
        }

        private void FormSetup_Load(object sender, EventArgs e)
        {
            ShowStep(_step);

            // Pre-select previously saved values
            var saved = Settings.Default.GameVersion;
            if (!string.IsNullOrEmpty(saved))
            {
                foreach (RadioButton rb in _pnlVersions.Controls)
                    if (rb.Tag?.ToString() == saved)
                        rb.Checked = true;
            }
        }

        private void ShowStep(int step)
        {
            _pnlStep1.Visible = step == 1;
            _pnlStep2.Visible = step == 2;
            _lblStep.Text = $"Step {step} of 2";

            _btnBack.Enabled = step > 1;
            _btnNext.Text = step == 2 ? "Finish" : "Next >";
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (_step == 1)
            {
                string selected = null;
                foreach (RadioButton rb in _pnlVersions.Controls)
                    if (rb.Checked) { selected = rb.Tag?.ToString(); break; }

                if (selected == null)
                {
                    MessageBox.Show("Please select a game version.", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Settings.Default.GameVersion = selected;
                _step = 2;
                ShowStep(_step);

                // Prefill paths if already set
                _tbDb2Path.Text = Settings.Default.DbcPath;
                _tbLocale.Text = Settings.Default.Locale;
                _tbGtPath.Text = Settings.Default.GtPath;
            }
            else if (_step == 2)
            {
                var db2Path = _tbDb2Path.Text.Trim();
                var locale = _tbLocale.Text.Trim();

                if (string.IsNullOrEmpty(db2Path) || !Directory.Exists(db2Path))
                {
                    MessageBox.Show("Please select a valid DB2 folder path.", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(locale))
                {
                    MessageBox.Show("Please enter a locale (e.g. enUS).", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var localePath = Path.Combine(db2Path, locale);
                if (!Directory.Exists(localePath))
                {
                    var res = MessageBox.Show(
                        $"The path '{localePath}' does not exist.\n\nContinue anyway?",
                        "Setup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.No)
                        return;
                }

                var gtPath = _tbGtPath.Text.Trim();

                if (string.IsNullOrEmpty(gtPath))
                {
                    MessageBox.Show("Please enter a GT path (folder containing SpellScaling.txt).", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Settings.Default.DbcPath = db2Path;
                Settings.Default.Locale = locale;
                Settings.Default.GtPath = gtPath;
                Settings.Default.Save();

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            _step = 1;
            ShowStep(_step);
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog
            {
                Description = "Select the DB2 root folder (the folder that contains the locale subfolder)",
                ShowNewFolderButton = false
            };

            if (!string.IsNullOrEmpty(_tbDb2Path.Text) && Directory.Exists(_tbDb2Path.Text))
                dlg.SelectedPath = _tbDb2Path.Text;

            if (dlg.ShowDialog() == DialogResult.OK)
                _tbDb2Path.Text = dlg.SelectedPath;
        }

        private void BtnBrowseGt_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog
            {
                Description = "Select the GT folder (the folder containing SpellScaling.txt)",
                ShowNewFolderButton = false
            };

            if (!string.IsNullOrEmpty(_tbGtPath.Text) && Directory.Exists(_tbGtPath.Text))
                dlg.SelectedPath = _tbGtPath.Text;

            if (dlg.ShowDialog() == DialogResult.OK)
                _tbGtPath.Text = dlg.SelectedPath;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
