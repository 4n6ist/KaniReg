using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace KaniReg
{
	public partial class ResultViewer : Form
	{
		/// <summary>
		/// Result file list
		/// </summary>
		List<string> _list;

		/// <summary>
		/// Search start pos
		/// </summary>
		int _startPos = 0;

		/// <summary>
		/// Search form
		/// </summary>
		Search _searchDlg;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="list">Result file list</param>
		public ResultViewer(List<string> list)
		{
			InitializeComponent();

			this._list = list;
			this._searchDlg = new Search();
		}

		#region "Form"

		/// <summary>
		/// Form load action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ResultViewer_Load(object sender, EventArgs e)
		{
			// Set Result list to ListView
			this.lvList.BeginUpdate();

			for (int i = 0; i < this._list.Count; ++i)
			{
				string path = this._list[i];

				var item = new ListViewItem(new string[] { (i + 1).ToString(), path });

				this.lvList.Items.Add(item);
			}

			// Select First item
			this.lvList.Items[0].Selected = true;

			this.lvList.EndUpdate();
		}

		#endregion

		#region "ListView"

		/// <summary>
		/// Result file list selected index changed action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lvList_SelectedIndexChanged(object sender, EventArgs e)
		{
			// いったんCountが0になってもう1回戻ってくるのでゼロの時は無視する
			if (this.lvList.SelectedItems.Count > 0)
			{
				this._startPos = 0;
				this.rtContents.Text = "";

				string path = this.lvList.SelectedItems[0].SubItems[1].Text;

				if (File.Exists(path))
				{
					this.rtContents.Text = File.ReadAllText(path, Encoding.UTF8);
				}
			}
		}

		#endregion

		#region "Mark"

		/// <summary>
		/// Color select button click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnColor_Click(object sender, EventArgs e)
		{
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				this.pnlColor.BackColor = colorDialog.Color;
			}
		}

		/// <summary>
		/// Mark button click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnMark_Click(object sender, EventArgs e)
		{
			this.rtContents.SelectionBackColor = this.pnlColor.BackColor;
		}

		/// <summary>
		/// Selected mark delete button click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnDeleteSelect_Click(object sender, EventArgs e)
		{
			this.rtContents.SelectionBackColor = this.rtContents.BackColor;
		}

		/// <summary>
		/// All mark delete button click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnDeleteAll_Click(object sender, EventArgs e)
		{
			int pos = this.rtContents.SelectionStart; // Store current line pos
			this.rtContents.SelectAll();
			this.rtContents.SelectionBackColor = this.rtContents.BackColor;
			this.rtContents.SelectionLength = 0;	// Remove selection
			this.rtContents.Select();				// Select rtCongtents control
			this.rtContents.Select(pos, 0);			// Restore current line pos
			this.rtContents.ScrollToCaret();		// Show Current line
		}

		/// <summary>
		/// button click action on rtContents
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void rtContents_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyCode == Keys.D) && e.Control)
			{
				this.btnMark.PerformClick();
			}
			else if (e.KeyData == Keys.F3)
			{
				this.btnSearch.PerformClick();
			}
			else if ((e.KeyCode == Keys.F) && e.Control)
			{
				this.tsmiSearch.PerformClick();
			}
			else if ((e.KeyCode == Keys.S) && e.Control)
			{
				this.btnSave.PerformClick();
			}
		}

		/// <summary>
		/// Search strip menu click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsmiSearch_Click(object sender, EventArgs e)
		{
			this._searchDlg.SearchText = this.rtContents.SelectedText;
			this._searchDlg.ShowDialog(this);
		}

		#endregion

		#region "Search"

		/// <summary>
		/// Search button click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSearch_Click(object sender, EventArgs e)
		{
			// If txtSearch is empty, set rtContents.selectedText to txtSearch.Text.
			if ((this.rtContents.SelectedText.Length > 0) && (this.txtSearch.Text.Length == 0))
			{
				this.txtSearch.Text = this.rtContents.SelectedText;
			}

			string text = this.txtSearch.Text;

			if (text.Length > 0)
			{
				int pos = rtContents.Find(text, this._startPos, RichTextBoxFinds.None);

				this._startPos = pos + text.Length;
			}

			this._searchDlg.ManageHistory(text);
		}

		/// <summary>
		/// Key click action on txtSearch
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtSearch_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyData == Keys.F3) || (e.KeyData == Keys.Enter))
			{
				this.btnSearch.PerformClick();
			}
		}

		/// <summary>
		/// txtSearch text changed action
		/// Reset search start pos
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtSearch_TextChanged(object sender, EventArgs e)
		{
			this._startPos = 0;
		}

		/// <summary>
		/// GetSearchText button click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnGetSearchText_Click(object sender, EventArgs e)
		{
			string text = this.rtContents.SelectedText;

			if (text.Length > 0)
			{
				this.txtSearch.Text = text;
				this.txtSearch.Focus();
			}
		}

		#endregion

		/// <summary>
		/// Mark save button click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSave_Click(object sender, EventArgs e)
		{
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				this.rtContents.SaveFile(saveFileDialog.FileName);
			}
		}

		/// <summary>
		/// Close button click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Called from Search Window
		/// </summary>
		/// <param name="searchText">Search words</param>
		internal void Search(string searchText)
		{
			if (!this.txtSearch.Text.Equals(searchText))
			{
				this.txtSearch.Text = searchText;
			}

			this.txtSearch.Focus();

			string text = this.txtSearch.Text;

			if (text.Length > 0)
			{
				int pos = rtContents.Find(text, this._startPos, RichTextBoxFinds.None);

				this._startPos = pos + text.Length;
			}
		}
	}
}
