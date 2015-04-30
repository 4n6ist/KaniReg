using System;
using System.Collections.Generic;


namespace KaniReg.Software {

    class ShellCommands : ClassBase {

        public ShellCommands(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            Description = "Fileタイプのopen動作で実際に行われる動作";
        }

        public override bool Process() {

            string[] commands = {"exe","cmd","bat","hta","pif"};
            string keyPath = string.Empty;
            RegistryValue value = null;
            foreach (string command in commands) {
                
                keyPath = "Classes\\" + command + "file\\shell\\open\\command";
                RegistryKey key = RootKey.GetSubkey(keyPath);
                if (null != key) {
                    Reporter.Write(keyPath);
                    Reporter.Write("最終更新日時: " + Library.TransrateTimestamp(key.Timestamp, TimeZoneBias, OutputUtc));

                    value = key.GetValue("(Default)");
                    if (null != value) {
                        Reporter.Write("\tCmd: " + value.GetDataAsObject().ToString());
                    } else {
                        Reporter.Write(command + "にはVALUEがありませんでした。");
                    }
                } else {
                    Reporter.Write(keyPath + " キーは見つかりませんでした。");
                }
            }
            Reporter.Write("");

            return true;
        }
    }
}
