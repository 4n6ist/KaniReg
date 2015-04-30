using System;
using System.Collections.Generic;
using System.Text;

namespace KaniReg
{

	public class RegistryValue
	{

		private const uint OFFSET_TO_FIRST_HBIN = 0x1000;
		private const uint VK_HEADER_LENGTH = 0x18;

		private int[] types = {
            Constants.REG_NONE,
            Constants.REG_SZ,
            Constants.REG_EXPAND_SZ,
            Constants.REG_BINARY,
            Constants.REG_DWORD,
            Constants.REG_DWORD_BIG_ENDIAN,
            Constants.REG_LINK,
            Constants.REG_MULTI_SZ,
            Constants.REG_RESOURCE_LIST,
            Constants.REG_FULL_RESOURCE_DESCRIPTOR,
            Constants.REG_RESOURCE_REQUIREMENTS_LIST,
            Constants.REG_QWORD
        };

		private byte[] _regFile;
		private ulong _offset;
		private ulong _length;
		private bool _allocated;
		private string _signature;
		private string _name;
		private ulong _type;
		private byte[] _data;
		private ulong _dataLength;
		private ulong _dataInline;
		private ulong _offsetToData;
		private bool _isAscii;

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

		public ulong Offset
		{
			set
			{
				_offset = value;
			}
			get
			{
				return _offset;
			}
		}

		public ulong Length
		{
			set
			{
				_length = value;
			}
			get
			{
				return _length;
			}
		}

		public bool Allocated
		{
			set
			{
				_allocated = value;
			}
			get
			{
				return _allocated;
			}
		}

