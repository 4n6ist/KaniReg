namespace KaniReg {
    partial class FormMain {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemOpenFile = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.versionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.labelHive = new System.Windows.Forms.Label();
			this.textBoxHive = new System.Windows.Forms.TextBox();
			this.btnBrowseFolder = new System.Windows.Forms.Button();
			this.labelPlugin = new System.Windows.Forms.Label();
			this.comboBoxPlugin = new System.Windows.Forms.ComboBox();
			this.buttonAnalyze = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.openFileDialogHive = new System.Windows.Forms.OpenFileDialog();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.saveFileDialogReport = new System.Windows.Forms.SaveFileDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.radioButtonUtc = new System.Windows.Forms.RadioButton();
			this.radioButtonDirect = new System.Windows.Forms.RadioButton();
			this.numericUpDownTimezone = new System.Windows.Forms.NumericUpDown();
			this.radioButtonLocal = new System.Windows.Forms.RadioButton();
			this.radioButtonDefault = new System.Windows.Forms.RadioButton();
			this.progressBarFile = new System.Windows.Forms.ProgressBar();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.btnBrowsFile = new System.Windows.Forms.Button();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.chkAutoSave = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.chkShowViewer = new System.Windows.Forms.CheckBox();
			this.progressBarPlugin = new System.Windows.Forms.ProgressBar();
			this.lblProgress = new KaniReg.TransparentLabel();
			this.menuStrip.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimezone)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(567, 26);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemOpenFile,
            this.menuItemOpenFolder,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(68, 22);
			this.fileToolStripMenuItem.Text = "ファイル";
			// 
			// menuItemOpenFile
			// 
			this.menuItemOpenFile.Name = "menuItemOpenFile";
			this.menuItemOpenFile.Size = new System.Drawing.Size(160, 22);
			this.menuItemOpenFile.Text = "ファイルを開く";
			this.menuItemOpenFile.Click += new System.EventHandler(this.menuItemOpenFile_Click);
			// 
			// menuItemOpenFolder
			// 
			this.menuItemOpenFolder.Name = "menuItemOpenFolder";
			this.menuItemOpenFolder.Size = new System.Drawing.Size(160, 22);
			this.menuItemOpenFolder.Text = "フォルダを開く";
			this.menuItemOpenFolder.Click += new System.EventHandler(this.menuItemOpenFolder_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.exitToolStripMenuItem.Text = "終了";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.versionToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(56, 22);
			this.helpToolStripMenuItem.Text = "ヘルプ";
			// 
			// versionToolStripMenuItem
			// 
			this.versionToolStripMenuItem.Name = "versionToolStripMenuItem";
			this.versionToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.versionToolStripMenuItem.Text = "バージョン情報";
			this.versionToolStripMenuItem.Click += new System.EventHandler(this.versionToolStripMenuItem_Click);
			// 
			// labelHive
			// 
			this.labelHive.AutoSize = true;
			this.labelHive.Location = new System.Drawing.Point(12, 44);
			this.labelHive.Name = "labelHive";
			this.labelHive.Size = new System.Drawing.Size(68, 12);
			this.labelHive.TabIndex = 1;
			this.labelHive.Text = "Hive ファイル:";
			// 
			// textBoxHive
			// 
			this.textBoxHive.AllowDrop = true;
			this.textBoxHive.Location = new System.Drawing.Point(103, 42);
			this.textBoxHive.Name = "textBoxHive";
			this.textBoxHive.Size = new System.Drawing.Size(321, 19);
			this.textBoxHive.TabIndex = 2;
			this.textBoxHive.TextChanged += new System.EventHandler(this.textBoxHive_TextChanged);
			this.textBoxHive.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxHive_DragDrop);
			this.textBoxHive.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxHive_DragEnter);
			// 
			// btnBrowseFolder
			// 
			this.btnBrowseFolder.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.btnBrowseFolder.Location = new System.Drawing.Point(430, 41);
			this.btnBrowseFolder.Name = "btnBrowseFolder";
			this.btnBrowseFolder.Size = new System.Drawing.Size(60, 21);
			this.btnBrowseFolder.TabIndex = 3;
			this.btnBrowseFolder.Text = "フォルダ...";
			this.btnBrowseFolder.UseVisualStyleBackColor = true;
			this.btnBrowseFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
			// 
			// labelPlugin
			// 
			this.labelPlugin.AutoSize = true;
			this.labelPlugin.Location = new System.Drawing.Point(12, 75);
			this.labelPlugin.Name = "labelPlugin";
			this.labelPlugin.Size = new System.Drawing.Size(88, 12);
			this.labelPlugin.TabIndex = 5;
			this.labelPlugin.Text = "Hiveファイル種別:";
			// 
			// comboBoxPlugin
			// 
			this.comboBoxPlugin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxPlugin.FormattingEnabled = true;
			this.comboBoxPlugin.Items.AddRange(new object[] {
            "ファイル名により判別",
            "ALL",
            "NTUSER",
            "SAM",
            "SECURITY",
            "SOFTWARE",
            "SYSTEM",
            "USRCLASS"});
			this.comboBoxPlugin.Location = new System.Drawing.Point(103, 72);
			this.comboBoxPlugin.Name = "comboBoxPlugin";
			this.comboBoxPlugin.Size = new System.Drawing.Size(153, 20);
			this.comboBoxPlugin.TabIndex = 6;
			// 
			// buttonAnalyze
			// 
			this.buttonAnalyze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAnalyze.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonAnalyze.Location = new System.Drawing.Point(12, 191);
			this.buttonAnalyze.Name = "buttonAnalyze";
			this.buttonAnalyze.Size = new System.Drawing.Size(60, 23);
			this.buttonAnalyze.TabIndex = 14;
			this.buttonAnalyze.Text = "実行";
			this.buttonAnalyze.UseVisualStyleBackColor = true;
			this.buttonAnalyze.Click += new System.EventHandler(this.buttonAnalyze_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonClose.Location = new System.Drawing.Point(496, 191);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(60, 23);
			this.buttonClose.TabIndex = 18;
			this.buttonClose.Text = "終了";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// openFileDialogHive
			// 
			this.openFileDialogHive.Filter = "全てのファイル|*.*|DATファイル|*.DAT";
			// 
			// backgroundWorker
			// 
			this.backgroundWorker.WorkerReportsProgress = true;
			this.backgroundWorker.WorkerSupportsCancellation = true;
			this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
			this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
			this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
			// 
			// saveFileDialogReport
			// 
			this.saveFileDialogReport.Filter = "テキストファイル|*.txt|全てのファイル|*.*";
			this.saveFileDialogReport.Title = "レポートファイルの保存先を指定してください。";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 138);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 12);
			this.label1.TabIndex = 10;
			this.label1.Text = "タイムゾーン指定:";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.radioButtonUtc);
			this.panel1.Controls.Add(this.radioButtonDirect);
			this.panel1.Controls.Add(this.numericUpDownTimezone);
			this.panel1.Controls.Add(this.radioButtonLocal);
			this.panel1.Location = new System.Drawing.Point(103, 132);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(415, 27);
			this.panel1.TabIndex = 11;
			// 
			// radioButtonUtc
			// 
			this.radioButtonUtc.AutoSize = true;
			this.radioButtonUtc.Location = new System.Drawing.Point(128, 5);
			this.radioButtonUtc.Name = "radioButtonUtc";
			this.radioButtonUtc.Size = new System.Drawing.Size(46, 16);
			this.radioButtonUtc.TabIndex = 1;
			this.radioButtonUtc.TabStop = true;
			this.radioButtonUtc.Text = "UTC";
			this.radioButtonUtc.UseVisualStyleBackColor = true;
			// 
			// radioButtonDirect
			// 
			this.radioButtonDirect.AutoSize = true;
			this.radioButtonDirect.Enabled = false;
			this.radioButtonDirect.Location = new System.Drawing.Point(278, 5);
			this.radioButtonDirect.Name = "radioButtonDirect";
			this.radioButtonDirect.Size = new System.Drawing.Size(71, 16);
			this.radioButtonDirect.TabIndex = 2;
			this.radioButtonDirect.TabStop = true;
			this.radioButtonDirect.Text = "個別指定";
			this.radioButtonDirect.UseVisualStyleBackColor = true;
			this.radioButtonDirect.Visible = false;
			this.radioButtonDirect.CheckedChanged += new System.EventHandler(this.radioButtonDirect_CheckedChanged);
			// 
			// numericUpDownTimezone
			// 
			this.numericUpDownTimezone.Enabled = false;
			this.numericUpDownTimezone.Location = new System.Drawing.Point(355, 4);
			this.numericUpDownTimezone.Maximum = new decimal(new int[] {
            14,
            0,
            0,
            0});
			this.numericUpDownTimezone.Minimum = new decimal(new int[] {
            12,
            0,
            0,
            -2147483648});
			this.numericUpDownTimezone.Name = "numericUpDownTimezone";
			this.numericUpDownTimezone.Size = new System.Drawing.Size(46, 19);
			this.numericUpDownTimezone.TabIndex = 3;
			this.numericUpDownTimezone.Visible = false;
			// 
			// radioButtonLocal
			// 
			this.radioButtonLocal.AutoSize = true;
			this.radioButtonLocal.Checked = true;
			this.radioButtonLocal.Location = new System.Drawing.Point(3, 5);
			this.radioButtonLocal.Name = "radioButtonLocal";
			this.radioButtonLocal.Size = new System.Drawing.Size(113, 16);
			this.radioButtonLocal.TabIndex = 0;
			this.radioButtonLocal.TabStop = true;
			this.radioButtonLocal.Text = "現在のタイムゾーン";
			this.radioButtonLocal.UseVisualStyleBackColor = true;
			// 
			// radioButtonDefault
			// 
			this.radioButtonDefault.AutoSize = true;
			this.radioButtonDefault.Checked = true;
			this.radioButtonDefault.Enabled = false;
			this.radioButtonDefault.Location = new System.Drawing.Point(524, 136);
			this.radioButtonDefault.Name = "radioButtonDefault";
			this.radioButtonDefault.Size = new System.Drawing.Size(79, 16);
			this.radioButtonDefault.TabIndex = 12;
			this.radioButtonDefault.TabStop = true;
			this.radioButtonDefault.Text = "タイムゾーン";
			this.radioButtonDefault.UseVisualStyleBackColor = true;
			this.radioButtonDefault.Visible = false;
			// 
			// progressBarFile
			// 
			this.progressBarFile.AccessibleDescription = "(double)";
			this.progressBarFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBarFile.Location = new System.Drawing.Point(139, 192);
			this.progressBarFile.Name = "progressBarFile";
			this.progressBarFile.Size = new System.Drawing.Size(351, 10);
			this.progressBarFile.TabIndex = 16;
			this.progressBarFile.Visible = false;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCancel.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonCancel.Location = new System.Drawing.Point(74, 191);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(60, 23);
			this.buttonCancel.TabIndex = 15;
			this.buttonCancel.Text = "中止";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Visible = false;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// btnBrowsFile
			// 
			this.btnBrowsFile.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.btnBrowsFile.Location = new System.Drawing.Point(496, 41);
			this.btnBrowsFile.Name = "btnBrowsFile";
			this.btnBrowsFile.Size = new System.Drawing.Size(60, 21);
			this.btnBrowsFile.TabIndex = 4;
			this.btnBrowsFile.Text = "ファイル...";
			this.btnBrowsFile.UseVisualStyleBackColor = true;
			this.btnBrowsFile.Click += new System.EventHandler(this.btnBrowsFile_Click);
			// 
			// folderBrowserDialog
			// 
			this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
			this.folderBrowserDialog.ShowNewFolderButton = false;
			// 
			// chkAutoSave
			// 
			this.chkAutoSave.AutoSize = true;
			this.chkAutoSave.Location = new System.Drawing.Point(106, 106);
			this.chkAutoSave.Name = "chkAutoSave";
			this.chkAutoSave.Size = new System.Drawing.Size(160, 16);
			this.chkAutoSave.TabIndex = 8;
			this.chkAutoSave.Text = "出力ファイルを自動的に保存";
			this.chkAutoSave.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 107);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(74, 12);
			this.label2.TabIndex = 7;
			this.label2.Text = "出力オプション:";
			// 
			// chkShowViewer
			// 
			this.chkShowViewer.AutoSize = true;
			this.chkShowViewer.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.chkShowViewer.Location = new System.Drawing.Point(274, 106);
			this.chkShowViewer.Name = "chkShowViewer";
			this.chkShowViewer.Size = new System.Drawing.Size(150, 16);
			this.chkShowViewer.TabIndex = 9;
			this.chkShowViewer.Text = "変換結果をビューアで表示";
			this.chkShowViewer.UseVisualStyleBackColor = true;
			// 
			// progressBarPlugin
			// 
			this.progressBarPlugin.AccessibleDescription = "(double)";
			this.progressBarPlugin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBarPlugin.Location = new System.Drawing.Point(139, 202);
			this.progressBarPlugin.Name = "progressBarPlugin";
			this.progressBarPlugin.Size = new System.Drawing.Size(351, 10);
			this.progressBarPlugin.TabIndex = 17;
			this.progressBarPlugin.Visible = false;
			// 
			// lblProgress
			// 
			this.lblProgress.BackColor = System.Drawing.Color.Transparent;
			this.lblProgress.Font = new System.Drawing.Font("MS UI Gothic", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.lblProgress.ForeColor = System.Drawing.Color.Black;
			this.lblProgress.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblProgress.Location = new System.Drawing.Point(12, 169);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new System.Drawing.Size(544, 12);
			this.lblProgress.TabIndex = 13;
			this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(567, 227);
			this.Controls.Add(this.radioButtonDefault);
			this.Controls.Add(this.chkAutoSave);
			this.Controls.Add(this.chkShowViewer);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnBrowsFile);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.buttonAnalyze);
			this.Controls.Add(this.comboBoxPlugin);
			this.Controls.Add(this.labelPlugin);
			this.Controls.Add(this.btnBrowseFolder);
			this.Controls.Add(this.textBoxHive);
			this.Controls.Add(this.labelHive);
			this.Controls.Add(this.menuStrip);
			this.Controls.Add(this.lblProgress);
			this.Controls.Add(this.progressBarFile);
			this.Controls.Add(this.progressBarPlugin);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Location = new System.Drawing.Point(200, 200);
			this.MainMenuStrip = this.menuStrip;
			this.MaximizeBox = false;
			this.Name = "FormMain";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "KaniReg";
			this.Load += new System.EventHandler(this.FormMain_Load);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimezone)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemOpenFile;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Label labelHive;
        private System.Windows.Forms.TextBox textBoxHive;
        private System.Windows.Forms.Button btnBrowseFolder;
        private System.Windows.Forms.Label labelPlugin;
        private System.Windows.Forms.ComboBox comboBoxPlugin;
        private System.Windows.Forms.Button buttonAnalyze;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.OpenFileDialog openFileDialogHive;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.SaveFileDialog saveFileDialogReport;
        private System.Windows.Forms.ToolStripMenuItem versionToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButtonLocal;
        private System.Windows.Forms.RadioButton radioButtonDirect;
		private System.Windows.Forms.NumericUpDown numericUpDownTimezone;
        private System.Windows.Forms.RadioButton radioButtonUtc;
        private System.Windows.Forms.RadioButton radioButtonDefault;
        private System.Windows.Forms.ProgressBar progressBarFile;
        private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button btnBrowsFile;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.ToolStripMenuItem menuItemOpenFolder;
		private System.Windows.Forms.CheckBox chkAutoSave;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox chkShowViewer;
		private System.Windows.Forms.ProgressBar progressBarPlugin;
		private TransparentLabel lblProgress;
    }
}

