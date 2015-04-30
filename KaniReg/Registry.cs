using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace KaniReg
{

	/// <summary>
	/// Hiveファイル全体プロパティ取扱クラス
	/// 現状NT系のみ対応、95系に対応させる場合は1段下げてphacade化
	/// </summary>
	public class Registry
	{

		private string _fileName;
		private uint _offsetToFirstKey;
		private byte[] _timestamp;  // 未使用
		private byte[] _regFile;
		private string _embededFileName;

		public string FileName
		{
			set
			{
				_fileName = value;
			}
			get
			{
				return _fileName;
			}
		}

		public uint OffsetToFirstKey
		{
			set
			{
				_offsetToFirstKey = value;
			}
			get
			{
				return _offsetToFirstKey;
			}
		}

		public byte[] Timestamp
		{
			set
			{
				_timestamp = value;
			}
			get
			{
				return _timestamp;
			}
		}

		public byte[] RegFile
		{
			set
			{
				_regFile = value;
			}
			get
			{
				return _regFile;
			}
		}

		public string EmbededFileName
		{
			set
			{
				_embededFileName = value;
			}
			get
			{
				return _embededFileName;
			}
		}

		public Registry(string hiveFileName)
		{
			this.FileName = hiveFileName;

			//===== ファイルを読み取りbyteの配列として保持 =====
			FileStream stream = new FileStream(hiveFileName, FileMode.Open, FileAccess.Read);
			byte[] bytes = new byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);
			stream.Close();

			this.RegFile = bytes;

			// ヘッダー部分の抜き出し
			byte[] header = Library.ExtractArrayElements(bytes, 0, 0x70);

			// Signatureとtimestampを取得
			string signature = Encoding.ASCII.GetString(Library.ExtractArrayElements(header, 0, 4));
			this.Timestamp = Library.ExtractArrayElements(header, 12, 8);

			// SIGNATUREがNT系規定か確認
			if (!Constants.SIGNATURE_NT.Equals(signature))
			{
				// NT系でなければException
				throw new Exception("File signature is not match.");
			}

			// Hiveファイル名
			this.EmbededFileName = Encoding.Unicode.GetString(Library.ExtractArrayElements(header, 0x30, 0x40));

			// 最初の要素へのoffset
			byte[] offsetSource = Library.ExtractArrayElements(bytes, 36, 4);
			this.OffsetToFirstKey = Constants.OFFSET_BASE + BitConverter.ToUInt32(offsetSource, 0);
		}

		/// <summary>
		/// Judge param file is registry or not
		/// </summary>
		/// <param name="path">File path</param>
		/// <returns>If registry return true</returns>
		public static bool IsRegistry(string path)
		{
			var reg = new Regex(@"LOG\d*$", RegexOptions.IgnoreCase);

			if (reg.IsMatch(path))
			{
				return false;
			}

			reg = new Regex(@"sav\d*$", RegexOptions.IgnoreCase);

			if (reg.IsMatch(path))
			{
				return false;
			}

			// 素人さん向けじゃないのでこの程度のチェックにしておく
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				// Get signature from First 4 bytes of file
				byte[] bytes = new byte[4];
				fs.Read(bytes, 0, 4);

				string sign = Encoding.ASCII.GetString(bytes);

				return Constants.SIGNATURE_NT.Equals(sign);
			}
		}

		/// <summary>
		/// Rootのキーを取得
		/// </summary>
		/// <returns></returns>
		public RegistryKey GetRootKey()
		{
			return new RegistryKey(this.RegFile, this.OffsetToFirstKey, null);
		}
	}
}