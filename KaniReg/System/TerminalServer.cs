using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.SysHive {

    class TerminalServer : ClassBase {

        public TerminalServer(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = "ControlSet00" + Current + @"\Control\Terminal Server";
            Description = "共有設定";
        }

        public override bool Process() {

            Reporter.Write("キーのパス：" + KeyPath);
            Reporter.Write("キーの最終更新日時：" + Library.TransrateTimestamp(Key.Timestamp, TimeZoneBias, OutputUtc));
            Reporter.Write("");

            Reporter.Write(KeyPath + " キー, fDenyTSConnections VALUE を表示します。");
            Reporter.Write("LastWrite Time " + Library.TransrateTimestamp(Key.Timestamp, TimeZoneBias, OutputUtc));

            RegistryValue value = Key.GetValue("fDenyTSConnections");

            if (null != value) {
                object data = value.GetDataAsObject();
                if (null != data) {
                    Reporter.Write("  fDenyTSConnections = " + data.ToString());
                } else {
                    Reporter.Write("fDenyTSConnections VALUE はありませんでした。");
                }
            } else  {
                Reporter.Write("fDenyTSConnections VALUE はありませんでした。 ");
            }

            return true;
        }
    }
}
