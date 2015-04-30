using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KaniReg
{
	/// <summary>
	/// 
	/// </summary>
	class MruValues
	{
		/// <summary>
		/// 
		/// </summary>
		private const int HEADER_SIZE = 45;

		public enum SortType
		{
			Alphabet,
			ListOrder
		}

		public enum MruType
		{
			Explorer,
			Libraries,
			Users,
			Public,
			My_Documents,
			My_Computer,
			My_Network,
			Recycle_Bin,
			Control_Panel,
			Game
		}

		public static void Parse(RegistryKey key, Reporter reporter, Logger logger, bool alphabet = false)
		{
			string checkedValue = string.Empty;

			// Retrieve values and load into a hash for sorting            
			RegistryValue[] values = key.GetListOfValues();

			if (null != values && 0 < values.Length)
			{
				List<int> mruList = new List<int>();
				List<byte> fileList = new List<byte>();

				List<KeyValuePair<string, string>> resultSet = new List<KeyValuePair<string, string>>();

				foreach (RegistryValue value in values)
				{
					if (value.Name.StartsWith("MRUList"))
					{
						byte[] data = value.Data;
						int mru = -1; // MRUはゼロもあり得るので-1を与える

						if (!alphabet && !value.Name.Contains("Ex"))
						{
							// MRUListExから値のソート順を取り出す
							for (int count = 0; data.Length > count; count += 4)
							{
								mru = BitConverter.ToInt32(data, count);
								if (0 <= mru) // ０をスキップしない
								{
									mruList.Add(mru);
								}
							}
						}
						else
						{
							for (int count = 0; data.Length > count; count += 2)
							{
								mru = BitConverter.ToInt16(data, count);
								if (0 <= mru) // ゼロをスキップしない
								{
									mruList.Add(mru);
								}
							}
						}
						continue;
					}
					else
					{
						string fileName = string.Empty;
						string directoryName = string.Empty;
						List<byte> list = new List<byte>();
						for (int count = 0; value.Data.Length > count; count += 2)
						{
							short item = BitConverter.ToInt16(value.Data, count);
							if (0x00 == item)
							{
								if (string.IsNullOrEmpty(fileName))
								{
									fileName = Encoding.Unicode.GetString(list.ToArray());
									list.Clear();
								}
								else if (string.IsNullOrEmpty(directoryName))
								{
									directoryName = Encoding.Unicode.GetString(list.ToArray());
									list.Clear();
								}
							}
							else
							{
								list.Add(value.Data[count]);
								list.Add(value.Data[count + 1]);
							}
						}

						if (0 < directoryName.Length)
						{
							checkedValue = directoryName + @"\" + fileName;
						}
						else
						{
							checkedValue = fileName;
						}
						resultSet.Add(new KeyValuePair<string, string>(value.Name, checkedValue));
					}
				}

				Write(mruList, reporter, logger, resultSet, alphabet);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="reporter"></param>
		/// <param name="logger"></param>
		public static void ParseEx(RegistryKey key, Reporter reporter, Logger logger)
		{
			RegistryValue[] values = key.GetListOfValues();

			if (null != values && 0 < values.Length)
			{
				List<KeyValuePair<string, string>> resultSet = new List<KeyValuePair<string, string>>();

				// writer対象文字列
				string checkedValue = string.Empty;
				// writer対象文字列
				List<int> mruList = new List<int>();
				// dir/fileバイト値格納
				List<byte> fileList = new List<byte>();
				List<byte> dirList = new List<byte>();

				// Retrieve values and load into a hash for sorting            
				foreach (RegistryValue value in values)
				{
					// Print sorted content to report file            
					if (value.Name.StartsWith("MRUList"))
					{
						byte[] data = value.Data;
						int mru = -1; // MRUはゼロもあり得るので-1を与える

						for (int count = 0; data.Length > count; count += 4)
						{
							mru = BitConverter.ToInt32(data, count);
							if (0 <= mru) // ０をスキップしない
							{
								mruList.Add(mru);
							}
						}

						continue;
					}
					else if (null != value.Name)
					{
						if (Constants.REG_BINARY != value.Type)
						{
							checkedValue = value.GetDataAsString();
						}
						else
						{
							MemoryStream stream = new MemoryStream(value.Data);

							string fileName = string.Empty;
							byte[] buffer = new byte[2];
							string drive = string.Empty;

							if (key.Name.StartsWith("LastVisited") ||
								key.Name.StartsWith("CIDSize")) // CIDの場合もファイル名を拾う
							{
								// get file name
								List<byte> byteList = new List<byte>();
								do
								{
									stream.Read(buffer, 0, 2);
									byteList.AddRange(buffer);
								} while (0 < BitConverter.ToInt16(buffer, 0));

								byteList.RemoveRange(byteList.Count - 2, 2);
								fileName = Encoding.Unicode.GetString(byteList.ToArray());

								if (key.Name.StartsWith("CIDSize"))
								{
									checkedValue = fileName;
								}
							}

							if (key.Name.StartsWith("FirstFolder")) // FirstFolderの場合もファイル名を拾う
							{
								fileName = Encoding.Unicode.GetString(value.Data);
								checkedValue = fileName.Replace("\0", " ").TrimEnd();
							}

							stream.Read(buffer, 0, 2);
							short headerSize = BitConverter.ToInt16(buffer, 0);
							stream.Seek(-2, SeekOrigin.Current);

							if (0x14 == headerSize)
							{
								byte[] headerBytes = new byte[headerSize];
								stream.Read(headerBytes, 0, headerSize);
								MruType mruType = GetType(headerBytes);

								// get drive block size
								stream.Read(buffer, 0, 2);
								int driveSize = BitConverter.ToInt16(buffer, 0);
								byte[] driveBlock = new byte[driveSize - 2];
								stream.Read(driveBlock, 0, driveSize - 2);

								// determin if block has drive letter
								if (MruType.My_Computer.Equals(mruType))
								{
									// パスの先頭に出身地を追加
									drive = string.Format("{0}\\",
										Enum.GetName(typeof(MruType), MruType.My_Computer));

									drive += Encoding.ASCII.GetString(new byte[] { driveBlock[1], driveBlock[2], driveBlock[3] });

									// Get child paths
									try
									{
										checkedValue = drive + GetPath(ref stream, logger);
									}
									catch (Exception exception)
									{
										logger.Write(LogLevel.WARNING, key.Name + "において発生:" + exception.Message);
									}
								}
								else if (MruType.My_Network.Equals(mruType))
								{
									// whole skip driveblock

									// パスの先頭に出身地を追加
									drive = string.Format("{0}\\",
										Enum.GetName(typeof(MruType), MruType.My_Network));

									// get next block
									stream.Read(buffer, 0, 2);
									short networkSize = BitConverter.ToInt16(buffer, 0);
									byte[] networkBytes = new byte[networkSize - 2];
									stream.Read(networkBytes, 0, networkSize - 2);

									List<byte> list = new List<byte>();
									bool inData = false;
									foreach (byte item in networkBytes)
									{
										if (!inData && 0x5C == item)
										{
											inData = true;
										}
										else if (inData && 0x00 == item)
										{
											inData = false;
										}
										if (inData)
										{
											list.Add(item);
										}
									}
									drive += Encoding.ASCII.GetString(list.ToArray()) + "\\";

									// Get child paths
									try
									{
										checkedValue = drive + GetPath(ref stream, logger);
									}
									catch (Exception exception)
									{
										logger.Write(LogLevel.WARNING, key.Name + "において発生:" + exception.Message);
									}
								}
								else if (MruType.Libraries.Equals(mruType))
								{
									// whole skip driveblock

									// パスの先頭に出身地を追加
									drive = string.Format("{0}\\",
										Enum.GetName(typeof(MruType), MruType.Libraries));

									// Get child paths
									stream.Read(buffer, 0, 2);
									short size = BitConverter.ToInt16(buffer, 0);
									stream.Seek(-2, SeekOrigin.Current);
									byte[] block = new byte[size];

									try
									{
										checkedValue = drive + GetPathFromBlock(block);
									}
									catch (Exception exception)
									{
										logger.Write(LogLevel.WARNING, key.Name + "において発生:" + exception.Message);
									}
								}
							}
							else
							{
								try
								{
									// OpenSaveとLastVisitedのときだけパスを取得する
									if (key.Name.StartsWith("OpenSave") || key.Name.StartsWith("LastVisited"))
									{
										checkedValue = GetPath(ref stream, logger);
									}
								}
								catch (Exception exception)
								{
									logger.Write(LogLevel.WARNING, key.Name + "において発生:" + exception.Message);
								}
							}
						}

						resultSet.Add(new KeyValuePair<string, string>(value.Name, checkedValue));
					}
				}

				Write(mruList, reporter, logger, resultSet);

			}
			else
			{
				Library.WriteNoValue(key.KeyPath, reporter);
			}
		}


		private static void Write(List<int> mruList, Reporter reporter, Logger logger, List<KeyValuePair<string, string>> resultSet, bool alphabet = false)
		{
			if (null != mruList)
			{

				StringBuilder builder = new StringBuilder();
				foreach (int mru in mruList)
				{
					if (0 < builder.Length)
					{
						builder.Append(" ");
					}
					if (!alphabet)
					{
						builder.Append(mru.ToString());
					}
					else
					{
						builder.Append((char)mru);
					}
				}

				reporter.Write("  MRUList = " + builder.ToString());

				if (0 < mruList.Count && mruList.Count > resultSet.Count)
				{
					reporter.Write("  mruListに対してのValueのリストを正しく取得できませんでした。logを確認してください。");
				}

				List<KeyValuePair<string, string>> sorted = new List<KeyValuePair<string, string>>();
				string tagString = string.Empty;

				foreach (int mru in mruList)
				{
					if (!alphabet)
					{
						tagString = mru.ToString();
					}
					else
					{
						tagString = ((char)mru).ToString();
					}
					foreach (KeyValuePair<string, string> pair in resultSet)
					{
						if (tagString.Equals(pair.Key))
						{
							sorted.Add(pair);
						}
					}
				}
				// 戻す
				resultSet = sorted;

				// 出力
				foreach (KeyValuePair<string, string> pair in resultSet)
				{
					reporter.Write("    " + pair.Key + " => " + pair.Value);
				}
			}
		}

		private static MruType GetType(byte[] header)
		{
			MruType result = MruType.My_Computer;
			switch (header[3])
			{
				case 0x00:
				case 0x68:
					result = MruType.Explorer;
					break;
				case 0x42:
					result = MruType.Libraries;
					break;
				case 0x44:
					result = MruType.Users;
					break;
				case 0x4C:
					result = MruType.Public;
					break;
				case 0x48:
					result = MruType.My_Documents;
					break;
				case 0x50:
					result = MruType.My_Computer;
					break;
				case 0x58:
					result = MruType.My_Network;
					break;
				case 0x60:
				case 0x78:
					result = MruType.Recycle_Bin;
					break;
				case 0x70:
					result = MruType.Control_Panel;
					break;
				case 0x80:
					result = MruType.Game;
					break;
			}

			return result;
		}

		private static string GetPath(ref MemoryStream stream, Logger logger)
		{
			byte[] buffer = new byte[2];
			StringBuilder pathBuilder = new StringBuilder();

			while (true)
			{
				stream.Read(buffer, 0, 2);
				int blockSize = BitConverter.ToInt16(buffer, 0);
				if (0 == blockSize)
					break;

				if (stream.Length - stream.Position < blockSize)
				{
					if (0 < pathBuilder.Length)
					{
						pathBuilder.Append(@"\");
					}
					pathBuilder.Append("---N/A---");
					throw new Exception("Valueに割り当てられたサイズを超えるブロックサイズが指定されています。");
				}
				else if (0 > blockSize)
				{
					if (0 < pathBuilder.Length)
					{
						pathBuilder.Append(@"\");
					}
					pathBuilder.Append("---N/A---");
					throw new Exception("Valueのサイズがマイナスでした。");
				}

				byte[] block = new byte[blockSize];
				stream.Seek(-2, SeekOrigin.Current);
				stream.Read(block, 0, blockSize);

				if (0 < pathBuilder.Length)
				{
					pathBuilder.Append(@"\");
				}
				pathBuilder.Append(GetPathFromBlock(block));
			}

			return pathBuilder.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="block"></param>
		/// <returns></returns>
		private static string GetPathFromBlock(byte[] block)
		{

			List<byte> list = new List<byte>();

			short value = -1;
			int position = block.Length - 5;
			while (true)
			{
				byte[] bytes = new byte[] { block[position - 1], block[position] };
				value = BitConverter.ToInt16(bytes, 0);
				if (0 == value)
				{
					try
					{
						bytes = new byte[] { block[position - 3], block[position - 2] };
						value = BitConverter.ToInt16(bytes, 0);
					}
					catch
					{
						// noop
					}

					if (0 == value)
					{
						break;
					}
					else
					{
						list.Add(0x00);
						list.Add(0x21);
					}
				}
				else
				{
					list.Add(block[position]);
					list.Add(block[position - 1]);
				}
				position -= 2;
				if (2 > position)
					break;
			}

			list.Reverse();

			string name = Encoding.Unicode.GetString(list.ToArray());
			if (name.Contains("!"))
			{
				string[] strings = name.Split('!');
				name = strings[0];
			}

			return name;
		}
	}
}