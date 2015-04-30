namespace KaniReg
{
	partial class ResultViewer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultViewer));
			this.btnClose = new System.Windows.Forms.Button();
			this.rtContents = new System.Windows.Forms.RichTextBox();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiSearch = new System.Windows.Forms.ToolStripMenuItem();
			this.lvList = new System.Windows.Forms.ListView();
			this.colNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.btnColor = new System.Windows.Forms.Button();
			this.btnMark = new System.Windows.Forms.Button();
			this.pnlColor = new System.Windows.Forms.Panel();
			this.btnSave = new System.Windows.Forms.Button();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.btnDeleteSelect = new System.Windows.Forms.Button();
			this.btnDeleteAll = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnGetSearchText = new System.Windows.Forms.Button();
			this.btnSearch = new System.Windows.Forms.Button();
			this.txtSearch = new System.Windows.Forms.TextBox();
			this.contextMenuStrip.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.btnClose.Location = new System.Drawing.Point(696, 624);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(60, 23);
			this.btnClose.TabIndex = 4;
			this.btnClose.Text = "閉じる";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// rtContents
			// 
			this.rtContents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtContents.AutoWordSelection = true;
			this.rtContents.ContextMenuStrip = this.contextMenuStrip;
			this.rtContents.DetectUrls = false;
			this.rtContents.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.rtContents.HideSelection = false;
			this.rtContents.Location = new System.Drawing.Point(0, 0);
			this.rtContents.Name = "rtContents";
			this.rtContents.Size = new System.Drawing.Size(744, 343);
			this.rtContents.TabIndex = 0;
			this.rtContents.Text = "";
			this.rtContents.WordWrap = false;
			this.rtContents.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtContents_KeyDown);
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSearch});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(101, 26);
			// 
			// tsmiSearch
			// 
			this.tsmiSearch.Name = "tsmiSearch";
			this.tsmiSearch.Size = new System.Drawing.Size(100, 22);
			this.tsmiSearch.Text = "検索";
			this.tsmiSearch.Click += new System.EventHandler(this.tsmiSearch_Click);
			// 
			// lvList
			// 
			this.lvList.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.lvList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lvList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colNo,
            this.colPath});
			this.lvList.FullRowSelect = true;
			this.lvList.GridLines = true;
			this.lvList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvList.HideSelection = false;
			this.lvList.LabelWrap = false;
			this.lvList.Location = new System.Drawing.Point(0, 0);
			this.lvList.MultiSelect = false;
			this.lvList.Name = "lvList";
			this.lvList.ShowGroups = false;
			this.lvList.Size = new System.Drawing.Size(744, 200);
			this.lvList.TabIndex = 0;
			this.lvList.UseCompatibleStateImageBehavior = false;
			this.lvList.View = System.Windows.Forms.View.Details;
			this.lvList.SelectedIndexChanged += new System.EventHandler(this.lvList_SelectedIndexChanged);
			// 
			// colNo
			// 
			this.colNo.Text = "No";
			this.colNo.Width = 40;
			// 
			// colPath
			// 
			this.colPath.Text = "パースファイルパス";
			this.colPath.Width = 680;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(12, 12);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.lvList);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.rtContents);
			this.splitContainer1.Size = new System.Drawing.Size(744, 549);
			this.splitContainer1.SplitterDistance = 200;
			this.splitContainer1.SplitterWidth = 6;
			this.splitContainer1.TabIndex = 0;
			// 
			// colorDialog
			// 
			this.colorDialog.AnyColor = true;
			this.colorDialog.Color = System.Drawing.Color.Yellow;
			// 
			// btnColor
			// 
			this.btnColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnColor.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.btnColor.Location = new System.Drawing.Point(43, 25);
			this.btnColor.Name = "btnColor";
			this.btnColor.Size = new System.Drawing.Size(60, 23);
			this.btnColor.TabIndex = 1;
			this.btnColor.Text = "色選択";
			this.btnColor.UseVisualStyleBackColor = true;
			this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
			// 
			// btnMark
			// 
			this.btnMark.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnMark.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.btnMark.Location = new System.Drawing.Point(116, 25);
			this.btnMark.Name = "btnMark";
			this.btnMark.Size = new System.Drawing.Size(90, 23);
			this.btnMark.TabIndex = 2;
			this.btnMark.Text = "マーク (Ctrl+D)";
			this.btnMark.UseVisualStyleBackColor = true;
			this.btnMark.Click += new System.EventHandler(this.btnMark_Click);
			// 
			// pnlColor
			// 
			this.pnlColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.pnlColor.BackColor = System.Drawing.Color.Yellow;
			this.pnlColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlColor.Location = new System.Drawing.Point(13, 25);
			this.pnlColor.Name = "pnlColor";
			this.pnlColor.Size = new System.Drawing.Size(23, 23);
			this.pnlColor.TabIndex = 0;
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.btnSave.Location = new System.Drawing.Point(596, 567);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(160, 23);
			this.btnSave.TabIndex = 3;
			this.btnSave.Text = "マーク状態を保存 (Ctrl+S)";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "rtf";
			this.saveFileDialog.Filter = "リッチテキストファイル|*.rtf|全てのファイル|*.*";
			this.saveFileDialog.Title = "保存先";
			// 
			// btnDeleteSelect
			// 
			this.btnDeleteSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnDeleteSelect.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.btnDeleteSelect.Location = new System.Drawing.Point(13, 57);
			this.btnDeleteSelect.Name = "btnDeleteSelect";
			this.btnDeleteSelect.Size = new System.Drawing.Size(90, 23);
			this.btnDeleteSelect.TabIndex = 3;
			this.btnDeleteSelect.Text = "マーク選択解除";
			this.btnDeleteSelect.UseVisualStyleBackColor = true;
			this.btnDeleteSelect.Click += new System.EventHandler(this.btnDeleteSelect_Click);
			// 
			// btnDeleteAll
			// 
			this.btnDeleteAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnDeleteAll.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.btnDeleteAll.Location = new System.Drawing.Point(116, 57);
			this.btnDeleteAll.Name = "btnDeleteAll";
			this.btnDeleteAll.Size = new System.Drawing.Size(90, 23);
			this.btnDeleteAll.TabIndex = 4;
			this.btnDeleteAll.Text = "マーク全解除";
			this.btnDeleteAll.UseVisualStyleBackColor = true;
			this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.btnDeleteAll);
			this.groupBox1.Controls.Add(this.btnDeleteSelect);
			this.groupBox1.Controls.Add(this.pnlColor);
			this.groupBox1.Controls.Add(this.btnMark);
			this.groupBox1.Controls.Add(this.btnColor);
			this.groupBox1.Location = new System.Drawing.Point(12, 567);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(220, 90);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "マーク";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox2.Controls.Add(this.btnGetSearchText);
			this.groupBox2.Controls.Add(this.btnSearch);
			this.groupBox2.Controls.Add(this.txtSearch);
			this.groupBox2.Location = new System.Drawing.Point(260, 567);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(280, 90);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "検索 (Ctrl+F)";
			// 
			// btnGetSearchText
			// 
			this.btnGetSearchText.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnGetSearchText.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.btnGetSearchText.Location = new System.Drawing.Point(13, 57);
			this.btnGetSearchText.Name = "btnGetSearchText";
			this.btnGetSearchText.Size = new System.Drawing.Size(110, 23);
			this.btnGetSearchText.TabIndex = 1;
			this.btnGetSearchText.Text = "検索文字列取得";
			this.btnGetSearchText.UseVisualStyleBackColor = true;
			this.btnGetSearchText.Click += new System.EventHandler(this.btnGetSearchText_Click);
			// 
			// btnSearch
			// 
			this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnSearch.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.btnSearch.Location = new System.Drawing.Point(168, 57);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(100, 23);
			this.btnSearch.TabIndex = 2;
			this.btnSearch.Text = "次を検索 (F3)";
			this.btnSearch.UseVisualStyleBackColor = true;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// txtSearch
			// 
			this.txtSearch.Location = new System.Drawing.Point(13, 27);
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.Size = new System.Drawing.Size(255, 19);
			this.txtSearch.TabIndex = 0;
			this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
			this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
			// 
			// ResultViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(768, 662);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.btnClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(784, 700);
			this.Name = "ResultViewer";
			this.Text = "KaniReg - ResultViewer";
			this.Load += new System.EventHandler(this.ResultViewer_Load);
			this.contextMenuStrip.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.RichTextBox rtContents;
		private System.Windows.Forms.ListView lvList;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.Button btnColor;
		private System.Windows.Forms.Button btnMark;
		private System.Windows.Forms.Panel pnlColor;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.ColumnHeader colPath;
		private System.Windows.Forms.ColumnHeader colNo;
		private System.Windows.Forms.Button btnDeleteSelect;
		private System.Windows.Forms.Button btnDeleteAll;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem tsmiSearch;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.TextBox txtSearch;
		private System.Windows.Forms.Button btnGetSearchText;

	}
}