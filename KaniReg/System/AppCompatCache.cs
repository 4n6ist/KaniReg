using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace KaniReg.SysHive
{
	/// <summary>
	/// Ver.20120817
	/// </summary>
	class AppCompatCache : ClassBase
	{
		#region Constants

		/// <summary>
		/// 結果辞書キー文字列：ModTime
		/// </summary>
		private const string MOD_TIME = "ModTime";

		/// <summary>
		/// 結果辞書キー文字列：UpdTime
		/// </summary>
		private const string UPD_TIME = "UpdTime";

		/// <summary>
		/// 結果辞書キー文字列：Size
		/// </summary>
		private const string SIZE = "Size";

		/// <summary>
		/// 結果辞書キー文字列：Executed
		/// </summary>
		private const string EXEC = "Executed";

		/// <summary>
		/// パス文字列や日付に含まれない区切り文字列（問題あったら要変更）："?**?"
		/// </summary>
		private const string DELIM = "?**?";

		#endregion

		public AppCompatCache(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet)
			: base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet)
		{
		}

		protected override void Initialize()
		{
			// First thing to do is get the ControlSet00x marked current...this is going
			// to be used over and over again in plugins that access the system file
			PrintKeyInBase = true;
			KeyPath = @"ControlSet00" + Current + @"\Control\Session Manager";
			Description = "プログラムの実行履歴情報";
		}

		public override bool Process()
		{
			// 最初にXP固有のキーの取得を試みる
			RegistryKey key = Key.GetSubkey("AppCompatibility");

			if (key == null)
			{
				// XP固有キーが存在しない場合、XP以外のキー名で検索
				key = Key.GetSubkey("AppCompatCache");
			}

			if (key == null)
			{
				Reporter.Write("Keyが見つかりませんでした。");
				return true;
			}
			
			RegistryValue value = key.GetValue("AppCompatCache");
			
			uint sig = BitConverter.ToUInt32(Library.ExtractArrayElements(value.Data, 0, 4), 0);
			Reporter.Write("Signature: 0x" + sig.ToString());

			#region Debug
#if DEBUG
			// データ部分として取得されたバイナリを見たいときshowHexをtrueにして表示させる
			bool showHex = false;
			if (showHex)
			{
				StringBuilder hex = new StringBuilder();
				for (int i = 0; i < Math.Min(value.Data.Length, 2000); ++i)
				{
					byte b = value.Data[i];
					hex.Append(b.ToString("X2"));
					hex.Append(" ");
				}

				string list = hex.ToString().TrimEnd();
				System.Windows.Forms.MessageBox.Show(list, "DataViewer",
					System.Windows.Forms.MessageBoxButtons.OK,
					System.Windows.Forms.MessageBoxIcon.Information,
					System.Windows.Forms.MessageBoxDefaultButton.Button1,
					System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly);
			}
#endif
			#endregion

			// Dictionary<パス文字列, Dictionary<値の種類, 値>>
			Dictionary<string, Dictionary<string, string>> results;

			switch (sig)
			{
				// XP32Bit
				case 0xdeadbeef:
					results = this.ParseXp32(value.Data);
					break;

				// Win2003, Vista, Win2008
				case 0xbadc0ffe:
					results = this.ParseWin2003(value.Data);
					break;

				// Win2008R2, Win7
				case 0xbadc0fee:
					results = ParseWin7(value.Data);
					break;

				// 本来は判定不能 ＝ 対象外バージョンだが、暫定的に Win8 として扱う
				default:
					results = ParseWin8(value.Data);
					//Reporter.Write("未対応バージョンのOSです。");
					//results = null;
					break;
			}

			if (results != null)
			{
				StringBuilder sb = new StringBuilder();

				// "temp"を含むパスを最後に列挙する
				var tempList = new List<string>();

				foreach (string s in results.Keys)
				{
					string path = s.Substring(0, s.IndexOf(DELIM));

					sb.AppendLine(path);

					if (0 <= s.IndexOf("temp", StringComparison.OrdinalIgnoreCase))
					{
						tempList.Add(path);
					}

					var values = results[s];

					if (values.ContainsKey(MOD_TIME))
					{
						sb.AppendLine("ModTime: " + values[MOD_TIME]);
					}

					if (values.ContainsKey(UPD_TIME))
					{
						sb.AppendLine("UpdTime: " + values[UPD_TIME]);
					}

					if (values.ContainsKey(SIZE))
					{
						sb.AppendLine("Size: " + values[SIZE]);
					}

					if (values.ContainsKey(EXEC))
					{
						sb.AppendLine(values[EXEC]);
					}

					sb.AppendLine();
				}

				if (0 < tempList.Count)
				{
					sb.AppendLine("Temp paths found:");

					foreach (string path in tempList)
					{
						sb.AppendLine(path);
					}
				}

				Reporter.Write(sb.ToString());
			}

			return true;
		}

		/// <summary>
		/// parse 32-bit XP data
		/// </summary>
		/// <param name="data">Registry data</param>
		/// <returns>Dictionary(File path, Dictionary(Value type, Value))</returns>
		private Dictionary<string, Dictionary<string, string>> ParseXp32(byte[] data)
		{
			Reporter.Write("WinXP, 32-bit");

			uint numEntries = BitConverter.ToUInt32(data, 4);

			var results = new Dictionary<string, Dictionary<string, string>>();

			string[] derim = new string[] { "\0\0" };

			for (int i = 0; i < numEntries; ++i)
			{
				// header is 400 bytes; each structure is 552 bytes in size
				byte[] x = Library.ExtractArrayElements(data, (ulong)(400 + i * 552), 552);

				string file = Encoding.ASCII.GetString(x, 0, 488);

				string[] array = file.Split(derim, StringSplitOptions.RemoveEmptyEntries);

				file = (array.Length == 0) ? file : array[0];

				file = file.Replace("\0", "");
				file = Regex.Replace(file, @"^(\\\?\?\\)", "");

				if (string.IsNullOrEmpty(file))
				{
					continue;
				}

				string modTime = Library.TransrateTimestamp(
					Library.CalculateTimestamp(BitConverter.ToUInt64(x, 528)), TimeZoneBias, OutputUtc);

				uint size1 = BitConverter.ToUInt32(x, 536);
				uint size2 = BitConverter.ToUInt32(x, 540);
				string size = (size2 == 0) ? size1.ToString() : "Too big";

				string updTime = Library.TransrateTimestamp(
					Library.CalculateTimestamp(BitConverter.ToUInt64(x, 544)), TimeZoneBias, OutputUtc);

				var result = new Dictionary<string, string>();

				result.Add(MOD_TIME, modTime);
				result.Add(UPD_TIME, updTime);
				result.Add(SIZE, size);

				results.Add(file + DELIM + modTime, result);
			}

			return results;
		}

		/// <summary>
		/// parse Win2k3, Vista, Win2k8 data
		/// </summary>
		/// <param name="data">Registry data</param>
		/// <returns>Dictionary(File path, Dictionary(Value type, Value))</returns>
		private Dictionary<string, Dictionary<string, string>> ParseWin2003(byte[] data)
		{
			uint numEntries = BitConverter.ToUInt32(data, 4);

			int structSize = 0;

			ushort length = BitConverter.ToUInt16(data, 8);
			ushort maxLength = BitConverter.ToUInt16(data, 10);
			uint padding = BitConverter.ToUInt32(data, 12);

			// 結局 padding=0 だけでOSを判定しているのだが((maxLength - length) == 2)は何のために？？
			// ここでelseになっても以下の処理が実行されてしまうのだが忠実に移植する
			if ((maxLength - length) == 2)
			{
				// if $padding == 0, 64-bit; otherwise, 32-bit
				if (padding == 0)
				{
					structSize = 32;
					Reporter.Write("Win2K3/Vista/Win2K8, 64-bit");
				}
				else
				{
					structSize = 24;
					Reporter.Write("Win2K3/Vista/Win2K8, 32-bit");
				}
			}

			var results = new Dictionary<string, Dictionary<string, string>>();

			for (int i = 0; i < numEntries; ++i)
			{
				byte[] structData =
					Library.ExtractArrayElements(data, (ulong)(8 + structSize * i), (ulong)structSize);

				if (structSize == 24) // 32bit
				{
					ushort len = BitConverter.ToUInt16(structData, 0);
					ushort maxLen = BitConverter.ToUInt16(structData, 2); // unused here
					uint offset = BitConverter.ToUInt32(structData, 4);
					//int t0 = BitConverter.ToInt32(structData, 8);
					//int t1 = BitConverter.ToInt32(structData, 12); // オリジナルでは modTime を作る
					uint f0 = BitConverter.ToUInt32(structData, 16);
					uint f1 = BitConverter.ToUInt32(structData, 20);

					string file =
						Encoding.ASCII.GetString(Library.ExtractArrayElements(data, offset, len));
					file = file.Replace("\0", "");
					file = Regex.Replace(file, @"^(\\\?\?\\)", "");

					string modTime = Library.TransrateTimestamp(Library.CalculateTimestamp(
						BitConverter.ToUInt64(structData, 8)), TimeZoneBias, OutputUtc);

					var result = new Dictionary<string, string>();

					result.Add(MOD_TIME, modTime);

					if ((f0 < 4) && Convert.ToBoolean(f0 & 0x2))
					{
						result.Add(EXEC, EXEC); // Dicのvalueは何でもいいが"Executed"を出力するので詰めておく
					}

					results.Add(file + DELIM + modTime, result);
				}
				else if (structSize == 32) // 64bit
				{
					ushort len = BitConverter.ToUInt16(structData, 0);
					ushort maxLen = BitConverter.ToUInt16(structData, 2);	// unused here
					uint pad = BitConverter.ToUInt32(structData, 4);			// unused here
					uint offset0 = BitConverter.ToUInt32(structData, 8);
					uint offset1 = BitConverter.ToUInt32(structData, 12);
					//int t0 = BitConverter.ToInt32(structData, 16);
					//int t1 = BitConverter.ToInt32(structData, 20); // オリジナルでは modTime を作る
					uint f0 = BitConverter.ToUInt32(structData, 24);
					uint f1 = BitConverter.ToUInt32(structData, 28);

					string file =
						Encoding.ASCII.GetString(Library.ExtractArrayElements(data, offset0, len));
					file = file.Replace("\0", "");
					file = Regex.Replace(file, @"^(\\\?\?\\)", "");

					string modTime = Library.TransrateTimestamp(Library.CalculateTimestamp(
						BitConverter.ToUInt64(structData, 16)), TimeZoneBias, OutputUtc);

					var result = new Dictionary<string, string>();

					result.Add(MOD_TIME, modTime);

					if ((f1 == 0) && (f0 > 3))
					{
						result.Add(SIZE, f0.ToString());
					}

					if ((f0 < 4) && Convert.ToBoolean(f0 & 0x2))
					{
						result.Add(EXEC, EXEC); // Dicのvalueは何でもいいが"Executed"を出力するので詰めておく
					}

					results.Add(file + DELIM + modTime, result);
				}
			}

			return results;
		}

		/// <summary>
		/// parse Win2k8R2, Win7 data
		/// </summary>
		/// <param name="data">Registry data</param>
		/// <returns>Dictionary(File path, Dictionary(Value type, Value))</returns>
		private Dictionary<string, Dictionary<string, string>> ParseWin7(byte[] data)
		{
			uint numEntries = BitConverter.ToUInt32(data, 4);

			int structSize = 0;

			ushort length = BitConverter.ToUInt16(data, 128);
			ushort maxLength = BitConverter.ToUInt16(data, 130);
			uint padding = BitConverter.ToUInt32(data, 132);

			// 結局 padding=0 だけでOSを判定しているのだが((maxLength - length) == 2)は何のために？？
			// ここでelseになっても以下の処理が実行されてしまうのだが忠実に移植する
			if ((maxLength - length) == 2)
			{
				// if $padding == 0, 64-bit; otherwise, 32-bit
				if (padding == 0)
				{
					structSize = 48;
					Reporter.Write("Win2K8R2/Win7, 64-bit");
				}
				else
				{
					structSize = 32;
					Reporter.Write("Win2K8R2/Win7, 32-bit");
				}
			}

			var results = new Dictionary<string, Dictionary<string, string>>();

			for (int i = 0; i < numEntries; ++i)
			{
				byte[] structData =
					Library.ExtractArrayElements(data, (ulong)(128 + structSize * i), (ulong)structSize);
				
				if (structSize == 32) // 32bit
				{
					ushort len = BitConverter.ToUInt16(structData, 0);
					ushort maxLen = BitConverter.ToUInt16(structData, 2); // unused here
					uint offset = BitConverter.ToUInt32(structData, 4);
					//int t0 = BitConverter.ToInt32(structData, 8);
					//int t1 = BitConverter.ToInt32(structData, 12); // オリジナルでは modTime を作る
					uint f0 = BitConverter.ToUInt32(structData, 16);
					uint f1 = BitConverter.ToUInt32(structData, 20);

					string file =
						Encoding.ASCII.GetString(Library.ExtractArrayElements(data, offset, len));
					file = file.Replace("\0", "");
					file = Regex.Replace(file, @"^(\\\?\?\\)", "");

					string modTime = Library.TransrateTimestamp(Library.CalculateTimestamp(
						BitConverter.ToUInt64(structData, 8)), TimeZoneBias, OutputUtc);

					var result = new Dictionary<string, string>();

					result.Add(MOD_TIME, modTime);

					if (Convert.ToBoolean(f0 & 0x2))
					{
						result.Add(EXEC, EXEC); // Dicのvalueは何でもいいが"Executed"を出力するので詰めておく
					}

					results.Add(file + DELIM + modTime, result);
				}
				else // 64bit 2003のメソッドではelse ifしてたがこっちはしてない…
				{
					ushort len = BitConverter.ToUInt16(structData, 0);
					ushort maxLen = BitConverter.ToUInt16(structData, 2);	// unused here
					uint pad = BitConverter.ToUInt32(structData, 4);		// unused here
					uint offset0 = BitConverter.ToUInt32(structData, 8);
					uint offset1 = BitConverter.ToUInt32(structData, 12);
					//int t0 = BitConverter.ToInt32(structData, 16);
					//int t1 = BitConverter.ToInt32(structData, 20); // オリジナルでは modTime を作る
					uint f0 = BitConverter.ToUInt32(structData, 24);
					uint f1 = BitConverter.ToUInt32(structData, 28);

					string file =
						Encoding.ASCII.GetString(Library.ExtractArrayElements(data, offset0, len));
					file = file.Replace("\0", "");
					file = Regex.Replace(file, @"^(\\\?\?\\)", "");

					string modTime = Library.TransrateTimestamp(Library.CalculateTimestamp(
						BitConverter.ToUInt64(structData, 16)), TimeZoneBias, OutputUtc);

					var result = new Dictionary<string, string>();

					result.Add(MOD_TIME, modTime);

					if (Convert.ToBoolean(f0 & 0x2))
					{
						result.Add(EXEC, EXEC); // Dicのvalueは何でもいいが"Executed"を出力するので詰めておく
					}

					results.Add(file + DELIM + modTime, result);
				}
			}

			return results;
		}

		/// <summary>
		/// parse Win8 data
		/// </summary>
		/// <param name="data">Registry data</param>
		/// <returns>Dictionary(File path, Dictionary(Value type, Value))</returns>
		private Dictionary<string, Dictionary<string, string>> ParseWin8(byte[] data)
		{
			// これがエントリ数かどうかは非常に怪しい
			//uint numEntries = BitConverter.ToUInt32(data, 4);

			int pos = 128; // ヘッダと思われる128バイトを飛ばす

			var results = new Dictionary<string, Dictionary<string, string>>();

			//for (int i = 0; i < numEntries; ++i)
			while (pos != data.Length)
			{
				// ファイル名の次の2バイトがファイルタイムへのオフセットのパターンとそうでないパターンがある
				// 当該2バイトが 0x00 00 の場合パターン1
				// ファイル名以外に 0 0 0 0 などデータを含む場合、オフセット長さとなるパターン2
				// パターン2の場合、オフセット分飛んで8バイト不明があり、そのあとにFILETIME（ModTime）

				// WIn8ではキーのバイナリ値の先頭 128バイトがヘッダなので飛ばす
				// 8 ？？（0x30307473..で始まるパターン
				// 4 レコード長（次のレコードへのオフセット） この4バイトの次の位置からオフセット分飛ぶと次のレコード(3030Pattern)
				// 2 Filename Length 可変 Filenameのレングス分のUTF-16LE
				// 2 長さ、0x00の場合には隣の8byteが不明でFILETIMEが続く、値がある場合長さ分飛ぶ
				// 8 ？？
				// 8 FILETIME
				// 可変長 ？？？

				int offset = 0;

				// 不明な8バイトを飛ばしてデータ長（次レコードへのオフセット）を取得
				offset += 8;
				uint dataLength = BitConverter.ToUInt32(data, pos + offset);

				// データ長分ずらしてパスデータ長を取得
				offset += 4;
				ushort pathLength = BitConverter.ToUInt16(data, pos + offset);

				// パスデータ長分ずらしてパスを取得
				offset += 2;
				string file = Encoding.ASCII.GetString(
					Library.ExtractArrayElements(data, (ulong)(pos + offset), (ulong)pathLength));
				file = file.Replace("\0", "");
				file = Regex.Replace(file, @"^(\\\?\?\\)", "");

				offset += pathLength;

				// ファイルパス直後の2バイトがゼロでない場合はタイムスタンプの位置が異なる
				ushort gap = BitConverter.ToUInt16(data, pos + offset);

				string modTime;

				if (gap == 0)
				{
					offset += 10; // gapに使った2バイトと不明な8バイト分ずらす
					modTime = Library.TransrateTimestamp(Library.CalculateTimestamp(
						BitConverter.ToUInt64(data, pos + offset)), TimeZoneBias, OutputUtc);
				}
				else
				{
					offset += gap + 10;
					modTime = Library.TransrateTimestamp(Library.CalculateTimestamp(
						BitConverter.ToUInt64(data, pos + offset)), TimeZoneBias, OutputUtc);
				}

				var result = new Dictionary<string, string>();

				result.Add(MOD_TIME, modTime);

				try
				{
					results.Add(file + DELIM + modTime, result);
				}
				catch (Exception ex)
				{
					string msg = ex.Message;
				}

				pos += (int)dataLength + 12; // 3030 パターンの8バイト分とオフセット分の4バイト分をずらす
			}

			return results;
		}
	}
}