		public string Signature
		{
			set
			{
				_signature = value;
			}
			get
			{
				return _signature;
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

		public ulong Type
		{
			set
			{
				_type = value;
			}
			get
			{
				return _type;
			}
		}

		public byte[] Data
		{
			set
			{
				_data = value;
			}
			get
			{
				return _data;
			}
		}

		public ulong DataLength
		{
			set
			{
				_dataLength = value;
			}
			get
			{
				return _dataLength;
			}
		}

		public ulong DataInline
		{
			set
			{
				_dataInline = value;
			}
			get
			{
				return _dataInline;
			}
		}

		public ulong OffsetToData
		{
			set
			{
				_offsetToData = value;
			}
			get
			{
				return _offsetToData;
			}
		}

		public bool IsAscii
		{
			set
			{
				_isAscii = value;
			}
			get
			{
				return _isAscii;
			}
		}

		public RegistryValue(byte[] regFile, ulong offset, string parentKeyPath)
		{

			/*
				croak "Missing registry file" if !defined $regfile;
				croak "Missing offset" if !defined $offset;
			*/

			// when errors are encountered
			string whereabouts = string.Empty;
			if (null != parentKeyPath && !string.Empty.Equals(parentKeyPath))
			{
				whereabouts = "(a value of " + parentKeyPath + ")";
			}
			/*
						if (0) {
							printf("NEW VALUE at 0x%x%s\n", $offset, $whereabouts);
						}
			*/

			// 0x00 dword = value length (as negative number)
			// 0x04 word  = 'vk' signature
			// 0x06 word  = value name length
			// 0x08 dword = length of data (bit 31 set => data stored inline)
			// 0x0c dword = offset to data/inline data
			// 0x10 dword = type of data
			// 0x14 word  = flag (bit 0 set = name present, unset = default)   <- means Unicode / ASCII?
			// 0x16 word
			// 0x18       = value name [for name length bytes]

			// Extracted offsets are always relative to first HBIN
			byte[] vkHeader;
			if (offset + VK_HEADER_LENGTH <= (ulong)regFile.Length)
			{
				vkHeader = Library.ExtractArrayElements(regFile, offset, VK_HEADER_LENGTH);
			}
			else
			{
				//if (VK_HEADER_LENGTH != vkHeader.Length) {
				//                warnf("Could not read value at 0x%x%s", $offset, $whereabouts);
				return;
				//}

			}

			// 各値を切り取る
			byte[] lengthBytes = Library.ExtractArrayElements(vkHeader, 0, 0x04);
			string signature = Encoding.ASCII.GetString(Library.ExtractArrayElements(vkHeader, 0x04, 2));
			ushort nameLength = BitConverter.ToUInt16(Library.ExtractArrayElements(vkHeader, 0x06, 2), 0);
			uint dataLength = BitConverter.ToUInt32(Library.ExtractArrayElements(vkHeader, 0x08, 4), 0);
			uint offsetToData = BitConverter.ToUInt32(Library.ExtractArrayElements(vkHeader, 0x0C, 4), 0);
			uint type = BitConverter.ToUInt32(Library.ExtractArrayElements(vkHeader, 0x10, 4), 0);
			bool isAscii = (0 < BitConverter.ToUInt16(Library.ExtractArrayElements(vkHeader, 0x14, 4), 0));

			bool allocated = false;
			byte[] standardBytes = { 0x7F, 0xFF, 0xFF, 0xFF };
			int result;
			for (int i = 0; i < lengthBytes.Length; i++)
			{
				result = standardBytes[i].CompareTo(lengthBytes[i]);
				if (0 == result)
				{
					continue;
				}
				else if (result > 0)
				{
					break;
				}
				else
				{
					allocated = true;
					break;
				}
			}

			// Bitがひっくり返ってるのでちょい怪しい感じもする…
			Array.Reverse(lengthBytes);
			uint length = BitConverter.ToUInt32(lengthBytes, 0);
			;
			if (allocated)
			{
				length = (0xFFFFFFFF - length) + 1;
			}

			if (!"vk".Equals(signature))
			{
				// warnf("Invalid signature for value at 0x%x%s", $offset, $whereabouts);
				return;
			}

			string name = string.Empty;
			if (0 != nameLength)
			{
				byte[] namepart = Library.ExtractArrayElements(regFile, offset + 0x18, nameLength);

				if (isAscii)
				{
					name = Encoding.ASCII.GetString(namepart);
				}
				else
				{
					name = Encoding.Unicode.GetString(namepart);
				}
			}
			else if (0 != dataLength)
			{
				// Defaultで取れるように指定しておく
				name = "(Default)";

			}
			else
			{
				// warnf("Could not read name for value at 0x%x%s", $offset, $whereabouts);
				return;
			}

			byte[] data;

			// If the top bit of the data_length is set, then
			// the value is inline and stored in the offset to data field (at 0xc).
			uint dataInline = dataLength >> 31;
			if (0 < dataInline)
			{
				// REG_DWORDs are always inline, but I've also seen
				// REG_SZ, REG_BINARY, REG_EXPAND_SZ, and REG_NONE inline
				dataLength &= 0x7FFFFFFF;

				if (0 == dataLength)
				{
					data = new byte[0];
				}
				else if (4 < dataLength)
				{
					// warnf("Invalid inline data length for value '%s' at 0x%x%s",
					// $name, $offset, $whereabouts);
					data = null;
				}
				else
				{
					// unpack inline data from header
					data = Library.ExtractArrayElements(vkHeader, 0xC, dataLength);
				}
			}
			else
			{
				if ((offsetToData != 0) && (0xFFFFFFFF != offsetToData))
				{
					offsetToData += OFFSET_TO_FIRST_HBIN;
				}

				try
				{
					data = Library.ExtractData(regFile, offsetToData, dataLength);
				}
				catch (IndexOutOfRangeException ex)
				{
					throw ex;
				}
			}

			this.RegFile = regFile;
			this.Offset = offset;
			this.Length = length;
			this.Allocated = allocated;
			this.Signature = signature;
			this.Name = name;
			this.Type = type;
			this.Data = data;
			this.DataLength = dataLength;
			this.DataInline = dataInline;
			this.OffsetToData = offsetToData;
			this.IsAscii = isAscii;
		}

		public string AsString()
		{
			string name = (!string.Empty.Equals(this.Name)) ? this.Name : "(Default)";
			return name + "(" + this.GetTypeAsString() + ") = " + this.GetTypeAsString();
		}



		public string GetTypeAsString()
		{
			if (0 < Array.IndexOf(types, this.Type))
			{
				return types[this.Type].ToString();
			}
			else
			{
				// Return unrecognised types as REG_<number>
				// REGEDIT displays them as formatted hex numbers, e.g. 0x1f4
				return "REG_" + this.Type.ToString();
			}
		}

		public string GetDataAsString()
		{
			object data = this.GetDataAsObject();

			if (null == data)
			{
				// return "(invalid data)";
				return string.Empty;
			}
			else if (Constants.REG_SZ == this.Type || Constants.REG_EXPAND_SZ == this.Type)
			{
				return (string)data;
			}
			else if (Constants.REG_MULTI_SZ == this.Type)
			{
				StringBuilder resultBuilder = new StringBuilder();
				foreach (string datum in (string[])data)
				{
					if (0 < resultBuilder.Length)
					{
						resultBuilder.Append(" ");
					}
					resultBuilder.Append(datum);
				}

				return resultBuilder.ToString();
			}
			else if (Constants.REG_DWORD == this.Type)
			{
				uint transratedData = BitConverter.ToUInt32(this.Data, 0);
				return "0x" + string.Format("{0,0:X8}", transratedData) + " " + transratedData.ToString();
			}
			else
			{
				StringBuilder resultBuilder = new StringBuilder();
				foreach (byte datum in this.Data)
				{
					if (0 < resultBuilder.Length)
					{
						resultBuilder.Append(" ");
					}
					resultBuilder.Append(string.Format("{0:X}", datum));
				}

				return resultBuilder.ToString();
			}
		}

		public object GetDataAsObject()
		{
			object data = this.Data;
			if (Constants.REG_DWORD == this.Type)
			{
				if (4 == this.Data.Length)
				{
					data = BitConverter.ToUInt32(this.Data, 0);
				}
			}
			else if (Constants.REG_SZ == this.Type || Constants.REG_EXPAND_SZ == this.Type)
			{
				// Unicode対応で変換（本家モジュールは恐ろしいことに切り捨てていた･･･）
				// 一応末尾Trim、ヤバかったら消す
				if (!Library.IsAscii(this.Data))
				{
					data = Encoding.Unicode.GetString(this.Data).TrimEnd('\x00');
				}
				else
				{
					data = Encoding.ASCII.GetString(this.Data).TrimEnd('\x00');
				}
			}
			else if (Constants.REG_MULTI_SZ == this.Type)
			{
				List<string> multiList = new List<string>();
				List<byte> extracted = new List<byte>();

				List<byte[]> splitted = Library.SplitByteData(this.Data);

				foreach (byte[] bytes in splitted)
				{
					// Unicodeで変換、リストに追加
					if (0 < bytes.Length)
					{
						multiList.Add(Encoding.Unicode.GetString(bytes));
					}
				}
				// wantarrayがよー分らんので、配列返しちゃる
				data = multiList.ToArray();
			}

			return data;
		}
	}
}