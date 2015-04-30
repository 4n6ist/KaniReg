using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace KaniReg.UsrClass
{
	/// <summary>
	/// Based on shellbags v.20130102
	/// </summary>
	/// <remarks>
	/// 仕様が一致するものはパース出来ているが、オリジナルはXPを捨ててるようにみえる
	/// </remarks>
	class ShellBag : ClassBase
	{
		#region Constants

		/// <summary>
		/// mtime_str
		/// </summary>
		private const string MTS = "mtime_str";

		/// <summary>
		/// atime_str
		/// </summary>
		private const string ATS = "atime_str";

		/// <summary>
		/// ctime_str
		/// </summary>
		private const string CTS = "ctime_str";

		/// <summary>
		/// datetime
		/// </summary>
		private const string DT = "datetime";

		/// <summary>
		/// path
		/// </summary>
		private const string PATH = "path";

		/// <summary>
		/// name
		/// </summary>
		private const string NAME = "name";

		/// <summary>
		/// type
		/// </summary>
		private const string TYPE = "type";

		#endregion

		#region Member

		/// <summary>
		/// GUID dictionary
		/// </summary>
		private Dictionary<string, string> _cpGuids = new Dictionary<string, string>();

		/// <summary>
		/// Folder type dictionary
		/// </summary>
		private Dictionary<string, string> _folderTypes = new Dictionary<string, string>();

		/// <summary>
		/// WinXP のレジストリっぽいですよフラグ
		/// </summary>
		private bool _isXp = false;

		#endregion

		public ShellBag(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger)
			: base(rootKey, timeZoneBias, outputUtc, reporter, logger)
		{
			// 他を触らないでルートからキーを取り直すための苦肉の策
			if (this.Key == null)
			{
				KeyPath = @"Software\Microsoft\Windows\ShellNoRoam\BagMRU";
				this.Key = this.RootKey.GetSubkey(KeyPath);

				if (null != Key)
				{
					Reporter.Write("XPのキーを検索");
					Reporter.Write("キーのパス：" + KeyPath);

					if (PrintKeyInBase)
					{
						Reporter.Write("キーの最終更新日時：" + Library.TransrateTimestamp(Key.Timestamp, TimeZoneBias, OutputUtc));
						Reporter.Write("");
					}

					this._isXp = true;
				}
			}
		}

		protected override void Initialize()
		{
			PrintKeyInBase = true;
			KeyPath = @"Local Settings\Software\Microsoft\Windows\Shell\BagMRU";
			Description = "フォルダ毎の表示位置 / サイズ等の設定";

			#region Create GUID and FolderType dictionary

			this._cpGuids.Add("{bb64f8a7-bee7-4e1a-ab8d-7d8273f7fdb6}", "Action Center");
			this._cpGuids.Add("{7a979262-40ce-46ff-aeee-7884ac3b6136}", "Add Hardware");
			this._cpGuids.Add("{d20ea4e1-3957-11d2-a40b-0c5020524153}", "Administrative Tools");
			this._cpGuids.Add("{9c60de1e-e5fc-40f4-a487-460851a8d915}", "AutoPlay");
			this._cpGuids.Add("{b98a2bea-7d42-4558-8bd1-832f41bac6fd}", "Backup and Restore Center");
			this._cpGuids.Add("{0142e4d0-fb7a-11dc-ba4a-000ffe7ab428}", "Biometric Devices");
			this._cpGuids.Add("{d9ef8727-cac2-4e60-809e-86f80a666c91}", "BitLocker Drive Encryption");
			this._cpGuids.Add("{b2c761c6-29bc-4f19-9251-e6195265baf1}", "Color Management");
			this._cpGuids.Add("{1206f5f1-0569-412c-8fec-3204630dfb70}", "Credential Manager");
			this._cpGuids.Add("{e2e7934b-dce5-43c4-9576-7fe4f75e7480}", "Date and Time");
			this._cpGuids.Add("{00c6d95f-329c-409a-81d7-c46c66ea7f33}", "Default Location");
			this._cpGuids.Add("{17cd9488-1228-4b2f-88ce-4298e93e0966}", "Default Programs");
			this._cpGuids.Add("{37efd44d-ef8d-41b1-940d-96973a50e9e0}", "Desktop Gadgets");
			this._cpGuids.Add("{74246bfc-4c96-11d0-abef-0020af6b0b7a}", "Device Manager");
			this._cpGuids.Add("{a8a91a66-3a7d-4424-8d24-04e180695c7a}", "Devices and Printers");
			this._cpGuids.Add("{c555438b-3c23-4769-a71f-b6d3d9b6053a}", "Display");
			this._cpGuids.Add("{d555645e-d4f8-4c29-a827-d93c859c4f2a}", "Ease of Access Center");
			this._cpGuids.Add("{6dfd7c5c-2451-11d3-a299-00c04f8ef6af}", "Folder Options");
			this._cpGuids.Add("{93412589-74d4-4e4e-ad0e-e0cb621440fd}", "Fonts");
			this._cpGuids.Add("{259ef4b1-e6c9-4176-b574-481532c9bce8}", "Game Controllers");
			this._cpGuids.Add("{15eae92e-f17a-4431-9f28-805e482dafd4}", "Get Programs");
			this._cpGuids.Add("{cb1b7f8c-c50a-4176-b604-9e24dee8d4d1}", "Getting Started");
			this._cpGuids.Add("{67ca7650-96e6-4fdd-bb43-a8e774f73a57}", "HomeGroup");
			this._cpGuids.Add("{87d66a43-7b11-4a28-9811-c86ee395acf7}", "Indexing Options");
			this._cpGuids.Add("{a0275511-0e86-4eca-97c2-ecd8f1221d08}", "Infrared");
			this._cpGuids.Add("{a3dd4f92-658a-410f-84fd-6fbbbef2fffe}", "Internet Options");
			this._cpGuids.Add("{a304259d-52b8-4526-8b1a-a1d6cecc8243}", "iSCSI Initiator");
			this._cpGuids.Add("{725be8f7-668e-4c7b-8f90-46bdb0936430}", "Keyboard");
			this._cpGuids.Add("{e9950154-c418-419e-a90a-20c5287ae24b}", "Location and Other Sensors");
			this._cpGuids.Add("{1fa9085f-25a2-489b-85d4-86326eedcd87}", "Manage Wireless Networks");
			this._cpGuids.Add("{6c8eec18-8d75-41b2-a177-8831d59d2d50}", "Mouse");
			this._cpGuids.Add("{7007acc7-3202-11d1-aad2-00805fc1270e}", "Network Connections");
			this._cpGuids.Add("{8e908fc9-becc-40f6-915b-f4ca0e70d03d}", "Network and Sharing Center");
			this._cpGuids.Add("{05d7b0f4-2121-4eff-bf6b-ed3f69b894d9}", "Notification Area Icons");
			this._cpGuids.Add("{d24f75aa-4f2b-4d07-a3c4-469b3d9030c4}", "Offline Files");
			this._cpGuids.Add("{96ae8d84-a250-4520-95a5-a47a7e3c548b}", "Parental Controls");
			this._cpGuids.Add("{f82df8f7-8b9f-442e-a48c-818ea735ff9b}", "Pen and Input Devices");
			this._cpGuids.Add("{5224f545-a443-4859-ba23-7b5a95bdc8ef}", "People Near Me");
			this._cpGuids.Add("{78f3955e-3b90-4184-bd14-5397c15f1efc}", "Performance Information and Tools");
			this._cpGuids.Add("{ed834ed6-4b5a-4bfe-8f11-a626dcb6a921}", "Personalization");
			this._cpGuids.Add("{40419485-c444-4567-851a-2dd7bfa1684d}", "Phone and Modem");
			this._cpGuids.Add("{025a5937-a6be-4686-a844-36fe4bec8b6d}", "Power Options");
			this._cpGuids.Add("{2227a280-3aea-1069-a2de-08002b30309d}", "Printers");
			this._cpGuids.Add("{fcfeecae-ee1b-4849-ae50-685dcf7717ec}", "Problem Reports and Solutions");
			this._cpGuids.Add("{7b81be6a-ce2b-4676-a29e-eb907a5126c5}", "Programs and Features");
			this._cpGuids.Add("{9fe63afd-59cf-4419-9775-abcc3849f861}", "Recovery");
			this._cpGuids.Add("{62d8ed13-c9d0-4ce8-a914-47dd628fb1b0}", "Regional and Language Options");
			this._cpGuids.Add("{241d7c96-f8bf-4f85-b01f-e2b043341a4b}", "RemoteApp and Desktop Connections");
			this._cpGuids.Add("{00f2886f-cd64-4fc9-8ec5-30ef6cdbe8c3}", "Scanners and Cameras");
			this._cpGuids.Add("{f2ddfc82-8f12-4cdd-b7dc-d4fe1425aa4d}", "Sound");
			this._cpGuids.Add("{58e3c745-d971-4081-9034-86e34b30836a}", "Speech Recognition Options");
			this._cpGuids.Add("{9c73f5e5-7ae7-4e32-a8e8-8d23b85255bf}", "Sync Center");
			this._cpGuids.Add("{bb06c0e4-d293-4f75-8a90-cb05b6477eee}", "System");
			this._cpGuids.Add("{80f3f1d5-feca-45f3-bc32-752c152e456e}", "Tablet PC Settings");
			this._cpGuids.Add("{0df44eaa-ff21-4412-828e-260a8728e7f1}", "Taskbar and Start Menu");
			this._cpGuids.Add("{d17d1d6d-cc3f-4815-8fe3-607e7d5d10b3}", "Text to Speech");
			this._cpGuids.Add("{c58c4893-3be0-4b45-abb5-a63e4b8c8651}", "Troubleshooting");
			this._cpGuids.Add("{60632754-c523-4b62-b45c-4172da012619}", "User Accounts");
			this._cpGuids.Add("{be122a0e-4503-11da-8bde-f66bad1e3f3a}", "Windows Anytime Upgrade");
			this._cpGuids.Add("{78cb147a-98ea-4aa6-b0df-c8681f69341c}", "Windows CardSpace");
			this._cpGuids.Add("{d8559eb9-20c0-410e-beda-7ed416aecc2a}", "Windows Defender");
			this._cpGuids.Add("{4026492f-2f69-46b8-b9bf-5654fc07e423}", "Windows Firewall");
			this._cpGuids.Add("{3e7efb4c-faf1-453d-89eb-56026875ef90}", "Windows Marketplace");
			this._cpGuids.Add("{5ea4f148-308c-46d7-98a9-49041b1dd468}", "Windows Mobility Center");
			this._cpGuids.Add("{087da31b-0dd3-4537-8e23-64a18591f88b}", "Windows Security Center");
			this._cpGuids.Add("{e95a4861-d57a-4be1-ad0f-35267e261739}", "Windows SideShow");
			this._cpGuids.Add("{36eef7db-88ad-4e81-ad49-0e313f0c35f8}", "Windows Update");

			this._folderTypes.Add("{724ef170-a42d-4fef-9f26-b60e846fba4f}", "Administrative Tools");
			this._folderTypes.Add("{d0384e7d-bac3-4797-8f14-cba229b392b5}", "Common Administrative Tools");
			this._folderTypes.Add("{de974d24-d9c6-4d3e-bf91-f4455120b917}", "Common Files");
			this._folderTypes.Add("{c1bae2d0-10df-4334-bedd-7aa20b227a9d}", "Common OEM Links");
			this._folderTypes.Add("{5399e694-6ce5-4d6c-8fce-1d8870fdcba0}", "Control Panel");
			this._folderTypes.Add("{1ac14e77-02e7-4e5d-b744-2eb1ae5198b7}", "CSIDL_SYSTEM");
			this._folderTypes.Add("{b4bfcc3a-db2c-424c-b029-7fe99a87c641}", "Desktop");
			this._folderTypes.Add("{7b0db17d-9cd2-4a93-9733-46cc89022e7c}", "Documents Library");
			this._folderTypes.Add("{fdd39ad0-238f-46af-adb4-6c85480369c7}", "Documents");
			this._folderTypes.Add("{374de290-123f-4565-9164-39c4925e467b}", "Downloads");
			this._folderTypes.Add("{de61d971-5ebc-4f02-a3a9-6c82895e5c04}", "Get Programs");
			this._folderTypes.Add("{a305ce99-f527-492b-8b1a-7e76fa98d6e4}", "Installed Updates");
			this._folderTypes.Add("{871c5380-42a0-1069-a2ea-08002b30309d}", "Internet Explorer (Homepage)");
			this._folderTypes.Add("{031e4825-7b94-4dc3-b131-e946b44c8dd5}", "Libraries");
			this._folderTypes.Add("{4bd8d571-6d19-48d3-be97-422220080e43}", "Music");
			this._folderTypes.Add("{20d04fe0-3aea-1069-a2d8-08002b30309d}", "My Computer");
			this._folderTypes.Add("{450d8fba-ad25-11d0-98a8-0800361b1103}", "My Documents");
			this._folderTypes.Add("{ed228fdf-9ea8-4870-83b1-96b02cfe0d52}", "My Games");
			this._folderTypes.Add("{208d2c60-3aea-1069-a2d7-08002b30309d}", "My Network Places");
			this._folderTypes.Add("{f02c1a0d-be21-4350-88b0-7367fc96ef3c}", "Network");
			this._folderTypes.Add("{33e28130-4e1e-4676-835a-98395c3bc3bb}", "Pictures");
			this._folderTypes.Add("{a990ae9f-a03b-4e80-94bc-9912d7504104}", "Pictures");
			this._folderTypes.Add("{7c5a40ef-a0fb-4bfc-874a-c0f2e0b9fa8e}", "Program Files (x86)");
			this._folderTypes.Add("{905e63b6-c1bf-494e-b29c-65b732d3d21a}", "Program Files");
			this._folderTypes.Add("{df7266ac-9274-4867-8d55-3bd661de872d}", "Programs and Features");
			this._folderTypes.Add("{3214fab5-9757-4298-bb61-92a9deaa44ff}", "Public Music");
			this._folderTypes.Add("{b6ebfb86-6907-413c-9af7-4fc2abf07cc5}", "Public Pictures");
			this._folderTypes.Add("{2400183a-6185-49fb-a2d8-4a392a602ba3}", "Public Videos");
			this._folderTypes.Add("{4336a54d-38b-4685-ab02-99bb52d3fb8b}", "Public");
			this._folderTypes.Add("{491e922f-5643-4af4-a7eb-4e7a138d8174}", "Public");
			this._folderTypes.Add("{dfdf76a2-c82a-4d63-906a-5644ac457385}", "Public");
			this._folderTypes.Add("{645ff040-5081-101b-9f08-00aa002f954e}", "Recycle Bin");
			this._folderTypes.Add("{d65231b0-b2f1-4857-a4ce-a8e7c6ea7d27}", "System32 (x86)");
			this._folderTypes.Add("{9e52ab10-f80d-49df-acb8-4330f5687855}", "Temporary Burn Folder");
			this._folderTypes.Add("{f3ce0f7c-4901-4acc-8648-d5d44b04ef8f}", "Users Files");
			this._folderTypes.Add("{59031a47-3f72-44a7-89c5-5595fe6b30ee}", "Users");
			this._folderTypes.Add("{f38bf404-1d43-42f2-9305-67de0b28fc23}", "Windows");

			#endregion
		}

		public override bool Process()
		{
			// Output Header
			Reporter.Write("");
			Reporter.Write(this.GetRecordText(
				"MRU Time", "Modified", "Accessed", "Created", "Zip_Subfolder", "Resource"));
			string delim = "--------------------";
			Reporter.Write(this.GetRecordText(delim, delim, delim, delim, delim, delim));

			var item = new Dictionary<string, string>();

			item.Add(PATH, @"Desktop\");
			item.Add(NAME, "");

			this.Traverse(Key, ref item);
			
			return true;
		}

		/// <summary>
		/// Get an formatted output line
		/// </summary>
		/// <param name="MRU_Time">yyyy/mm/dd hh:mm:ss</param>
		/// <param name="Modified">yyyy/mm/dd hh:mm:ss</param>
		/// <param name="Accessed">yyyy/mm/dd hh:mm:ss</param>
		/// <param name="Created">yyyy/mm/dd hh:mm:ss</param>
		/// <param name="Zip_Subfolder">path format</param>
		/// <param name="Resource">path format</param>
		/// <returns>Text devided by pipe</returns>
		private string GetRecordText(string MRU_Time, string Modified, string Accessed,
									 string Created, string Zip_Subfolder, string Resource)
		{
			// %-20s →　Fixed 20 letters left-arign. In .Net, just only -20. In Japanese, 25 letters required.
			string format = "{0, -25} | {1, -25} | {2, -25} | {3, -25} | {4, -25} | {5}";

			object[] param = new object[] { MRU_Time, Modified, Accessed, Created, Zip_Subfolder, Resource };

			return string.Format(format, param);
		}

		/// <summary>
		/// Parse ShellBags recursively
		/// </summary>
		/// <param name="pKey">Registry key</param>
		/// <param name="pItem">Key value list (call by reference)</param>
		private void Traverse(RegistryKey pKey, ref Dictionary<string, string> pItem)
		{
			if (pKey == null)
			{
				return;
			}

			// RegRipper sorts this list manually
			var values = new SortedList<string, byte[]>();

			var item = new Dictionary<string, string>();

			foreach (RegistryValue rv in pKey.GetListOfValues())
			{
				values.Add(rv.Name, rv.Data);
			}

			values.Remove("NodeSlot");

			uint mru = 0;
			if (values.ContainsKey("MRUListEx"))
			{
				mru = BitConverter.ToUInt32(values["MRUListEx"], 0);
			}

			values.Remove("MRUListEx");

			try
			{
				foreach (string key in values.Keys)
				{
					int num = 0;
					if (!int.TryParse(key, out num))
					{
						continue;
					}

					int type = Convert.ToInt32(values[key][2]); // Get type as decimal...

					// Need to first check to see if the parent of the item was a zip folder
					// and if the 'zipsubfolder' value is set to 1
					if (pItem.ContainsKey("zipsubfolder") && pItem["zipsubfolder"] == "1")
					{
						item = ParseZipSubFolderItem(values[key]);
						item["zipsubfolder"] = "1";
					}
					else if (type == 0x00) // type is decimal but... I implanted original code faithfully...
					{
						// Variable/Property Sheet
						item = ParseVariableEntry(values[key]);
					}
					else if (type == 0x01)
					{
						item = Parse01ShellItem(values[key]);
					}
					else if (type == 0x1F)
					{
						// System Folder
						item = ParseSystemFolderEntry(values[key]);
					}
					else if (type == 0x2E)
					{
						// Device
						item = ParseDeviceEntry(values[key]);
					}
					else if (type == 0x2F)
					{
						// Volume (Drive Letter)
						item = ParseDriveEntry(values[key]);
					}
					else if ((type == 0xC3) || (type == 0x41) ||
							 (type == 0x42) || (type == 0x46) || (type == 0x47))
					{
						// Network stuff
						// RegRipper uses if statements but finally calling same method with same parameter
						item = ParseNetworkEntry(values[key]);
					}
					else if ((type == 0x31) || (type == 0x32) || (type == 0xB1) || (type == 0x74))
					{
						// Folder or Zip File
						item = ParseFolderEntry(values[key]);
					}
					else if (type == 0x35)
					{
						item = ParseFolderEntry2(values[key]);
					}
					else if (type == 0x71)
					{
						// Control Panel
						item = ParseControlPanelEntry(values[key]);
					}
					else if (type == 0x61)
					{
						// URI type
						item = ParseURIEntry(values[key]);
					}
					else
					{
						// Unknown type    convert decimal type to hex string
						if (item.ContainsKey(NAME))
						{
							item[NAME] = string.Format("Unknown Type (0x{0})", type.ToString("X2"));
						}
						else
						{
							item.Add(NAME, string.Format("Unknown Type (0x{0})", type.ToString("X2")));
						}
					}

					if (item[NAME].ToLower().EndsWith(@".zip") && (type == 0x32))
					{
						item["zipsubfolder"] = "1";
					}

					if ((mru != 4294967295) && (key == mru.ToString()))
					{
						if (item.ContainsKey("mrutime"))
						{
							item["mrutime"] = pKey.Timestamp.ToString();
						}
						else
						{
							item.Add("mrutime", pKey.Timestamp.ToString());
						}

						if (item.ContainsKey("mrutime_str"))
						{
							item["mrutime_str"] =
								Library.TransrateTimestamp(pKey.Timestamp, TimeZoneBias, OutputUtc);
						}
						else
						{
							item.Add("mrutime_str",
								Library.TransrateTimestamp(pKey.Timestamp, TimeZoneBias, OutputUtc));
						}
					}
					else
					{
                            // RegRipperでは""で上書きしている
                            item.Remove("mrutime_str");
                            item.Add("mrutime_str", "");
					}

					string m = (item.ContainsKey(MTS) && (item[MTS] != "0")) ? item[MTS] : "";
					string a = (item.ContainsKey(ATS) && (item[ATS] != "0")) ? item[ATS] : "";
					string c = (item.ContainsKey(CTS) && (item[CTS] != "0")) ? item[CTS] : "";
					string o = (item.ContainsKey(DT) && (item[DT] != "N/A")) ? item[DT] : "";

					string resource = pItem[PATH] + item[NAME];
					if (item.ContainsKey("filesize"))
					{
						resource += string.Format(" [{0}]", item["filesize"]);
					}

					Reporter.Write(this.GetRecordText(item["mrutime_str"], m, a, c, o, resource));

					if ((item[NAME] == "") || (item[NAME].EndsWith(@"\")))
					{
						// noop
					}
					else
					{
						item[NAME] += @"\";
					}

					//item[PATH] = pItem[PATH] + @"\" + item[NAME];
                    item[PATH] = pItem[PATH] + item[NAME];
					this.Traverse(pKey.GetSubkey(key), ref item);
				}
			}
			catch (Exception ex)
			{
				var msg = ex.Message;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private Dictionary<string, string> ParseVariableEntry(byte[] data)
		{
			var item = new Dictionary<string, string>();

			item.Add(TYPE, Convert.ToString(data[2]));

			int tag = Convert.ToInt32(Convert.ToString(data[0x0A]), 16);
			
			if (BitConverter.ToUInt16(data, 4) == 0x1A)
			{
				string guid = this.ParseGUID(Library.ExtractArrayElements(data, 14, 16));

				if (this._folderTypes.ContainsKey(guid))
				{
					item.Add(NAME, this._folderTypes[guid]);
				}
				else
				{
					item.Add(NAME, guid);
				}
			}
			// オリジナルはバイナリも文字列も関係なく照合したり切ったりUnpackしたりやりたい放題だが.Netではそうはいかない・・・
			else if (BitConverter.ToString(data).Contains("31-53-50-53")) // "1SPS" = "31-53-50-53"
			{
				// いったん文字列に変換して指定の文字で切る
				string[] seg = BitConverter.ToString(data).Split(new string[] { "-31-53-50-53-" },
																	StringSplitOptions.RemoveEmptyEntries);

				var segs = new Dictionary<string, byte[]>();

				var bytes = new List<byte>();

				foreach (string s in seg)
				{
					// バイト配列に戻して処理
					string[] arr = s.Split(new char[] {'-'});
					foreach (string letter in arr)
					{
						bytes.Add(Convert.ToByte(letter, 16));
					}

					string guid = this.ParseGUID(Library.ExtractArrayElements(bytes.ToArray(), 0, 16));
					if (!segs.ContainsKey(guid))
					{
						segs.Add(guid, bytes.ToArray());
					}
					bytes.Clear();
				}

				if (segs.ContainsKey("{b725f130-47ef-101a-a5f1-02608c9eebac}"))
				{
					byte[] stuff = segs["{b725f130-47ef-101a-a5f1-02608c9eebac}"];

					bool tag2 = true;
					int cnt = 0x10;

					while (tag2)
					{
						uint sz = BitConverter.ToUInt32(stuff, cnt);
						uint id = BitConverter.ToUInt32(stuff, cnt + 4);

						// sub-segment types
						// 0x0a - file name
						// 0x14 - short name
						// 0x0e, 0x0f, 0x10 - mod date, create date, access date(?)
						// 0x0c - size
						if (sz == 0x00)
						{
							tag2 = false;
							continue;
						}
						else if (id == 0x0a)
						{
							uint num = BitConverter.ToUInt32(stuff, cnt + 13);
							string str = Encoding.Unicode.GetString(stuff, cnt + 13 + 4, ((int)num * 2));

							item.Add(NAME, str.Replace("\0", ""));
						}
						cnt += (int)sz;
					}
				}
				else
				{
					item.Add(NAME, "Unknown Type");
				}
			}
			// TODO ASCII/Unicode 未確認
			else if (Encoding.Unicode.GetString(data, 4, 4).Equals("AugM"))
			{
				item = this.ParseFolderEntry(data);
			}
			// Following two entries are for Device Property data
			else if (tag == 0x7b || tag == 0xbb || tag == 0xfb)
			{
				uint sz1 = BitConverter.ToUInt32(data, 0x3e);
				//uint sz2 = BitConverter.ToUInt32(data, 0x3e + 4);
				//uint sz3 = BitConverter.ToUInt32(data, 0x3e + 4 + 4); Don't use...

				item.Add(NAME, Encoding.Unicode.GetString(data, 0x4a, (int)sz1 * 2).Replace("\0", ""));
			}
			else if (tag == 0x02 || tag == 0x03)
			{
				uint sz1 = BitConverter.ToUInt32(data, 0x26);
				item.Add(NAME, Encoding.Unicode.GetString(data, 0x36, (int)sz1 * 2).Replace("\0", ""));
			}
			else
			{
				item.Add(NAME, "Unknown Type");
			}

			return item;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data">Registry key data</param>
		/// <returns>Path list</returns>
		private Dictionary<string, string> ParseNetworkEntry(byte[] data)
		{
			var item = new Dictionary<string, string>();

			item.Add(TYPE, Convert.ToString(data[2]));

			item.Add(NAME, Encoding.ASCII.GetString(data, 5, 5 + Array.IndexOf(data, "\0")));

			return item;
		}

		/// <summary>
		/// parses what appears to be Zip file subfolders; this type 
		/// appears to contain the date and time of when the subfolder
		/// was accessed/opened, in string format.
		/// </summary>
		/// <param name="data">Registry key data</param>
		/// <returns>Path list</returns>
		private Dictionary<string, string> ParseZipSubFolderItem(byte[] data)
		{
			var item = new Dictionary<string, string>();

			// Get the opened/accessed date/time
			item.Add(DT, Encoding.ASCII.GetString(data, 0x24, 6).Replace("\0", ""));
			// TODO ASCII/Unicode 未確認

			if (item[DT] == "N/A")
			{
				// noop
			}
			else
			{
				// TODO ASCII/Unicode 未確認
				string dt = Encoding.ASCII.GetString(data, 0x24, 40).Replace("\0", "");
				item[DT] = dt;

				string[] datetime = dt.Split(new char[] { ' ' }, 2);
				string date = datetime[0].Trim();
				string time = datetime[1].Trim();

				string[] ymd = date.Split(new char[] { '/' }, 3);
				string mon = ymd[0].Trim();
				string day = ymd[1].Trim();
				string yr = ymd[2].Trim();;

				string[] hms = time.Split(new char[] { ':' }, 3);
				string hr = hms[0].Trim();
				string min = hms[1].Trim();
				string sec = hms[2].Trim();

				item.Add("datetime", string.Format("{0}/{1}/{2} {3}:{4}:{5}",
					new object[] { yr, mon, day, hr, min, sec }));
			}

			uint sz = BitConverter.ToUInt32(data, 0x54);
			uint sz2 = BitConverter.ToUInt32(data, 0x58);

			string str1 = Encoding.ASCII.GetString(data, 0x5C, (int)sz * 2).Replace("\0", "");

			if (0 < sz2)
			{
				// TODO ASCII/Unicode 未確認
				string str2 =
					Encoding.ASCII.GetString(data, 0x5C + ((int)sz * 2), (int)sz2 * 2).Replace("\0", "");

				item.Add(NAME, Path.Combine(str1, str2));
			}
			else
			{
				item.Add(NAME, str1);
			}
			
			return item;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private Dictionary<string, string> Parse01ShellItem(byte[] data)
		{
			var item = new Dictionary<string, string>();

			item.Add(TYPE, Convert.ToString(data[2]));

			item.Add(NAME, "");

			return item;
		}

		/// <summary>
		/// I honestly have no idea what to do with this data; there's really
		/// no reference for or description of the format of this data.  For
		/// now, this is just a place holder
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private Dictionary<string, string> ParseURIEntry(byte[] data)
		{
			var item = new Dictionary<string, string>();

			item.Add(TYPE, Convert.ToString(data[2]));

			uint lo = BitConverter.ToUInt32(data, 0x0e);
			uint hi = BitConverter.ToUInt32(data, 0x0e + 4);

			double timestamp = Library.GetTime(lo, hi);

			item.Add("uritime", timestamp.ToString());

			uint sz = BitConverter.ToUInt32(data, 0x2a);
			string uri = Encoding.ASCII.GetString(data, 0x2e, (int)sz).Replace("\0", "");
			// TODO ASCII/Unicode 未確認

			// TODO ASCII/Unicode 未確認
			string proto = Encoding.ASCII.GetString(data, data.Length - 6, 6).Replace("\0", "");

			item.Add(NAME, string.Format("{0}://{1} [{2}]", proto, uri,
				Library.TransrateTimestamp(timestamp, TimeZoneBias, OutputUtc)));

			return item;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private Dictionary<string, string> ParseSystemFolderEntry(byte[] data)
		{
			var item = new Dictionary<string, string>();

			var vals = new Dictionary<int, string>();
			vals.Add(0x00, "Explorer");
			vals.Add(0x42, "Libraries");
			vals.Add(0x44, "Users");
			vals.Add(0x4c, "Public");
			vals.Add(0x48, "My Documents");
			vals.Add(0x50, "My Computer");
			vals.Add(0x58, "My Network Places");
			vals.Add(0x60, "Recycle Bin");
			vals.Add(0x68, "Explorer");
			vals.Add(0x70, "Control Panel");
			vals.Add(0x78, "Recycle Bin");
			vals.Add(0x80, "My Games");

			item.Add(TYPE, Convert.ToString(data[2]));

			string id = Convert.ToString(data[3]);
			item.Add("id", id);

			int key = int.MinValue;
			int.TryParse(id, out key);
			if (vals.ContainsKey(key))
			{
				item.Add(NAME, vals[key]);
			}
			else
			{
				item.Add(NAME, this.ParseGUID(Library.ExtractArrayElements(data, 4, 16)));
			}

			return item;
		}

		/// <summary>
		/// Takes 16 bytes of binary data, returns a string formatted as an MS GUID.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private string ParseGUID(byte[] data)
		{
			string d1 = BitConverter.ToUInt32(data, 0).ToString("x8");
			string d2 = BitConverter.ToUInt16(data, 4).ToString("x4");
			string d3 = BitConverter.ToUInt16(data, 6).ToString("x4");

			string d4 = data[8].ToString("x2") + data[9].ToString("x2");

			string d5 = data[10].ToString("x2") + data[11].ToString("x2") +
						data[12].ToString("x2") + data[13].ToString("x2") +
						data[14].ToString("x2") + data[15].ToString("x2");

			return "{" + string.Format("{0}-{1}-{2}-{3}-{4}", new object[] { d1, d2, d3, d4, d5 }) + "}";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private Dictionary<string, string> ParseDeviceEntry(byte[] data)
		{
			var item = new Dictionary<string, string>();

			if ((data.Length < 34) || this._isXp) // ここのロジックはVista以降専用っぽいので
			{
				item.Add(NAME, "パース不可");
				return item;
			}

			uint userlen = BitConverter.ToUInt32(data, 30);
			//uint devlen = BitConverter.ToUInt32(data, 34);

			if (data.Length < (0x28 + (long)userlen * 2))
			{
				item.Add(NAME, "パース不可");
			}
			else
			{
				// TODO ASCII/Unicode 未確認
				string user = Encoding.ASCII.GetString(data, 0x28, (int)userlen * 2).Replace("\0", "");
				//string dev = Encoding.ASCII.GetString(data,
				//    0x28 + ((int)userlen * 2), (int)devlen * 2).Replace("\0", ""); // また使ってないし・・・

				item.Add(NAME, user);
			}

			return item;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private Dictionary<string, string> ParseDriveEntry(byte[] data)
		{
			var item = new Dictionary<string, string>();

			item.Add(TYPE, Convert.ToString(data[2]));
			item.Add(NAME, Encoding.ASCII.GetString(data, 3, 3));

			return item;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private Dictionary<string, string> ParseControlPanelEntry(byte[] data)
		{
			var item = new Dictionary<string, string>();

			item.Add(TYPE, Convert.ToString(data[2]));

			string guid = this.ParseGUID(Library.ExtractArrayElements(data, 14, 16));

			item.Add(NAME, this._cpGuids.ContainsKey(guid) ? this._cpGuids[guid] : guid);

			return item;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private Dictionary<string, string> ParseFolderEntry(byte[] data)
		{
			var item = new Dictionary<string, string>();

			item.Add(TYPE, Convert.ToString(data[2]));

			int type = int.MinValue;
			int.TryParse(item[TYPE], out type);

			// Type 0x74 folders have a slightly different format

			int ofsMdate;
			int ofsShortName;

			if (type == 0x74)
			{
				ofsMdate = 0x12;
			}
			else if (Encoding.ASCII.GetString(data, 4, 4).Equals("AugM"))
			{
				ofsMdate = 0x1c;
			}
			else
			{
				ofsMdate = 0x08;
			}

			// some type 0x32 items will include a file size
			if (type == 0x32)
			{
				uint size = BitConverter.ToUInt32(data, 4);

				if (size != 0)
				{
					item.Add("filesize", size.ToString());
				}
			}

			ushort m0 = BitConverter.ToUInt16(data, ofsMdate);
			ushort m1 = BitConverter.ToUInt16(data, ofsMdate + 2);

			double gmtime = 0;

			item.Add(MTS, this.ConvertDosDate(m0, m1, out gmtime));
			//item.Add("mtime", gmtime.ToString());
			
			// Need to read in short name; nul-term ASCII
			// $item{shortname} = (split(/\00/,substr($data,12,length($data) - 12),2))[0];
			ofsShortName = ofsMdate + 6;

			bool tag = true;
			int cnt = 0;
			string shortName = "";

			while (tag)
			{
				string s = Encoding.ASCII.GetString(data, ofsShortName + cnt, 1);

				if (s.Equals("\0") && (((cnt + 1) % 2) == 0))
				{
					tag = false;
				}
				else
				{
					shortName += s;
					cnt++;
				}
			}

			shortName = shortName.Replace("\0", "");

			int ofs = ofsShortName + cnt + 1;

			// Read progressively, 1 byte at a time, looking for 0xbeef
			tag = true;
			cnt = 0;

			while (tag)
			{
				if (BitConverter.ToUInt16(data, ofs + cnt) == 0xbeef)
				{
					tag = false;
				}
				else
				{
					cnt++;
				}
			}

			item.Add("extver", BitConverter.ToUInt16(data, ofs + cnt - 4).ToString());

			ofs += cnt + 2;

			m0 = BitConverter.ToUInt16(data, ofs);
			m1 = BitConverter.ToUInt16(data, ofs + 2);

			gmtime = 0;

			item.Add(CTS, this.ConvertDosDate(m0, m1, out gmtime));
			//item.Add("ctime", gmtime.ToString());

			ofs += 4;

			m0 = BitConverter.ToUInt16(data, ofs);
			m1 = BitConverter.ToUInt16(data, ofs + 2);

			gmtime = 0;

			item.Add(ATS, this.ConvertDosDate(m0, m1, out gmtime));
			//item.Add("atime", gmtime.ToString());

			if (int.Parse(item["extver"]) == 0x03)
			{
				ofs += 8;
			}
			else if (int.Parse(item["extver"]) == 0x07)
			{
				ofs += 26;
			}
			else if (int.Parse(item["extver"]) == 0x08)
			{
				ofs += 30;
			}
			else
			{
				// noop
			}

			string longName = "";
			if (ofs + (data.Length - 30) < data.Length)
			{
				longName = Encoding.ASCII.GetString(data, ofs, data.Length - 30);
				longName =
					longName.Split(new string[] { "\0\0" }, 2, StringSplitOptions.None)[0].Replace("\0", "");
			}

			if (!string.IsNullOrEmpty(longName))
			{
				item.Add(NAME, longName);
			}
			else
			{
				item.Add(NAME, shortName);
			}

			return item;
		}

		/// <summary>
		/// subroutine to convert 4 bytes of binary data into a human-
		/// readable format.  Returns both a string and a Unix-epoch time.
		/// </summary>
		/// <param name="date"></param>
		/// <param name="time"></param>
		/// <param name="gmTime">Extra return value (epochtime of return value)</param>
		/// <returns></returns>
		private string ConvertDosDate(ushort date, ushort time, out double gmTime)
		{
			if (date == 0x00 || time == 0x00)
			{
				gmTime = 0;
				return DateTime.MinValue.ToString();
			}
			else
			{
				int sec = ((time & 0x1f) * 2);
				if (sec.Equals(60))
				{
					sec = 59;
				}

				int min = ((time & 0x7e0) >> 5);
				int hr = ((time & 0xF800) >> 11);
				int day = (date & 0x1f);
				int mon = ((date & 0x1e0) >> 5);
				int yr = (((date & 0xfe00) >> 9) + 1980);

				// TODO 解析したところ特に必要な処理ではないので、gmTimeの算出は必要になったら実装する
				gmTime = 0;

				// Maybe registry has UTC, so convert JST
				DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				DateTime dt = new DateTime(yr, mon, day, hr, min, sec);
				double timestamp = dt.Subtract(epoch).TotalSeconds;

				return Library.TransrateTimestamp(timestamp, TimeZoneBias, OutputUtc);
			}
		}

		/// <summary>
		/// Initial code for parsing type 0x35
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private Dictionary<string, string> ParseFolderEntry2(byte[] data)
		{
			var item = new Dictionary<string, string>();

			int ofs = 0;
			bool tag = true;

			while (tag)
			{
				if (BitConverter.ToUInt16(data, ofs) == 0xbeef)
				{
					tag = false;
				}
				else
				{
					ofs++;
				}
			}

			item.Add("extver", BitConverter.ToUInt16(data, ofs - 4).ToString());

			// Move offset over to end of where the ctime value would be
			ofs += 4;

			if (int.Parse(item["extver"]) == 0x03)
			{
				ofs += 8;
			}
			else if (int.Parse(item["extver"]) == 0x07)
			{
				ofs += 26;
			}
			else if (int.Parse(item["extver"]) == 0x08)
			{
				ofs += 30;
			}
			else
			{
				// noop
			}

			Reporter.Write(" --- parseFolderEntry2 --- ");

			ulong length = Math.Min((ulong)(data.Length - ofs), (ulong)(data.Length - 30));

			byte[] bytes = Library.ExtractArrayElements(data, (ulong)ofs, length);

			var d = this.PrintData(bytes);

			foreach (string s in d)
			{
				Reporter.Write(s);
			}

			string str = Encoding.Unicode.GetString(bytes);

			string repBefore = ((char)13).ToString() + ((char)20).ToString();
			string repAfter = ((char)2D).ToString() + ((char)00).ToString();

			str = str.Split(new string[] { "\0\0" }, 2, StringSplitOptions.None)[0];
			str = str.Replace(repBefore, repAfter).Replace("\0", "");

			// 制御文字を削除
			str = System.Text.RegularExpressions.Regex.Replace(str, @"\p{Cc}", "");

			item.Add(NAME, str);

			return item;
		}

		/// <summary>
		/// subroutine used primarily for debugging; takes an arbitrary
		/// length of binary data, prints it out in hex editor-style
		/// format for easy debugging
		/// </summary>
		/// <param name="data"></param>
		/// <returns>String list</returns>
		private List<string> PrintData(byte[] data)
		{
			int len = data.Length;
			var display = new List<string>();

			int loop = len / 16;
			if ((len % 16) != 0)
			{
				loop++;
			}

			for (int cnt = 0; cnt < loop; ++cnt)
			{
				int left = len - (cnt * 16);

				int n = (left < 16) ? left : 16;

				byte[] seg = Library.ExtractArrayElements(data, (ulong)(cnt * 16), (ulong)n);

				var str1 = new List<string>();
				string str = "";

				foreach (byte b in seg)
				{
					byte[] bar = new byte[] { b };

					str1.Add(string.Format("{0:X2}", b));

					if (Library.IsAscii(bar))
					{
						str += Encoding.ASCII.GetString(bar);
					}
					else
					{
						str += ".";
					}
				}

				string h = string.Join(" ", str1.ToArray());

				display.Add(string.Format("{0, 8:X8}: {1, -47}  {2}", (cnt * 16), h, str));
			}

			return display;
		}

		#region Old Code Not in use
		private void Recursive(RegistryKey key, string path, string folderName)
		{
			// NodeSlotから実体のフォルダの内容を取得
			RegistryValue slotValue = key.GetValue("NodeSlot");

			if (null != slotValue)
			{
				string slotNumber = slotValue.GetDataAsObject().ToString();
				//                RegistryKey bagKey = RootKey.GetSubkey(pathBuilder.ToString() + @"\" + slotValue.GetData().ToString());
				RegistryKey bagKey = RootKey.GetSubkey(path.Remove(path.Length - 3) + @"s\" + slotNumber + @"\Shell");

				if (null != bagKey)
				{
					RegistryKey[] subkeys = bagKey.GetListOfSubkeys();
					if (null != subkeys)
					{

					}

					RegistryValue[] bagValues = bagKey.GetListOfValues();

					if (null != bagValues)
					{

						Reporter.Write("  FolderName : " + ((0 != folderName.Length) ? folderName : "デスクトップ?"));
						Reporter.Write("    [ValueList]");
						string dataString = "";
						byte[] bytes;
						StringBuilder dataBuilder = new StringBuilder();
						foreach (RegistryValue bagValue in bagValues)
						{
							if (Constants.REG_BINARY != bagValue.Type)
							{
								dataString = bagValue.GetDataAsString();
							}
							else
							{
								bytes = bagValue.Data;
								foreach (byte item in bytes)
								{
									if (0 < dataBuilder.Length)
									{
										dataBuilder.Append(" ");
									}
									dataBuilder.Append(item.ToString("X2"));
								}
								dataString = dataBuilder.ToString();
							}
							Reporter.Write("      " + bagValue.Name + " : " + dataString);
						}
						Reporter.Write("");
					}
				}
			}

			// まずMRUListExを取得
			RegistryValue mruValue = key.GetValue("MRUListEx");

			if (null != mruValue)
			{
				byte[] mruData = (byte[])mruValue.GetDataAsObject();
				if (null != mruData)
				{
					uint mru = 0;
					RegistryValue value;
					List<byte> byteList = new List<byte>();
					byte[] bytes = null;

					for (uint count = 0; count < mruData.Length; count += 4)
					{
						mru = BitConverter.ToUInt16(Library.ExtractArrayElements(mruData, count, 4), 0);

						value = key.GetValue(mru.ToString());

						if (null != value)
						{
							bytes = (byte[])value.GetDataAsObject();

							if (0x19 == bytes[0])
							{
								if (0 < folderName.Length)
								{
									Logger.Write(LogLevel.WARNING, folderName + " / " + mru.ToString() + "は怪しげです。");
								}
								folderName = Encoding.ASCII.GetString(Library.ExtractArrayElements(bytes, 3, 3));

							}
							else
							{
								byteList = new List<byte>();
								//                                Reporter.Write("\t" + value.Name + "WriteTime  : " Library.ExtractArrayElements(bytes, 8, 4));
								//hoge++;
								//System.IO.File.WriteAllBytes(@"C:\WORK\KaniReg\dummyFile\" + hoge.ToString() + ".txt" , (byte[])data);                                
								bool start = false;
								for (uint innerCount = 4; innerCount < bytes.Length; innerCount++)
								{

									if (!start && 0x14 == bytes[innerCount - 4] && 0x00 == bytes[innerCount - 3] && 0x00 == bytes[innerCount - 2] && 0x00 == bytes[innerCount - 1])
									{
										start = true;
									}

									if (!start)
										continue;

									byteList.Add(bytes[innerCount]);
									if (0x00 == bytes[innerCount] && 0x00 == bytes[innerCount + 1])
									{
										break;
									}
								}
								if (0 < folderName.Length)
								{
									folderName += @"\";
								}
								folderName += Encoding.Unicode.GetString(byteList.ToArray());
							}
						}

						RegistryKey subKey = key.GetSubkey(mru.ToString());

						if (null != subKey)
						{
							Recursive(subKey, path, folderName);
						}
					}
				}
			}
		}
		#endregion
	}
}