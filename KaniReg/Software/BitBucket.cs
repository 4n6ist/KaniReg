using System;
using System.Collections.Generic;


namespace KaniReg.Software {

    class BitBucket : ClassBase {

        public BitBucket(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\Windows\CurrentVersion\Explorer\BitBucket";
            Description = "ゴミ箱の設定";
        }

        public override bool Process() {

            RegistryValue value = Key.GetValue("NukeOnDelete");
            if (null != value) {
                this.Reporter.Write("NukeOnDelete VALUE = " + Key.GetValue("NukeOnDelete").GetDataAsObject().ToString());
            } else {
                this.Reporter.Write("NukeOnDelete VALUEは見つかりませんでした。");
            }
            this.Reporter.Write("");
            
            RegistryKey[] subkeys = this.Key.GetListOfSubkeys();
            uint data = 0;
            if (null != subkeys && 0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {
                    this.Reporter.Write(KeyPath + "\\" + subkey.Name);
                    this.Reporter.Write("LastWrite Time = " + Library.TransrateTimestamp(subkey.Timestamp, this.TimeZoneBias, this.OutputUtc));
                    try {
                        data = (uint)subkey.GetValue("VolumeSerialNumber").GetDataAsObject();
                        this.Reporter.Write("VolumeSerialNumber = 0x" + data.ToString("X8"));
                    } catch {

                    }
                    this.Reporter.Write("");
                }
            } else {
                this.Reporter.Write(KeyPath + " にはサブキーがありませんでした。");
            }

            return true;
        }
    }
}
