using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Reflection;

namespace KaniReg
{
	public partial class FormMain : Form
	{
		// LoggerとReporter指定
		Logger _logger;
		Reporter _reporter;

		/// <summary>
		/// Number of target registry files
		/// </summary>
		private int _registryFileCount;

		/// <summary>
		/// Number of executed files
		/// </summary>
		private int _executedFileCount;

		/// <summary>
		/// Store Hive type ComboBox enable status
		/// </summary>
		private bool _cmbPluginEnable;

		/// <summary>
		/// Store auto save CheckBox enable status
		/// </summary>
		private bool _autoSaveEnable;

		/// <summary>
		/// Store cmbPlugin's seelcted index
		/// </summary>
		private int _cmbIndex;

		/// <summary>
		/// Store cmbPlugin's selected item text
		/// </summary>
		private string _cmbText;

		/// <summary>
		/// Store result file paths
		/// </summary>
		private List<string> _resultPathList;

		public FormMain()
		{
			InitializeComponent();
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			comboBoxPlugin.SelectedIndex = 0;

			string logPath = Assembly.GetEntryAssembly().Location.Replace(".exe", "");
			_logger = new Logger(logPath);
		}

		/// <summary>
		/// Open File menu item click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuItemOpenFile_Click(object sender, EventArgs e)
		{
			// call common method
			OpenFile();
		}

		/// <summary>
		/// Open Folder menu item click adtion
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuItemOpenFolder_Click(object sender, EventArgs e)
		{
			// call common method
			this.OpenFolder();
		}

		/// <summary>
		/// Exit menu item click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Dispose();
		}

