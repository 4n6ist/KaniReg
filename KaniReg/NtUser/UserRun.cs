using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.NtUser {

    class UserRun : ClassBase {

        public UserRun(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
            Description = "自動実行プログラム";
        }

        public override bool Process() {

            this.Reporter.Write(KeyPath);
            this.Reporter.Write("LastWrite Time " + Library.TransrateTimestamp(this.Key.Timestamp, this.TimeZoneBias, this.OutputUtc));

            RegistryValue[] values = this.Key.GetListOfValues();
            if (null != values && 0 < values.Length) {
                foreach (RegistryValue value in values) {
                    this.Reporter.Write("    " + value.Name + "->" + value.GetDataAsString());
                }
            } else {
                this.Reporter.Write(KeyPath + " has no values.");
            }

            RegistryKey[] subkeys = this.Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {
                    this.Reporter.Write("");
                    this.Reporter.Write(KeyPath + "\\" + subkey.Name);
                    this.Reporter.Write("LastWrite Time " + Library.TransrateTimestamp(subkey.Timestamp, this.TimeZoneBias, this.OutputUtc));

                    values = subkey.GetListOfValues();
                    if (null != values && 0 < values.Length) {
                        foreach (RegistryValue value in values) {
                            this.Reporter.Write("    " + value.Name + " -> " + value.GetDataAsObject().ToString());
                        }
                    }
                }
            } else {
                this.Reporter.Write("");
                this.Reporter.Write(KeyPath + " has no subkeys.");
            }

            return true;
        }
    }
}
