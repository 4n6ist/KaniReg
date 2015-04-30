using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.SysHive {

    class ShutdownCount : ClassBase {

        public ShutdownCount(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = "ControlSet00" + Current + @"\Control\Watchdog\Display";
            Description = "シャットダウン回数";
        }

        public override bool Process() {

// 元のとSelectが見つからない時の挙動が若干違うが、大差ないので共通化を優先

            this.Reporter.Write("ShutdownCount");
            this.Reporter.Write(KeyPath);
            this.Reporter.Write("キーの最終更新日時 " + Library.TransrateTimestamp(Key.Timestamp, this.TimeZoneBias, this.OutputUtc));
            this.Reporter.Write("");
                
            uint count = 0;
            RegistryValue[] values = Key.GetListOfValues();
            if (null != values && 0 < values.Length) {
                foreach (RegistryValue value in values) {
                    if ("ShutdownCount".Equals(value.Name)) {
                        count = 1;
                        this.Reporter.Write("ShutdownCount = " + value.GetDataAsObject().ToString());
                    }
                }

                if (0 == count) {
                    this.Reporter.Write("ShutdownCount VALUE はありませんでした。");
                }
            }
            else {
                this.Reporter.Write(KeyPath + " にはVALUEがありませんでした。");
            }

            return true;
        }
    }
}
