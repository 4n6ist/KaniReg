using System;
using System.Collections.Generic;


namespace KaniReg.Software {

    class AppInitDlls : ClassBase {

        public AppInitDlls(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\Windows NT\CurrentVersion\Windows";
            Description = "Windowsロード時に実行されるDLL";
        }

        public override bool Process() {

            RegistryValue[] values = this.Key.GetListOfValues();
            object data = null;
            string writeData = string.Empty;
            foreach (RegistryValue value in values) {
                if ("AppInit_DLLs".Equals(value.Name)) {
                    data = value.GetDataAsObject();
                    if (null != data) {
                        writeData = data.ToString().Replace("\0", "");
                        if (string.Empty.Equals(writeData.Trim())) {
                            writeData = "{blank}";
                        }
                    } else {
                        writeData = "{blank}";
                    }
                    this.Reporter.Write(value.Name + " -> " + writeData);
                }
            }

            return true;
        }
    }
}
