using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg
{
	public class RegistryKey
	{
		private Logger _logger;

		private byte[] _regFile;
		private string _name;
		private ushort _nameLength;
		private string _keyPath;
		private ushort _nodeType;
		private uint _offsetToParent;
		private uint _numberOfSubkeys;
		private uint _offsetToSubkeyList;
		private uint _numberOfValues;
		private uint _offsetToValueList;
		private double _timestamp;
		private uint _offsetToSecurity;
		private ushort _classNameLength;
		private uint _offsetToClassName;
		private string _className;

		public Logger Logger
		{
			set
			{
				_logger = value;
			}
			get
			{
				return _logger;
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

		public string Name
		{
			set
			{
				_name = value;
			}
			get
			{
				return _name;
			}
		}

		public ushort NameLength
		{
			set
			{
				_nameLength = value;
			}
			get
			{
				return _nameLength;
			}
		}

		public string KeyPath
		{
			set
			{
				_keyPath = value;
			}
			get
			{
				return _keyPath;
			}
		}

		public ushort NodeType
		{
			set
			{
				_nodeType = value;
			}
			get
			{
				return _nodeType;
			}
		}

		public uint OffsetToParent
		{
			set
			{
				_offsetToParent = value;
			}
			get
			{
				return _offsetToParent;
			}
		}

		public uint NumberOfSubkeys
		{
			set
			{
				_numberOfSubkeys = value;
			}
			get
			{
				return _numberOfSubkeys;
			}
		}

		public uint OffsetToSubkeyList
		{
			set
			{
				_offsetToSubkeyList = value;
			}
			get
			{
				return _offsetToSubkeyList;
			}
		}

		public uint NumberOfValues
		{
			set
			{
				_numberOfValues = value;
			}
			get
			{
				return _numberOfValues;
			}
		}

		public uint OffsetToValueList
		{
			set
			{
				_offsetToValueList = value;
			}
			get
			{
				return _offsetToValueList;
			}
		}

		public double Timestamp
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

		public uint OffsetToSecurity
		{
			set
			{
				_offsetToSecurity = value;
			}
			get
			{
				return _offsetToSecurity;
			}
		}

		public uint OffsetToClassName
		{
			set
			{
				_offsetToClassName = value;
			}
			get
			{
				return _offsetToClassName;
			}
		}

		public ushort ClassNameLength
		{
			set
			{
				_classNameLength = value;
			}
			get
			{
				return _classNameLength;
			}
		}

		public string ClassName
		{
			set
			{
				_className = value;
			}
			get
			{
				return _className;
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="regFile">レジストリデータ</param>
		/// <param name="offset">オフセット</param>
		/// <param name="parentKeyPath">親キーのパス</param>
		public RegistryKey(byte[] regFile, uint offset, string parentKeyPath)
		{
			// 位置把握用
			uint position = offset;

			// Registryキーのヘッダー情報取得
			byte[] nkHeader = null;
			try
			{
				nkHeader = Library.ExtractArrayElements(regFile, position, 0x50);
				position += 0x50;
			}
			catch (ArgumentException ex)
			{
				throw ex;
			}

			// ヘッダー情報分解
			byte[] size = Library.ExtractArrayElements(nkHeader, 0x00, 4);
			string signature = Encoding.ASCII.GetString(Library.ExtractArrayElements(nkHeader, 0x04, 2));
			ushort nodeType = BitConverter.ToUInt16(Library.ExtractArrayElements(nkHeader, 0x06, 2), 0);
			double timestamp = Library.UnpackWindowsTime(Library.ExtractArrayElements(nkHeader, 0x08, 8));
			uint offsetToParent = BitConverter.ToUInt32(Library.ExtractArrayElements(nkHeader, 0x14, 4), 0);
			uint numberOfSubkeys = BitConverter.ToUInt32(Library.ExtractArrayElements(nkHeader, 0x18, 4), 0);
			uint offsetToSubkeyList = BitConverter.ToUInt32(Library.ExtractArrayElements(nkHeader, 0x20, 4), 0);
			uint numberOfValues = BitConverter.ToUInt32(Library.ExtractArrayElements(nkHeader, 0x28, 4), 0);
			uint offsetToValueList = BitConverter.ToUInt32(Library.ExtractArrayElements(nkHeader, 0x2C, 4), 0);
			uint offsetToSecurity = BitConverter.ToUInt32(Library.ExtractArrayElements(nkHeader, 0x30, 4), 0);
			uint offsetToClassName = BitConverter.ToUInt32(Library.ExtractArrayElements(nkHeader, 0x34, 4), 0);
			ushort nameLength = BitConverter.ToUInt16(Library.ExtractArrayElements(nkHeader, 0x4C, 2), 0);
			ushort classNameLength = BitConverter.ToUInt16(Library.ExtractArrayElements(nkHeader, 0x4E, 2), 0);

			// 分解した情報からOFFSET系値を計算
			offsetToParent =
				(0xFFFFFFFF != offsetToParent) ? Constants.OFFSET_BASE + offsetToParent : offsetToParent;
			offsetToSubkeyList =
				(0xFFFFFFFF != offsetToSubkeyList) ? Constants.OFFSET_BASE + offsetToSubkeyList : offsetToSubkeyList;
			offsetToValueList =
				(0xFFFFFFFF != offsetToValueList) ? Constants.OFFSET_BASE + offsetToValueList : offsetToValueList;
			offsetToSecurity =
				(0xFFFFFFFF != offsetToSecurity) ? Constants.OFFSET_BASE + offsetToSecurity : offsetToSecurity;
			offsetToClassName =
				(0xFFFFFFFF != offsetToClassName) ? Constants.OFFSET_BASE + offsetToClassName : offsetToClassName;

			// 一応sign比較？
			if (!Constants.VALID_KEY_SIGNATURE.Equals(signature))
			{
				throw new Exception("nk sign error");
			}

			// この処理をやっている理由は現状不明だが、オリジナルにないのでやめておく
			// これまでは問題なかったが、ShellBag対応の際に1件だけこのチェックに引っかかるが正常なキーが存在したため
			// もしあちこちおかしくなったら戻して対応が必要
			// nodetype比較?
			//if (0x00 != nodeType &&
			//    0x20 != nodeType && 0x2C != nodeType &&
			//    0x30 != nodeType &&
			//    0xA0 != nodeType && 0xAC != nodeType &&
			//    0x1020 != nodeType &&
			//    0x10A0 != nodeType)
			//{

			//    throw new Exception("nodetype error");
			//}

			// name取得
			byte[] nameSource = Library.ExtractArrayElements(regFile, position, nameLength);
			position += nameLength;


			// ASCIIでなければUnicodeで変換
			string name = (Library.IsAscii(nameSource)) ? Encoding.ASCII.GetString(nameSource) : Encoding.Unicode.GetString(nameSource);

			// Parentとがっちゃんこ
			string keyPath =
				(null != parentKeyPath && 0 < parentKeyPath.Length) ? parentKeyPath + "\\" + name : name;

			// class name取得
			string className = string.Empty;
			if (0xFFFFFFFF != offsetToClassName)
			{
				className = Encoding.Unicode.GetString(Library.ExtractArrayElements(regFile, offsetToClassName + 4, classNameLength));
			}

			// 取得した値をAccessorに設定していく
			RegFile = regFile;
			Name = name;
			NameLength = nameLength;
			KeyPath = keyPath;
			NodeType = nodeType;
			OffsetToParent = offsetToParent;
			NumberOfSubkeys = numberOfSubkeys;
			OffsetToSubkeyList = offsetToSubkeyList;
			NumberOfValues = numberOfValues;
			OffsetToValueList = offsetToValueList;
			Timestamp = timestamp;
			;
			OffsetToSecurity = offsetToSecurity;
			OffsetToClassName = offsetToClassName;
			ClassNameLength = classNameLength;
			ClassName = className;
		}

		/// <summary>
		/// サブキーを取得します。
		/// </summary>
		/// <param name="subKeyPath">サブキーのパス</param>
		/// <returns>サブキー</returns>
		public RegistryKey GetSubkey(string subKeyPath)
		{
			if (null == subKeyPath)
			{
				throw new ArgumentException("No subkey name specified for get_subkey");
			}

			string[] pathComponents = subKeyPath.Split('\\');

			RegistryKey key = this;

			foreach (string component in pathComponents)
			{
				RegistryKey subkey = key.LookUpSubkey(component);

				if (null != subkey)
				{
					key = subkey;
				}
				else
				{
					return null;
				}
			}

			return key;
		}

		/// <summary>
		/// 指定された名前のサブキーを検索します
		/// </summary>
		/// <param name="subKeyName">サブキー名</param>
		/// <returns>当該サブキー</returns>
		private RegistryKey LookUpSubkey(string subKeyName)
		{
			foreach (RegistryKey subkey in GetListOfSubkeys())
			{
				if (subKeyName.Equals(subkey.Name, StringComparison.CurrentCultureIgnoreCase))
				{
					return subkey;
				}
			}

			return null;
		}

		/// <summary>
		/// サブキーのリストを取得します
		/// </summary>
		/// <returns>サブキーのリスト</returns>
		public RegistryKey[] GetListOfSubkeys()
		{
			string whereabouts = (null != KeyPath) ?
				" (when enumarating subkeys of " + KeyPath + ")" : "";


			List<RegistryKey> listOfSubkeys = new List<RegistryKey>();
			if (0 < NumberOfSubkeys)
			{
				foreach (uint offsetToSubkey in GetOffsetsToSubkeys())
				{
					try
					{
						RegistryKey subkey = new RegistryKey(RegFile, offsetToSubkey, KeyPath);
						if (null != subkey)
						{
							listOfSubkeys.Add(subkey);
						}
					}
					catch (Exception ex)
					{
						throw ex;
					}
				}
			}

			return listOfSubkeys.ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public uint[] GetOffsetsToSubkeys()
		{
			return GetOffsetsToSubkeys(0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="initialOffset"></param>
		/// <returns></returns>
		public uint[] GetOffsetsToSubkeys(uint initialOffset)
		{
			// position
			uint position = 0;

			// 引数なしはAccessorよりoffset取得
			uint offsetToSubkeyList = (initialOffset > 0) ? initialOffset : OffsetToSubkeyList;

			string whereabouts = (null != KeyPath && !string.Empty.Equals(KeyPath)) ?
				" (when enumarating subkeys of " + KeyPath + ")" : string.Empty;

			byte[] subkeyListHeader = Library.ExtractArrayElements(RegFile, offsetToSubkeyList, 8);
			position = offsetToSubkeyList + 8;

			uint size = BitConverter.ToUInt32(Library.ExtractArrayElements(subkeyListHeader, 0, 4), 0);
			string signature = Encoding.ASCII.GetString(Library.ExtractArrayElements(subkeyListHeader, 4, 2));
			ushort numberOfEntries = BitConverter.ToUInt16(Library.ExtractArrayElements(subkeyListHeader, 6), 0);

			List<uint> offsetsToSubkeys = new List<uint>();

			uint subkeyListLength = 0;
			if ("lf".Equals(signature) || "lh".Equals(signature))
			{
				subkeyListLength = 2 * 4 * (uint)numberOfEntries;
			}
			else if ("ri".Equals(signature) || "li".Equals(signature))
			{
				subkeyListLength = 4 * (uint)numberOfEntries;
			}
			else
			{
				// エラー吐く
				// exception or return null
				Logger.Write(LogLevel.WARNING, "Invalid subkey list signature at 0x" +
					String.Format("{0:X}", offsetToSubkeyList) + whereabouts);
				return new uint[0];

			}

			byte[] subkeyList = Library.ExtractArrayElements(RegFile, position, subkeyListLength);

			if ("lf".Equals(signature) || "lh".Equals(signature))
			{
				uint offset = 0;
				for (uint i = 0; i < numberOfEntries; i++)
				{
					offset = BitConverter.ToUInt32(Library.ExtractArrayElements(subkeyList, 8 * i, 4), 0);
					offsetsToSubkeys.Add(offset + Constants.OFFSET_BASE);
				}
			}
			else if ("ri".Equals(signature))
			{

				List<long> listOfOffset = new List<long>();
				for (uint i = 0; i < numberOfEntries; i++)
				{
					listOfOffset.Add(BitConverter.ToUInt32(Library.ExtractArrayElements(subkeyList, 4 * i, 4), 0));
				}

				foreach (uint offset in listOfOffset)
				{
					uint[] offsets = GetOffsetsToSubkeys(Constants.OFFSET_BASE + offset);

					if (0 < offsets.Length)
					{
						offsetsToSubkeys.AddRange(offsets);
					}
				}
			}
			else if ("li".Equals(signature))
			{
				List<uint> listOfOffset = new List<uint>();
				for (uint i = 0; i < numberOfEntries; i++)
				{
					offsetsToSubkeys.Add(Constants.OFFSET_BASE + BitConverter.ToUInt32(Library.ExtractArrayElements(subkeyList, 4 * i, 4), 0));
				}
			}

			return offsetsToSubkeys.ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public RegistryValue[] GetListOfValues()
		{
			return GetListOfValues(false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="usingDefault"></param>
		/// <returns></returns>
		public RegistryValue[] GetListOfValues(bool usingDefault)
		{
			List<RegistryValue> listOfValues = new List<RegistryValue>();

			if (0 < NumberOfValues)
			{
				foreach (uint offset in GetOffsetsToValues())
				{
					RegistryValue value = new RegistryValue(RegFile, offset, KeyPath);

					if (null != value && null != value.Name)
					{
						if (!(!usingDefault && "(Default)".Equals(value.Name)))
						{
							listOfValues.Add(value);
						}
					}
				}
			}

			return listOfValues.ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public uint[] GetOffsetsToValues()
		{
			List<uint> listOfOffset = new List<uint>();

			// NumberOfValuesが0ならException終了
			if (0 == NumberOfValues)
			{
				throw new Exception("unexpected error: NumberOfValues is zero");
			}

			string whereabouts = (null != KeyPath && !string.Empty.Equals(KeyPath)) ?
				" (when enumarating valuesof " + KeyPath + ")" : "";

			// 「OffsetToValueList + 4」 オリジナルではunpackの際、最初にx4してるのでその分を足してる
			byte[] valueList = Library.ExtractArrayElements(RegFile, OffsetToValueList + 4, 4 + NumberOfValues * 4);

			uint maxCount = Math.Min(NumberOfValues, (uint)(valueList.Length / 4));
			for (uint i = 0; i < maxCount; i++)
			{
				listOfOffset.Add(Constants.OFFSET_BASE + BitConverter.ToUInt32(Library.ExtractArrayElements(valueList, i * 4, 4), 0));
			}

			return listOfOffset.ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string RegeneratePath()
		{

			List<string> keyNameList = new List<string>();
			keyNameList.Add(Name);

			RegistryKey key = this;

			while (key.IsRoot())
			{
				key = key.GetParent();

				if (null != key)
				{
					keyNameList.Clear();
					keyNameList.Add("(Invalid Parent Key)");
					break;
				}

			}

			keyNameList.Reverse();
			StringBuilder keyPathBuilder = new StringBuilder();
			for (int i = 0; i < keyNameList.Count; i++)
			{
				if (0 != i)
				{
					keyPathBuilder.Append("\\");
				}
				keyPathBuilder.Append(keyNameList[i]);
			}

			return keyPathBuilder.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsRoot()
		{
			return (0x2C == NodeType || 0xAC == NodeType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public RegistryKey GetParent()
		{

			if (IsRoot())
			{
				return null;
			}

			string[] keys = KeyPath.Split('\\');
			StringBuilder parentBuilder = new StringBuilder();
			for (int i = 0; i < keys.Length - 1; i++)
			{
				if (0 != i)
				{
					parentBuilder.Append("\\");
				}
				parentBuilder.Append(keys[i]);
			}

			return new RegistryKey(RegFile, (uint)OffsetToParent, parentBuilder.ToString());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string AsString()
		{
			return KeyPath + " [" + GetTimestampAsString(Timestamp) + "]";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="timestamp"></param>
		/// <returns></returns>
		public string GetTimestampAsString(double timestamp)
		{

			if (double.NaN == timestamp)
			{
				return "(undefined)";
			}

			// On Windows, gmtime will return undef if $time < 0 or > 0x7fffffff
			// ビットがひっくり返ってちょい怪しいかも
			if (0 > timestamp || timestamp > 0x7FFFFFFF)
			{
				return "(undefined)";
			}

			DateTime dateTime = new DateTime(1970, 1, 1);
			dateTime = dateTime.AddSeconds(timestamp);

			// The final 'Z' indicates UTC ("zero meridian")
			return dateTime.ToString("u");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public RegistryValue GetValue(string name)
		{
			RegistryValue[] values = GetListOfValues(true);
			foreach (RegistryValue value in values)
			{
				if (name.ToUpper().Equals(value.Name.ToUpper()))
				{
					return value;
				}
			}

			return null;
		}
	}
}