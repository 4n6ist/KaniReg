using System;
using System.Collections.Generic;


namespace KaniReg.Software {

    class NetworkListNlaCache : ClassBase {

        public NetworkListNlaCache(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\Windows NT\CurrentVersion\NetworkList\Nla\Cache";
            Description = "NetworkListの接続種類/ドメイン毎の更新日付(Vista/7用)";
        }

        public override bool Process() {

            RegistryKey[] subkeys = this.Key.GetListOfSubkeys();
            RegistryKey[] endkeys = null;
            if (null != subkeys && 0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {
                    this.Reporter.Write(subkey.Name);
                    endkeys = subkey.GetListOfSubkeys();
                    if (null != endkeys && 0 < endkeys.Length) {
                        foreach (RegistryKey endkey in endkeys) {
                            this.Reporter.Write("\t" + endkey.Name + " => " + Library.TransrateTimestamp(endkey.Timestamp, this.TimeZoneBias, this.OutputUtc));
                        }
                    }
                }
            }

            return true;
        }
    }
}