		private void versionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Version " + Application.ProductVersion + "\r\n\r\n    Pay tribute to Harlan Carvery, the developer of RegRipper.", "Version of " + Application.ProductName, MessageBoxButtons.OK);
		}

		/// <summary>
		/// Set Controls' property when input path changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textBoxHive_TextChanged(object sender, EventArgs e)
		{
			string path = this.textBoxHive.Text;

			if(File.Exists(path))
			{
				this.comboBoxPlugin.Enabled = true;
				this.chkAutoSave.Enabled = true;
			}

			if(Directory.Exists(path))
			{
				this.comboBoxPlugin.SelectedIndex = 0;
				this.comboBoxPlugin.Enabled = false;

				this.chkAutoSave.Checked = true;
				this.chkAutoSave.Enabled = false;
			}
		}

		private void textBoxHive_DragDrop(object sender, DragEventArgs e)
		{
			string[] data = (string[])e.Data.GetData("FileDrop", false);

			if (File.Exists(data[0]))
			{
				textBoxHive.Text = data[0];
			}
			else if (Directory.Exists(data[0]))
			{
				textBoxHive.Text = data[0];
			}
		}

		private void textBoxHive_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.All;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		/// <summary>
		/// Hive Browse button click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnBrowseFolder_Click(object sender, EventArgs e)
		{
			// call common method
			this.OpenFolder();
		}

		/// <summary>
		/// Folder Browse butoon click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnBrowsFile_Click(object sender, EventArgs e)
		{
			// call common method
			OpenFile();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void radioButtonDirect_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonDirect.Checked)
			{
				numericUpDownTimezone.Enabled = true;
			}
			else
			{
				numericUpDownTimezone.Enabled = false;
			}
		}

		/// <summary>
		/// Rip It button click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonAnalyze_Click(object sender, EventArgs e)
		{
			string logPath = Assembly.GetEntryAssembly().Location.Replace(".exe", "");
			_logger = new Logger(logPath += DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".log");

			this._executedFileCount = 0;

			// タイムゾーンオプションの取得とタイムゾーンの取得・計算
			short timeZoneHours = 0;

			if (radioButtonLocal.Checked)
			{
				// double => 整数系 ネパールとかで無理繰りキャストしたらコケるんだっけ？その時はtry-catch
				timeZoneHours = (short)TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalHours;
			}
			else if (radioButtonDirect.Checked)
			{
				timeZoneHours = (short)numericUpDownTimezone.Value;
			}

			// 出力日付をUTCとするかどうか
			bool outputUtc = false;
			if (radioButtonUtc.Checked)
			{
				outputUtc = true;
			}

			// Store cmb selection info
			this._cmbIndex = this.comboBoxPlugin.SelectedIndex;
			this._cmbText = this.comboBoxPlugin.SelectedItem.ToString();

			// 引数を設定
			Arguments arguments = new Arguments(textBoxHive.Text, "", timeZoneHours, outputUtc);

			// 処理完了後に、コントロールのEnableを処理開始前の状態に戻すために退避する
			this._cmbPluginEnable = this.comboBoxPlugin.Enabled;
			this._autoSaveEnable = this.chkAutoSave.Enabled;

			fileToolStripMenuItem.Enabled = false;
			helpToolStripMenuItem.Enabled = false;
			textBoxHive.Enabled = false;
			btnBrowseFolder.Enabled = false;
			comboBoxPlugin.Enabled = false;
			panel1.Enabled = false;
			buttonAnalyze.Enabled = false;
			buttonClose.Enabled = false;

			this.chkAutoSave.Enabled = false;
			this.chkShowViewer.Enabled = false;

			buttonCancel.Visible = true;
			progressBarFile.Visible = true;
			progressBarPlugin.Visible = true;
			
			// run backgroud trunsaction
			backgroundWorker.RunWorkerAsync(arguments);
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			backgroundWorker.CancelAsync();
			this.lblProgress.Text = "";
		}

		/// <summary>
		/// Close buttonh click action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonClose_Click(object sender, EventArgs e)
		{
			Dispose();
		}

		/// <summary>
		/// background transaction
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			Arguments args = (Arguments)e.Argument;

			// 今回はやっつけ的な増築で対処　本気で1000台レベルをやることになったら考える
			var list = this.GetFileList(args.HiveFileName);

			if (list.Count == 0)
			{
				MessageBox.Show("指定のパスにレジストリファイルがありません。", "KaniReg",
					MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

				e.Cancel = true;
				return;
			}

			this._resultPathList = new List<string>();

			var errors = new StringBuilder();

			// 使い回し用
			FileInfo info;
			string hiveName;
			Arguments arguments;
			Analyzer analyzer;
						
			foreach (var file in list)
			{
				_reporter = new Reporter();

				// hive指定の取得
				hiveName = string.Empty;
				info = new FileInfo(file);

				if (0 == this._cmbIndex)
				{
					if ("NTUSER.DAT".Equals(info.Name, StringComparison.OrdinalIgnoreCase))
					{
						hiveName = "NTUSER";
					}
					else if ("USRCLASS.DAT".Equals(info.Name, StringComparison.OrdinalIgnoreCase))
					{
						hiveName = "USRCLASS";
					}
					else
					{
						hiveName = info.Name.ToUpper();
					}
				}
				else
				{
					hiveName = this._cmbText;
				}

				// Refill hiveName Arguments
				arguments = new Arguments(file, hiveName, args.TimeZoneBias, args.OutputUtc);

				// Show processing file name on progress bar
				this.Invoke((MethodInvoker)delegate()
				{
					this.lblProgress.Text = file;
				});

				analyzer = new Analyzer(arguments, _reporter, _logger, backgroundWorker);
				bool result = analyzer.Process(this._executedFileCount);

				if (!string.IsNullOrEmpty(analyzer.ErrorMessage))
				{
					errors.AppendLine(analyzer.ErrorMessage);
				}

				if (backgroundWorker.CancellationPending)
				{
					e.Cancel = true;
					return;
				}

				this.SaveReport(file, analyzer.ErrorMessage, result);

				this._executedFileCount++;

				int progress = (int)(((double)this._executedFileCount /
									  (double)this._registryFileCount) * 100);
				progress = (100 > progress) ? progress : 100;
				this.backgroundWorker.ReportProgress(progress, "Files");
			}

			e.Result = errors.ToString();
		}

		private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if ("Files".Equals(e.UserState))
			{
				progressBarFile.Value = e.ProgressPercentage;
			}
			else if("Plugins".Equals(e.UserState))
			{
				progressBarPlugin.Value = e.ProgressPercentage;
			}
		}

		/// <summary>
		/// background transaction completed action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				MessageBox.Show("処理を中止しました。", "解析中止", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				if ((e.Result == null) || (e.Result.ToString() == string.Empty))
				{
					MessageBox.Show("処理が完了しました。", "解析完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show("正常に処理できなかったファイルが存在します。\nログで詳細を確認してください。",
						"異常終了", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

				// Create Result List
				if (this._resultPathList.Count > 0)
				{
					string path = Assembly.GetEntryAssembly().Location.Replace(".exe", "ResultList");
					path += DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".txt";

					var sb = new StringBuilder();

					foreach (var s in this._resultPathList)
					{
						sb.AppendLine(s);
					}

					File.AppendAllText(path, sb.ToString(), Encoding.UTF8);
				}
			}

			fileToolStripMenuItem.Enabled = true;
			helpToolStripMenuItem.Enabled = true;
			textBoxHive.Enabled = true;
			btnBrowseFolder.Enabled = true;
			comboBoxPlugin.Enabled = this._cmbPluginEnable;
			panel1.Enabled = true;
			buttonAnalyze.Enabled = true;
			progressBarFile.Value = 0;
			progressBarPlugin.Value = 0;
			buttonClose.Enabled = true;

			this.chkAutoSave.Enabled = this._autoSaveEnable;
			this.chkShowViewer.Enabled = true;
			this.lblProgress.Text = "";

			buttonCancel.Visible = false;
			progressBarFile.Visible = false;
			progressBarPlugin.Visible = false;
			
			Application.DoEvents();

			if (this.chkShowViewer.Checked && (this._resultPathList.Count > 0))
			{
				var viewer = new ResultViewer(this._resultPathList);
				viewer.Show();
			}
		}

		/// <summary>
		/// Save results
		/// </summary>
		/// <param name="fileName">Target file path</param>
		/// <param name="error">Error text</param>
		/// <param name="IsCreateFile">Set to create result file or not when error exists</param>
		private void SaveReport(string fileName, string error, bool IsCreateFile)
		{
			string resultName = fileName + ".txt";

			if (string.IsNullOrEmpty(error))
			{
				// No error
				bool confirmed = false;
				FileInfo info;

				if (this.chkAutoSave.Checked)
				{
					// No error And Auto save
					this._reporter.Close(resultName);
					this._resultPathList.Add(resultName);
					this._logger.Write(LogLevel.INFO, "【処理完了】：" + fileName);
				}
				else
				{
					// No error And Manual save
					while (!confirmed)
					{
						this.Invoke((MethodInvoker)delegate()
						{
							saveFileDialogReport.FileName = resultName;

							if (DialogResult.OK == saveFileDialogReport.ShowDialog())
							{
								info = new FileInfo(saveFileDialogReport.FileName);
								if (!comboBoxPlugin.Items.Contains(info.Name.ToLower()) &&
									!"NTUSER.DAT".Equals(info.Name.ToLower()))
								{
									_reporter.Close(saveFileDialogReport.FileName);
                                    _resultPathList.Add(saveFileDialogReport.FileName);
									this._logger.Write(LogLevel.INFO, "【処理完了】：" + fileName);
									confirmed = true;
								}
								else
								{
									MessageBox.Show("ファイル名が危険なため避けてください。", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
								}
							}
							else
							{
								if (DialogResult.Yes == MessageBox.Show("レポートを作成せずに終了しますが宜しいですか？", "レポート作成確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
								{
									this._logger.Write(LogLevel.INFO, "【正常終了（ファイル作成なし）】");
									_reporter.Close();
									confirmed = true;
								}
							}
						});
					}
				}
			}
			else
			{
				// Error esists
				_logger.Write(LogLevel.ERROR, string.Format("{0}\n{1}\n", fileName, error));

				if (IsCreateFile)
				{
					this._reporter.Close(resultName);
					this._resultPathList.Add(resultName);
					this._logger.Write(LogLevel.INFO, "【処理完了（エラー有・パースファイル生成）】：" + fileName);
				}
				else
				{
					_reporter.Close();
					this._logger.Write(LogLevel.INFO, "【処理完了（エラー有・パースファイル生成なし）】：" + fileName);
				}
			}
		}

		/// <summary>
		/// common method opening hive file dialog
		/// </summary>
		private void OpenFile()
		{
			// open successfully
			if (DialogResult.OK == openFileDialogHive.ShowDialog())
			{
				// set hive file name from dialog
				textBoxHive.Text = openFileDialogHive.FileName;
			}
		}

		/// <summary>
		/// Common method opening folder select dialog
		/// </summary>
		private void OpenFolder()
		{
			if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				this.textBoxHive.Text = folderBrowserDialog.SelectedPath;
			}
		}

		/// <summary>
		/// Get file path list only registry files
		/// </summary>
		/// <param name="target">Input value of textBoxHive</param>
		/// <returns>Path List</returns>
		private List<string> GetFileList(string target)
		{
			var list = new List<string>();

			if (File.Exists(target) && Registry.IsRegistry(target))
			{
				// target is registry file
				list.Add(target);

				this._registryFileCount = 1;
			}
			else if (Directory.Exists(target))
			{
				var paths = Directory.GetFiles(target, "*", SearchOption.AllDirectories);

				foreach (var path in paths)
				{
					// If path is registry file add to list
					if (Registry.IsRegistry(path))
					{
						list.Add(path);
					}
				}

				this._registryFileCount = list.Count;
			}

			// ファイルでもフォルダでもなければ0件のリストが返る

			return list;
		}
	}
}