using System;
using System.Collections.Generic;


namespace KaniReg.Software {

    class ProfileList : ClassBase {

        public ProfileList(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\Windows NT\CurrentVersion\ProfileList";
            Description = "ユーザープロファイル一覧";
        }

        public override bool Process() {

            Reporter.Write(KeyPath);
            Reporter.Write("LastWrite Time " + Library.TransrateTimestamp(Key.Timestamp, TimeZoneBias, OutputUtc));
            Reporter.Write("");
        
            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {

                string path = string.Empty;
                uint low = 0;
                uint high = 0;
                ulong filetime = 0;
                byte[] bytes = new byte[8];

                foreach (RegistryKey subkey in subkeys) {
                    try {
                        path = subkey.GetValue("ProfileImagePath").GetDataAsObject().ToString();
                    } catch {
                        // noop
                    }
                    Reporter.Write("Path      : " + path);
                    Reporter.Write("SID       : " + subkey.Name);
                    Reporter.Write("LastWrite : " + Library.TransrateTimestamp(subkey.Timestamp, TimeZoneBias, OutputUtc));
                    
                    try {
/*
                        low = (uint)subkey.GetValue("ProfileLoadTimeLow").GetData();
                        high = (uint)subkey.GetValue("ProfileLoadTimeHigh").GetData();
                        Reporter.Write("LoadTime  : " + Library.TransrateTimestamp(Library.GetTime(low, high)));
*/
                        // lowとhighを取得
                        low = (uint)subkey.GetValue("ProfileLoadTimeLow").GetDataAsObject();
                        high = (uint)subkey.GetValue("ProfileLoadTimeHigh").GetDataAsObject();

                        // 配列化してマージ
                        BitConverter.GetBytes(low).CopyTo(bytes, 0);
                        BitConverter.GetBytes(high).CopyTo(bytes, 4);
                        filetime = BitConverter.ToUInt64(bytes, 0);

                        // Report
                        Reporter.Write("LoadTime  : " + Library.TransrateTimestamp(Library.CalculateTimestamp(filetime), TimeZoneBias, OutputUtc));

                    } catch {
                        // noop
                    }
                    Reporter.Write("");
                }
            }
            else {
                Reporter.Write(KeyPath + "にはサブキーがありませんでした。");
            }

            return true;
        }
    }
}
