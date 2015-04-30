using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KaniReg
{
	public partial class Search : Form
	{
		/// <summary>
		/// Store search history
		/// </summary>
		List<string> _history;

		/// <summary>
		/// Search text from parent
		/// </summary>
		public string SearchText
		{
			get;
			set;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public Search()
		{
			InitializeComponent();

			this._history = new List<string>();
		}

		/// <summary>
		/// Form load action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Search_Load(object sender, EventArgs e)
		{
			this.txtSearch.Text = this.SearchText;
			this.lbHistory.DataSource = this._history.ToArray();
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
		/// Search button click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSearch_Click(object sender, EventArgs e)
		{
			string text = this.txtSearch.Text;

			if(text == "")
			{
				return;
			}

			((ResultViewer)this.Owner).Search(text);

			this.ManageHistory(text);
		}

		/// <summary>
		/// Cancel button click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Hide();
		}

		/// <summary>
		/// Manage history list
		/// </summary>
		/// <param name="text"></param>
		internal void ManageHistory(string text)
		{
			if (this._history.Contains(text))
			{
				this._history.Remove(text);
			}

			this._history.Reverse();
			this._history.Add(text);
			this._history.Reverse();
			this.lbHistory.DataSource = this._history.ToArray();
		}

		private void lbHistory_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.txtSearch.Text = this.lbHistory.SelectedValue.ToString();
		}
	}
}
