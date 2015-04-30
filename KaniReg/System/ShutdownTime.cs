using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.SysHive {

    class ShutdownTime : ClassBase {

        public ShutdownTime(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = "ControlSet00" + Current + @"\Control\Windows";
            Description = "シャットダウン時間";
        }

        public override bool Process() {

            Reporter.Write("キーのパス：" + KeyPath);
            Reporter.Write("キーの最終更新日時：" + Library.TransrateTimestamp(Key.Timestamp, TimeZoneBias, OutputUtc));
            Reporter.Write("");

            Reporter.Write(KeyPath + " key, ShutdownTime value");
            Reporter.Write(KeyPath);

            byte[] data = (byte[])Key.GetValue("ShutdownTime").GetDataAsObject();

            if (null != data) {
                double filetime = Library.CalculateTimestamp(BitConverter.ToUInt64(data, 0));
                this.Reporter.Write("  ShutdownTime = " + Library.TransrateTimestamp(filetime, this.TimeZoneBias, this.OutputUtc));
                    
            } else {
                this.Reporter.Write("ShutdownTime VALUE はありませんでした。");
            }

            return true;
        }
    }
}
