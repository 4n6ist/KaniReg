using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KaniReg.NtUser {
    class FileExts : ClassBase{

        public FileExts(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts";
            Description = "拡張子のファイルを開く際に用いたアプリケーション";
        }

        public override bool  Process() {

            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            RegistryKey openWithListKey;
            string data;
            
            if (null != subkeys && 0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {

                    string name = subkey.Name;
                    // 拡張子形式でなければskip
                    if (!Regex.IsMatch(name, @"^\.[A-Za-z0-9]+$")) {
                        continue;
                    }

                    try {
                        openWithListKey = subkey.GetSubkey("OpenWithList");
                        data = openWithListKey.GetValue("MRUList").GetDataAsObject().ToString();

                        // 英数字のときには処理
                        if (Regex.IsMatch(data, @"^[A-Za-z0-9]+$")) {

                            Reporter.Write("File Extension: " + name);
                            Reporter.Write("LastWrite: " + Library.TransrateTimestamp(openWithListKey.Timestamp, TimeZoneBias, OutputUtc));
                            Reporter.Write("MRUList: " + data);

                            // 1文字ずつに切り刻む
                            char[] dataChars = data.ToCharArray();
                            foreach (char dataChar in dataChars) {
                                string single = dataChar.ToString();
                                string valueData = openWithListKey.GetValue(single).GetDataAsObject().ToString();
                                Reporter.Write("  " + single + " => " + valueData);
                            }
                            Reporter.Write("");
                        }
                    } catch {
                        // noop
                    }
                }
            } else {
                Reporter.Write(KeyPath + " does not have subkeys.");
            }

            return true;
        }
    }
}
