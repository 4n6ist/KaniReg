using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.NtUser {

    class WallPaper : ClassBase {

        public WallPaper(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer";
            Description = "デスクトップ壁紙の保存先情報";
        }

        public override bool Process() {

            RegistryKey key = this.Key.GetSubkey(@"Wallpaper\MRU");
            if (null != key) {
                // Old Windows
                RegistryValue[] values = key.GetListOfValues();
                if (null != values && 0 < values.Length) {

                    Dictionary<uint, string> dictionary = new Dictionary<uint, string>();
                    List<uint> tagList = new List<uint>();
                    byte[] data;

                    foreach (RegistryValue value in values) {

                        uint numberName = 0;
                        if (uint.TryParse(value.Name, out numberName)) {
                            data = Library.SplitByteData((byte[])value.GetDataAsObject())[0];
                            dictionary.Add(numberName, Encoding.Unicode.GetString(data));
                        } else if (value.Name.StartsWith("MRUList")) {
                            data = (byte[])value.GetDataAsObject();
                            for (uint cnt = 0; cnt < data.Length; cnt += 4) {
                                tagList.Add(BitConverter.ToUInt32(Library.ExtractArrayElements(data, cnt, 4), 0));
                            }
                        } else {
                            // nothing to do                    
                        }
                    }

                    tagList.Sort(
                        delegate(uint first, uint next) {
                            return first.CompareTo(next);
                        }
                    );

                    foreach (uint index in tagList) {
                        if (0xFFFFFFFF != index) {
                            this.Reporter.Write(index.ToString() + " -> " + dictionary[index]);
                        }
                    }

                } else {
                    this.Reporter.Write(KeyPath + @"\MRU" + " has no values");
                }
            } else {
                // Windows 7
                // Known Folder
                key = this.Key.GetSubkey(@"WallPapers\KnownFolders\0\Windows Wallpapers\MergeFolders");
                if (key == null)
                {
                    this.Reporter.Write("Keyが見つかりませんでした。");
                    return true;
                }
                RegistryValue[] values = key.GetListOfValues();
                if (null != values && 0 < values.Length) {
                    this.Reporter.Write("壁紙の場所: " + values[0].Name);
                } else {
                    this.Reporter.Write(KeyPath + @"WallPapers\KnownFolders\0\Windows Wallpapers\MergeFolders" + " has no values");
                }

                key = this.Key.GetSubkey(@"WallPapers\Images");
                values = key.GetListOfValues();
                this.Reporter.Write("指定した壁紙のパス");
                foreach (RegistryValue value in values) {
                    this.Reporter.Write("\t" + value.Name + ": " + value.GetDataAsString());
                }
            }

            return true;
        }
    }
}
