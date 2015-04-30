using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.NtUser {

    class UserBitBucket : ClassBase {

        public UserBitBucket(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\BitBucket";
            Description = "ゴミ箱の設定";
        }

        public override bool Process() {

            RegistryValue[] values = Key.GetListOfValues();
            if (null != values && 0 < values.Length) {
                foreach (RegistryValue value in values) {
                    Reporter.Write(value.Name + " : " + value.GetDataAsString());
                }

            } else {
                Library.WriteNoValue(KeyPath, Reporter);
            }
            Reporter.Write("");

            RegistryKey[] subkeys = null;
            RegistryKey volumeKey = Key.GetSubkey("Volume");
            if (null != volumeKey) {
                subkeys = volumeKey.GetListOfSubkeys();

                if (null != subkeys && 0 < subkeys.Length) {
                    RegistryValue nukeValue = null;
                    foreach (RegistryKey subkey in subkeys) {
                        Reporter.Write(subkey.Name + " [" + Library.TransrateTimestamp(subkey.Timestamp, TimeZoneBias, OutputUtc) + "]");
                        nukeValue = subkey.GetValue("NukeOnDelete");
                        if (null != nukeValue) {

                            Reporter.Write("   NukeOnDelete " + nukeValue.GetDataAsObject().ToString());
                        } else {
                            Reporter.Write("\\NukeOnDelete ではVALUEを取得できませんでした。");
                        }
                    }

                } else {
                    Library.WriteNoValue(KeyPath + "\\Volume", Reporter);
                }
            } else {
                Reporter.Write(KeyPath + "\\Volume サブキーにはアクセスできませんでした。");
            }

            return true;
        }
    }
}
