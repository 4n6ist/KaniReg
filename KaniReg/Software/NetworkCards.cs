using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.Software {

    class NetworkCards : ClassBase {

        public NetworkCards(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\Windows NT\CurrentVersion\NetworkCards";
            Description = "ネットワークカード一覧";
        }

        public override bool Process() {

            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            string name = string.Empty;
            string description = string.Empty;
            if (null != subkeys && 0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {
                    name = subkey.GetValue("ServiceName").GetDataAsObject().ToString();
                    description = subkey.GetValue("Description").GetDataAsObject().ToString();
                    Reporter.Write(name);
                    Reporter.Write("\t説明      :" + description);
                    Reporter.Write("\t最終更新日:" + Library.TransrateTimestamp(subkey.Timestamp, TimeZoneBias, OutputUtc));
                }
                
            } else {
                Reporter.Write(KeyPath + " にはサブキーがありませんでした。");
            }
            Reporter.Write("");

            return true;
        }
    }
}
