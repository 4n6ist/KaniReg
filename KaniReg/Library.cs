using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace KaniReg
{

	/// <summary>
	/// Classes Extract from CommonLibrary And New Defined
	/// </summary>
	class Library
	{
		/// <summary>
		/// バイト配列を愚直に16進数化
		/// </summary>
		/// <param name="bytes">バイト配列</param>
		/// <param name="separator">区切りの文字列</param>
		/// <returns>16進数化文字列</returns>
		public static string ByteArrayToHexString(byte[] bytes, string separator)
		{
			// construct StringBuilder class object
			StringBuilder builder = new StringBuilder();

			// bytes array convert to Hex string
			for (int i = 0; i < bytes.Length; i++)
			{
				if (0 < builder.Length)
				{
					builder.Append(separator);
				}

				if (bytes[i] <= 0x0F)
				{
					builder.Append("0" + String.Format("{0:X}", bytes[i]));
				}
				else
				{
					builder.Append(String.Format("{0:X}", bytes[i]));
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// バイト配列からuintのリストを作成する
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static List<uint> ByteArrayToUIntList(byte[] bytes)
		{
			List<uint> list = new List<uint>();
			for (ushort index = 0; index < bytes.Length; index += 4)
			{
				list.Add(BitConverter.ToUInt32(Library.ExtractArrayElements(bytes, index, 4), 0));
			}

			return list;
		}

		/// <summary>
		/// 取得長指定なしで配列要素をsubstring的に抽出する
		/// </summary>
		/// <param name="bytes"></param>
		/// <param name="start"></param>
		/// <returns></returns>
		public static byte[] ExtractArrayElements(byte[] bytes, ulong start)
		{
			// 実処理は全体を取得する指定をして長さ指定をコール
			return ExtractArrayElements(bytes, start, ((ulong)bytes.Length - start));
		}

		/// <summary>
		/// 配列要素をsubstring的に抽出する
		/// </summary>
		/// <param name="bytes">対象配列</param>
		/// <param name="start">開始インデックス</param>
		/// <param name="length">抽出する長さ</param>
		/// <returns>byte[]</returns>
		/// <throws>ArgumentException</throws>
		/// <remarks>
		/// こちらは Perl の substring の代替と思われる。
		/// オリジナルの Parse::Win32Registry::WinNT::Value::_extract_data に相当するのは下の ExtractData
		/// </remarks>
		public static byte[] ExtractArrayElements(byte[] bytes, ulong start, ulong length)
		{
			if (bytes.Length > 0 && start >= 0 && length > 0 && (ulong)bytes.Length >= (start + length))
			{
				List<byte> substitute = new List<byte>();

				for (ulong idx = start; idx < start + length; idx++)
				{
					substitute.Add(bytes[idx]);
				}

				return substitute.ToArray();
			}
			else
			{
				throw new ArgumentException("Argument is not suitable.");
			}
		}

		/// <summary>
		/// Parse::Win32Registry::WinNT::Value::_extract_data の移植
		/// </summary>
		/// <param name="regFile">ハイブファイル</param>
		/// <param name="offsetToData">vkヘッダから取得したデータへのオフセット</param>
		/// <param name="dataLength">vkヘッダから取得したデータ長</param>
		/// <returns>データ</returns>
		public static byte[] ExtractData(byte[] regFile, ulong offsetToData, ulong dataLength)
		{
			if ((regFile.Length > 0) &&
				(offsetToData >= 0) &&
				(offsetToData != 0xffffffff) &&
				(dataLength > 0) &&
				((ulong)regFile.Length >= (offsetToData + dataLength)))
			{
				// sysseek($fh, $offset_to_data, 0);
				int pos = (int)offsetToData;
				// ジェネリック List や配列のインデックスは int。 渡された ulong が int を超えないかは上でチェック済み

				byte[] dataHeader = new byte[4];
				Array.Copy(regFile, pos, dataHeader, 0, 4);
				// 4個取れない場合は例外を他所で握り潰して継続するのでここではエラー処理は考慮しない　以下同様

				pos += 4;

				uint maxDataLength = BitConverter.ToUInt32(dataHeader, 0);
				
				if (maxDataLength > 0x7fffffff)
				{
					maxDataLength = (0xffffffff - maxDataLength) + 1;
				}

				if (dataLength > maxDataLength)
				{
					byte[] dbEntry = new byte[8];
					Array.Copy(regFile, pos, dbEntry, 0, 8);

					string sig = Encoding.ASCII.GetString(dbEntry, 0, 2);
					uint numDataBlocks = BitConverter.ToUInt16(dbEntry, 2);
					uint offsetToDataBlockList = BitConverter.ToUInt32(dbEntry, 4);

					if (sig != "db")
					{
						throw new Exception("不正なシグネチャです");
					}

					offsetToDataBlockList += Constants.OFFSET_BASE;

					pos = (int)offsetToDataBlockList + 4;

					byte[] dataBlockList = new byte[numDataBlocks * 4];
					Array.Copy(regFile, pos, dataBlockList, 0, numDataBlocks * 4);

					List<int> offsets = new List<int>();

					// offsets
					for (int i = 0; i < dataBlockList.Length / 4; i++)
					{
						offsets.Add((int)(BitConverter.ToUInt32(dataBlockList, i * 4) + Constants.OFFSET_BASE));
					}

					List<byte> data = new List<byte>();

					for (int i = 0; i < offsets.Count; ++i)
					{
						pos = offsets[i];

						byte[] blockHeader = new byte[4];
						Array.Copy(regFile, pos, blockHeader, 0, 4);
						
						uint blocklength = BitConverter.ToUInt32(blockHeader, 0);

						if (blocklength > 0x7fffffff)
						{
							blocklength = 0xffffffff - blocklength + 1;
						}

						pos += 4;

						byte[] blockData = new byte[blocklength - 8];
						Array.Copy(regFile, pos, blockData, 0, blocklength - 8);

						data.AddRange(blockData);
					}

					if (data.Count < (int)dataLength)
					{
						throw new Exception("データブロックが不足しています");
					}

					byte[] results = new byte[(int)dataLength];

					data.CopyTo(0, results, 0, (int)dataLength);

					return results;
				}
				else
				{
					byte[] results = new byte[(int)dataLength];
					Array.Copy(regFile, pos, results, 0, (int)dataLength);

					return results;
				}
			}

			return new byte[0];
		}

		public static double UnpackWindowsTime(byte[] bytes)
		{
			if (null == bytes || 8 != bytes.Length)
			{
				return 0;
			}

			//long low = BitConverter.ToInt32(Library.ExtractArrayElements(bytes, 0, 4), 0);
			//long high = BitConverter.ToInt32(Library.ExtractArrayElements(bytes, 4), 0);
			ulong filetime = BitConverter.ToUInt64(bytes, 0);

			//            double epochTime = (high * System.Math.Pow(2, 32) + low - 116444736000000000) / 10000000;
			double epochTime = filetime * Math.Pow(10, -7) - 11644473600;
			/*
						double epchOffset = (new DateTime(1970, 1, 1)).Ticks / 10000000;
						epochTime += epchOffset;
			*/
			if (0 > epochTime)
			{
				epochTime = 0;
			}

			return epochTime;
		}

		/// <summary>
		/// バイナリからの変換数値(ulong)からタイムスタンプを計算する.
		/// </summary>
		/// <param name="filetime"></param>
		/// <returns></returns>
		public static double CalculateTimestamp(ulong filetime)
		{
			double timestamp;

			if (0 == filetime)
			{
				timestamp = 0;
			}
			else
			{
				timestamp = filetime * Math.Pow(10, -7) - 11644473600;
			}

			return (0 < timestamp) ? timestamp : 0;
		}

		/// <summary>
		/// UNIXタイムを指定タイムゾーン差分のDatetime形式にして返すoverload
		/// </summary>
		/// <param name="timestamp">UNIXタイム</param>
		/// <param name="diffHours">diffHours</param>
		/// <returns></returns>
		public static string TransrateTimestamp(double timestamp, short diffHours, bool outputUtc)
		{
			DateTime dateTime = new DateTime(1970, 1, 1);
			string suffix = string.Empty;
			if (!outputUtc)
			{
				dateTime = dateTime.AddSeconds(timestamp).AddHours((double)diffHours);
				suffix = " +" + diffHours.ToString("##") + ":00";
			}
			else
			{
				dateTime = dateTime.AddSeconds(timestamp);
				suffix = " (UTC)";
			}
			return dateTime.ToString("yyyy/MM/dd HH:mm:ss" + suffix);
		}

		/// <summary>
		/// Translate FILETIME object (2 DWORDS) to Unix time, to be passed to gmtime() or localtime()
		/// </summary>
		/// <param name="low"></param>
		/// <param name="high"></param>
		/// <returns>timestamp</returns>
		public static double GetTime(uint low, uint high)
		{
			double timestamp;

			if (0 == low && 0 == high)
			{
				timestamp = 0;
			}
			else
			{
				low -= 0xD53E8000;
				high -= 0x019DB1DE;
				timestamp = high * 429.4967296 + low / 1E7;
			}

			return (0 < timestamp) ? timestamp : 0;
		}

		/// <summary>
		/// ASCII文字だけかチェックを実行する.
		/// </summary>
		/// <param name="bytes">バイト配列</param>
		/// <returns>チェック結果:正否</returns>
		public static bool IsAscii(byte[] bytes)
		{
			// ASCIIだけ(制御文字以外の0x20～0x7E)かどうかチェックする
			foreach (byte item in bytes)
			{
				if (0x20 > item || 0x7E < item)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// バイト配列を指定した文字で区切って文字列化表示する.
		/// </summary>
		/// <param name="bytes">バイト配列</param>
		/// <param name="spacer">区切り文字</param>
		/// <returns>変換後文字列</returns>
		public static string ByteArrayToString(byte[] bytes, string spacer)
		{
			StringBuilder builder = new StringBuilder();

			foreach (byte item in bytes)
			{
				if (0 < builder.Length)
				{
					builder.Append(spacer);
				}
				builder.Append(item.ToString("X2"));
			}

			return builder.ToString();
		}

		/// <summary>
		/// バイト配列のデータをUnicode NULLで分断する.
		/// </summary>
		/// <param name="bytes">対象byte配列</param>
		/// <returns>分断したbyte配列のリスト</returns>
		public static List<byte[]> SplitByteData(byte[] bytes)
		{
			// 返却値格納リスト
			List<byte[]> list = new List<byte[]>();
			// 内部処理用一次リスト
			List<byte> innerList = new List<byte>();

			for (int i = 0; i < bytes.Length; i += 2)
			{
				if (!(0x00 == bytes[i] && 0x00 == bytes[i + 1]))
				{
					innerList.Add(bytes[i]);
					innerList.Add(bytes[i + 1]);
				}
				else
				{
					list.Add(innerList.ToArray());
					innerList.Clear();
				}
			}

			if (0 < innerList.Count)
			{
				list.Add(innerList.ToArray());
			}

			return list;
		}

		/// <summary>
		/// 指定した正規表現に文字列がマッチするかチェックする.
		/// </summary>
		/// <param name="pattern">正規表現パターン</param>
		/// <param name="value">チェックしたい値</param>
		/// <returns>チェック結果:正否</returns>
		//        public static bool RegexMatch(string pattern, string value) {
		//            Regex regex = new Regex(pattern);
		//            return regex.IsMatch(value);
		//        }

		/// <summary>
		/// RegistryKeyから配下の全RegistryValueを取得してDictionaryに整理する。
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static Dictionary<string, string> ValuesToDictionary(RegistryKey key)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();

			if (null != key)
			{
				RegistryValue[] values = key.GetListOfValues();
				if (null != values && 0 < values.Length)
				{
					foreach (RegistryValue value in values)
					{
						if ((null == value.Name || string.Empty.Equals(value.Name)) &&
							(null == value.GetDataAsObject() || string.Empty.Equals(value.GetDataAsObject().ToString())))
						{
							continue;
						}
						dictionary.Add(value.Name, value.GetDataAsObject().ToString());
					}
				}
			}

			return dictionary;
		}

		public static void WriteNoValue(string keyPath, Reporter reporter)
		{
			reporter.Write(keyPath + " にはValueがありませんでした。");
		}

		public static string TranseleteRot13(string name, bool isAscii)
		{

			byte item;
			byte postItem;
			short step;
			List<byte> replacedList = new List<byte>();


			Encoding encoding;
			if (isAscii)
			{
				encoding = Encoding.ASCII;
			}
			else
			{
				encoding = Encoding.Unicode;
			}
			byte[] bytes = encoding.GetBytes(name);

			if (Encoding.Unicode.Equals(encoding))
			{
				step = 2;
			}
			else
			{
				step = 1;
			}

			for (int i = 0; i < bytes.Length; i += step)
			{
				item = bytes[i];
				postItem = (2 == step) ? bytes[i + 1] : (byte)0x00;
				if (0x00 == postItem && (0x41 <= item && 0x4D >= item || 0x61 <= item && 0x6D >= item))
				{
					replacedList.Add((byte)(item + 0x0D));
				}
				else if (0x00 == postItem && (0x4E <= item && 0x5A >= item || 0x6E <= item && 0x7A >= item))
				{
					replacedList.Add((byte)(item - 0x0D));
				}
				else
				{
					replacedList.Add((byte)(item));
				}
				if (2 == step)
				{
					replacedList.Add(postItem);
				}
			}

			return encoding.GetString(replacedList.ToArray());
		}


		public static string GetClassName(ClassBase child)
		{
			string fullName = child.GetType().Name;
			int lastDot = fullName.LastIndexOf(".");
			return fullName.Substring(lastDot + 1);
		}
	}
}