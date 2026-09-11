namespace SpellWork.Forms
{
    partial class FormSetup
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            _lblTitle = new System.Windows.Forms.Label();
            _lblStep = new System.Windows.Forms.Label();
            _pnlStep1 = new System.Windows.Forms.Panel();
            _lblVersionPrompt = new System.Windows.Forms.Label();
            _pnlVersions = new System.Windows.Forms.Panel();
            _rb12x = new System.Windows.Forms.RadioButton();
            _rb10x = new System.Windows.Forms.RadioButton();
            _rb8x = new System.Windows.Forms.RadioButton();
            _rb7x = new System.Windows.Forms.RadioButton();
            _pnlStep2 = new System.Windows.Forms.Panel();
            _lblDb2Prompt = new System.Windows.Forms.Label();
            _lblDb2Path = new System.Windows.Forms.Label();
            _tbDb2Path = new System.Windows.Forms.TextBox();
            _btnBrowse = new System.Windows.Forms.Button();
            _lblLocale = new System.Windows.Forms.Label();
            _tbLocale = new System.Windows.Forms.TextBox();
            _lblLocaleHint = new System.Windows.Forms.Label();
            _lblGtPath = new System.Windows.Forms.Label();
            _tbGtPath = new System.Windows.Forms.TextBox();
            _btnBrowseGt = new System.Windows.Forms.Button();
            _lblGtHint = new System.Windows.Forms.Label();
            _pnlButtons = new System.Windows.Forms.Panel();
            _btnBack = new System.Windows.Forms.Button();
            _btnNext = new System.Windows.Forms.Button();
            _btnCancel = new System.Windows.Forms.Button();
            _pnlStep1.SuspendLayout();
            _pnlVersions.SuspendLayout();
            _pnlStep2.SuspendLayout();
            _pnlButtons.SuspendLayout();
            SuspendLayout();
            // 
            // _lblTitle
            // 
            _lblTitle.AutoSize = false;
            _lblTitle.BackColor = System.Drawing.SystemColors.ActiveCaption;
            _lblTitle.ForeColor = System.Drawing.Color.White;
            _lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            _lblTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            _lblTitle.Location = new System.Drawing.Point(0, 0);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Padding = new System.Windows.Forms.Padding(10, 8, 0, 8);
            _lblTitle.Size = new System.Drawing.Size(440, 48);
            _lblTitle.TabIndex = 0;
            _lblTitle.Text = "SpellWork Setup";
            // 
            // _lblStep
            // 
            _lblStep.AutoSize = true;
            _lblStep.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            _lblStep.ForeColor = System.Drawing.Color.Gray;
            _lblStep.Location = new System.Drawing.Point(12, 55);
            _lblStep.Name = "_lblStep";
            _lblStep.Size = new System.Drawing.Size(60, 13);
            _lblStep.TabIndex = 1;
            _lblStep.Text = "Step 1 of 2";
            // 
            // _pnlStep1
            // 
            _pnlStep1.Controls.Add(_lblVersionPrompt);
            _pnlStep1.Controls.Add(_pnlVersions);
            _pnlStep1.Location = new System.Drawing.Point(0, 72);
            _pnlStep1.Name = "_pnlStep1";
            _pnlStep1.Size = new System.Drawing.Size(440, 235);
            _pnlStep1.TabIndex = 2;
            // 
            // _lblVersionPrompt
            // 
            _lblVersionPrompt.AutoSize = true;
            _lblVersionPrompt.Font = new System.Drawing.Font("Segoe UI", 10F);
            _lblVersionPrompt.Location = new System.Drawing.Point(15, 10);
            _lblVersionPrompt.Name = "_lblVersionPrompt";
            _lblVersionPrompt.Size = new System.Drawing.Size(250, 19);
            _lblVersionPrompt.TabIndex = 0;
            _lblVersionPrompt.Text = "Select the game version to work with:";
            // 
            // _pnlVersions
            // 
            _pnlVersions.Controls.Add(_rb7x);
            _pnlVersions.Controls.Add(_rb8x);
            _pnlVersions.Controls.Add(_rb10x);
            _pnlVersions.Controls.Add(_rb12x);
            _pnlVersions.Location = new System.Drawing.Point(30, 40);
            _pnlVersions.Name = "_pnlVersions";
            _pnlVersions.Size = new System.Drawing.Size(380, 165);
            _pnlVersions.TabIndex = 1;
            // 
            // _rb7x
            // 
            _rb7x.AutoSize = true;
            _rb7x.Font = new System.Drawing.Font("Segoe UI", 10F);
            _rb7x.Location = new System.Drawing.Point(10, 10);
            _rb7x.Name = "_rb7x";
            _rb7x.Size = new System.Drawing.Size(200, 23);
            _rb7x.TabIndex = 0;
            _rb7x.Tag = "7.x";
            _rb7x.Text = "Legion (7.x)";
            _rb7x.UseVisualStyleBackColor = true;
            // 
            // _rb8x
            // 
            _rb8x.AutoSize = true;
            _rb8x.Font = new System.Drawing.Font("Segoe UI", 10F);
            _rb8x.Location = new System.Drawing.Point(10, 45);
            _rb8x.Name = "_rb8x";
            _rb8x.Size = new System.Drawing.Size(210, 23);
            _rb8x.TabIndex = 1;
            _rb8x.Tag = "8.x";
            _rb8x.Text = "Battle for Azeroth (8.x)";
            _rb8x.UseVisualStyleBackColor = true;
            // 
            // _rb10x
            // 
            _rb10x.AutoSize = true;
            _rb10x.Checked = true;
            _rb10x.Font = new System.Drawing.Font("Segoe UI", 10F);
            _rb10x.Location = new System.Drawing.Point(10, 80);
            _rb10x.Name = "_rb10x";
            _rb10x.Size = new System.Drawing.Size(225, 23);
            _rb10x.TabIndex = 2;
            _rb10x.Tag = "10.x";
            _rb10x.Text = "Dragonflight (10.x)";
            _rb10x.UseVisualStyleBackColor = true;
            // 
            // _rb12x
            // 
            _rb12x.AutoSize = true;
            _rb12x.Font = new System.Drawing.Font("Segoe UI", 10F);
            _rb12x.Location = new System.Drawing.Point(10, 115);
            _rb12x.Name = "_rb12x";
            _rb12x.Size = new System.Drawing.Size(235, 23);
            _rb12x.TabIndex = 3;
            _rb12x.Tag = "12.x";
            _rb12x.Text = "Midnight (12.x)";
            _rb12x.UseVisualStyleBackColor = true;
            // 
            // _pnlStep2
            // 
            _pnlStep2.Controls.Add(_lblDb2Prompt);
            _pnlStep2.Controls.Add(_lblDb2Path);
            _pnlStep2.Controls.Add(_tbDb2Path);
            _pnlStep2.Controls.Add(_btnBrowse);
            _pnlStep2.Controls.Add(_lblLocale);
            _pnlStep2.Controls.Add(_tbLocale);
            _pnlStep2.Controls.Add(_lblLocaleHint);
            _pnlStep2.Controls.Add(_lblGtPath);
            _pnlStep2.Controls.Add(_tbGtPath);
            _pnlStep2.Controls.Add(_btnBrowseGt);
            _pnlStep2.Controls.Add(_lblGtHint);
            _pnlStep2.Location = new System.Drawing.Point(0, 72);
            _pnlStep2.Name = "_pnlStep2";
            _pnlStep2.Size = new System.Drawing.Size(440, 240);
            _pnlStep2.TabIndex = 3;
            _pnlStep2.Visible = false;
            // 
            // _lblDb2Prompt
            // 
            _lblDb2Prompt.AutoSize = true;
            _lblDb2Prompt.Font = new System.Drawing.Font("Segoe UI", 10F);
            _lblDb2Prompt.Location = new System.Drawing.Point(15, 10);
            _lblDb2Prompt.Name = "_lblDb2Prompt";
            _lblDb2Prompt.Size = new System.Drawing.Size(200, 19);
            _lblDb2Prompt.TabIndex = 0;
            _lblDb2Prompt.Text = "Select the DB2 files location:";
            // 
            // _lblDb2Path
            // 
            _lblDb2Path.AutoSize = true;
            _lblDb2Path.Location = new System.Drawing.Point(15, 44);
            _lblDb2Path.Name = "_lblDb2Path";
            _lblDb2Path.Size = new System.Drawing.Size(57, 15);
            _lblDb2Path.TabIndex = 1;
            _lblDb2Path.Text = "DB2 Path:";
            // 
            // _tbDb2Path
            // 
            _tbDb2Path.Location = new System.Drawing.Point(80, 41);
            _tbDb2Path.Name = "_tbDb2Path";
            _tbDb2Path.Size = new System.Drawing.Size(265, 23);
            _tbDb2Path.TabIndex = 2;
            // 
            // _btnBrowse
            // 
            _btnBrowse.Location = new System.Drawing.Point(352, 41);
            _btnBrowse.Name = "_btnBrowse";
            _btnBrowse.Size = new System.Drawing.Size(75, 23);
            _btnBrowse.TabIndex = 3;
            _btnBrowse.Text = "Browse...";
            _btnBrowse.UseVisualStyleBackColor = true;
            _btnBrowse.Click += new System.EventHandler(BtnBrowse_Click);
            // 
            // _lblLocale
            // 
            _lblLocale.AutoSize = true;
            _lblLocale.Location = new System.Drawing.Point(15, 78);
            _lblLocale.Name = "_lblLocale";
            _lblLocale.Size = new System.Drawing.Size(42, 15);
            _lblLocale.TabIndex = 4;
            _lblLocale.Text = "Locale:";
            // 
            // _tbLocale
            // 
            _tbLocale.Location = new System.Drawing.Point(80, 75);
            _tbLocale.Name = "_tbLocale";
            _tbLocale.Size = new System.Drawing.Size(100, 23);
            _tbLocale.TabIndex = 5;
            _tbLocale.Text = "enUS";
            // 
            // _lblLocaleHint
            // 
            _lblLocaleHint.AutoSize = true;
            _lblLocaleHint.ForeColor = System.Drawing.Color.Gray;
            _lblLocaleHint.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            _lblLocaleHint.Location = new System.Drawing.Point(185, 78);
            _lblLocaleHint.Name = "_lblLocaleHint";
            _lblLocaleHint.Size = new System.Drawing.Size(180, 15);
            _lblLocaleHint.TabIndex = 6;
            _lblLocaleHint.Text = "(e.g. enUS — subfolder inside DB2 path)";
            // 
            // _lblGtPath
            // 
            _lblGtPath.AutoSize = true;
            _lblGtPath.Location = new System.Drawing.Point(15, 115);
            _lblGtPath.Name = "_lblGtPath";
            _lblGtPath.Size = new System.Drawing.Size(52, 15);
            _lblGtPath.TabIndex = 7;
            _lblGtPath.Text = "GT Path:";
            // 
            // _tbGtPath
            // 
            _tbGtPath.Location = new System.Drawing.Point(80, 112);
            _tbGtPath.Name = "_tbGtPath";
            _tbGtPath.Size = new System.Drawing.Size(265, 23);
            _tbGtPath.TabIndex = 8;
            // 
            // _btnBrowseGt
            // 
            _btnBrowseGt.Location = new System.Drawing.Point(352, 112);
            _btnBrowseGt.Name = "_btnBrowseGt";
            _btnBrowseGt.Size = new System.Drawing.Size(75, 23);
            _btnBrowseGt.TabIndex = 9;
            _btnBrowseGt.Text = "Browse...";
            _btnBrowseGt.UseVisualStyleBackColor = true;
            _btnBrowseGt.Click += new System.EventHandler(BtnBrowseGt_Click);
            // 
            // _lblGtHint
            // 
            _lblGtHint.AutoSize = true;
            _lblGtHint.ForeColor = System.Drawing.Color.Gray;
            _lblGtHint.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            _lblGtHint.Location = new System.Drawing.Point(15, 140);
            _lblGtHint.Name = "_lblGtHint";
            _lblGtHint.Size = new System.Drawing.Size(200, 15);
            _lblGtHint.TabIndex = 10;
            _lblGtHint.Text = "(folder containing SpellScaling.txt)";
            // 
            // _pnlButtons
            // 
            _pnlButtons.Controls.Add(_btnCancel);
            _pnlButtons.Controls.Add(_btnNext);
            _pnlButtons.Controls.Add(_btnBack);
            _pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            _pnlButtons.Location = new System.Drawing.Point(0, 324);
            _pnlButtons.Name = "_pnlButtons";
            _pnlButtons.Size = new System.Drawing.Size(440, 44);
            _pnlButtons.TabIndex = 4;
            // 
            // _btnBack
            // 
            _btnBack.Anchor = System.Windows.Forms.AnchorStyles.Right;
            _btnBack.Location = new System.Drawing.Point(248, 10);
            _btnBack.Name = "_btnBack";
            _btnBack.Size = new System.Drawing.Size(56, 25);
            _btnBack.TabIndex = 0;
            _btnBack.Text = "< Back";
            _btnBack.UseVisualStyleBackColor = true;
            _btnBack.Click += new System.EventHandler(BtnBack_Click);
            // 
            // _btnNext
            // 
            _btnNext.Anchor = System.Windows.Forms.AnchorStyles.Right;
            _btnNext.Location = new System.Drawing.Point(308, 10);
            _btnNext.Name = "_btnNext";
            _btnNext.Size = new System.Drawing.Size(60, 25);
            _btnNext.TabIndex = 1;
            _btnNext.Text = "Next >";
            _btnNext.UseVisualStyleBackColor = true;
            _btnNext.Click += new System.EventHandler(BtnNext_Click);
            // 
            // _btnCancel
            // 
            _btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            _btnCancel.Location = new System.Drawing.Point(374, 10);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new System.Drawing.Size(56, 25);
            _btnCancel.TabIndex = 2;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = true;
            _btnCancel.Click += new System.EventHandler(BtnCancel_Click);
            // 
            // FormSetup
            // 
            AcceptButton = _btnNext;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(440, 368);
            Controls.Add(_lblTitle);
            Controls.Add(_lblStep);
            Controls.Add(_pnlStep1);
            Controls.Add(_pnlStep2);
            Controls.Add(_pnlButtons);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormSetup";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "SpellWork Setup";
            Load += new System.EventHandler(FormSetup_Load);
            _pnlStep1.ResumeLayout(false);
            _pnlStep1.PerformLayout();
            _pnlVersions.ResumeLayout(false);
            _pnlVersions.PerformLayout();
            _pnlStep2.ResumeLayout(false);
            _pnlStep2.PerformLayout();
            _pnlButtons.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label _lblTitle;
        private System.Windows.Forms.Label _lblStep;
        private System.Windows.Forms.Panel _pnlStep1;
        private System.Windows.Forms.Label _lblVersionPrompt;
        private System.Windows.Forms.Panel _pnlVersions;
        private System.Windows.Forms.RadioButton _rb7x;
        private System.Windows.Forms.RadioButton _rb8x;
        private System.Windows.Forms.RadioButton _rb10x;
        private System.Windows.Forms.RadioButton _rb12x;
        private System.Windows.Forms.Panel _pnlStep2;
        private System.Windows.Forms.Label _lblDb2Prompt;
        private System.Windows.Forms.Label _lblDb2Path;
        private System.Windows.Forms.TextBox _tbDb2Path;
        private System.Windows.Forms.Button _btnBrowse;
        private System.Windows.Forms.Label _lblLocale;
        private System.Windows.Forms.TextBox _tbLocale;
        private System.Windows.Forms.Label _lblLocaleHint;
        private System.Windows.Forms.Label _lblGtPath;
        private System.Windows.Forms.TextBox _tbGtPath;
        private System.Windows.Forms.Button _btnBrowseGt;
        private System.Windows.Forms.Label _lblGtHint;
        private System.Windows.Forms.Panel _pnlButtons;
        private System.Windows.Forms.Button _btnBack;
        private System.Windows.Forms.Button _btnNext;
        private System.Windows.Forms.Button _btnCancel;
    }
}
