using System;
using System.Collections.Generic;


namespace KaniReg.Software {

    class ActiveXCompatibilities : ClassBase {

        private readonly string[] GUIDS = {
            "{F0E42D50-368C-11D0-AD81-00A0C90DC8D9}",
            "{F0E42D60-368C-11D0-AD81-00A0C90DC8D9}",
            "{F2175210-368C-11D0-AD81-00A0C90DC8D9}"};

        public ActiveXCompatibilities(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\Internet Explorer";
            Description = "ActiveXの脆弱性スナップショット";
        }

        public override bool Process() {

            string version = string.Empty;

            RegistryValue value = this.Key.GetValue("Version");

            if (null != value) {
                this.Reporter.Write("IE Version = " + value.GetDataAsObject().ToString());
            } else {
                this.Reporter.Write("IE Version は見つかりませんでした。");
            }

            this.Reporter.Write("");

            RegistryKey subkey;
            foreach (string guid in GUIDS) {
                subkey = this.Key.GetSubkey("ActiveX Compatibility\\" + guid);
                if (null != subkey) {
                    this.Reporter.Write("GUID: " + guid);
                    value = subkey.GetValue("Compatibility Flags");

                    if (null != value) {
                        this.Reporter.Write("Compatibility Flags  0x" + ((uint)value.GetDataAsObject()).ToString("X8")); 
                    } else {
                        this.Reporter.Write("Compatibility Flags は見つかりませんでした。");
                    }
                } else  {
                    this.Reporter.Write(guid + " は見つかりませんでした。");
                }
                this.Reporter.Write("");
            }

            return true;
        }
    }
}
