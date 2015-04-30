using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.NtUser
{
	/// <summary>
	/// Ver.20121008
	/// </summary>
	class ComDlg32 : ClassBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rootKey"></param>
		/// <param name="timeZoneBias"></param>
		/// <param name="outputUtc"></param>
		/// <param name="reporter"></param>
		/// <param name="logger"></param>
		public ComDlg32(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger)
		{
		}

		protected override void Initialize()
		{
			PrintKeyInBase = true;
			KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32";
			Description = "ダイアログにより開閉されたファイル等の情報";
		}

		public override bool Process()
		{
			RegistryKey[] keys = Key.GetListOfSubkeys();

			foreach (RegistryKey key in keys)
			{
				if (key.Name.StartsWith("OpenSave"))
				{
					RegistryKey[] subkeys = key.GetListOfSubkeys();
					Reporter.Write("[" + key.Name + "]");

					if (null != subkeys && 0 < subkeys.Length)
					{
						foreach (RegistryKey subkey in subkeys)
						{
							Reporter.Write("サブキー名：" + subkey.Name);
							Reporter.Write("最終更新日：" + Library.TransrateTimestamp(subkey.Timestamp, TimeZoneBias, OutputUtc));
							// call function
							try
							{
								if (key.Name.Contains("Pidl"))
								{
									MruValues.ParseEx(subkey, Reporter, Logger);
								}
								else
								{
									MruValues.Parse(subkey, Reporter, Logger, true);
								}
							}
							catch (Exception exception)
							{
								Reporter.Write("未対応のレコードフォーマットのようです。");
								Logger.Write(LogLevel.WARNING, "未対応のValueフォーマットのようです。: " + exception.Message);
							}
						}
					}
					else
					{
						Reporter.Write("サブキーがありませんでした。");
					}
				}
				else if (key.Name.StartsWith("LastVisited"))
				{
					Reporter.Write("[" + key.Name + "]");
					if (key.Name.Contains("Pidl"))
					{
						MruValues.ParseEx(key, Reporter, Logger);
					}
					else
					{
						MruValues.Parse(key, Reporter, Logger, true);
					}
				}
				else if (key.Name.StartsWith("CIDSize"))
				{
					Reporter.Write("[" + key.Name + "]");
					MruValues.ParseEx(key, Reporter, Logger);
				}
				else if (key.Name.StartsWith("FirstFolder"))
				{
					Reporter.Write("[" + key.Name + "]");
					MruValues.ParseEx(key, Reporter, Logger);
				}
			}

			return true;
		}
	}
}
