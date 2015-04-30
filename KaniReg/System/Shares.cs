using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.SysHive {

    class Shares : ClassBase {

        public Shares(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = "ControlSet00" + Current + "\\Services\\lanmanserver\\Shares";
            Description = "共有設定";
        }

        public override bool Process() {

            RegistryValue[] values = Key.GetListOfValues();
            List<byte[]> splitted = null;
            if (null != values && 0 < values.Length) {
                foreach (RegistryValue value in values) {
                    this.Reporter.Write(value.Name);

//                    splitted = Library.SplitByteData((byte[])value.GetDataAsObject());
                    splitted = Library.SplitByteData(value.Data);
                    foreach (byte[] data in splitted) {
                        // $i =~ s/\00//g;
                        this.Reporter.Write("  " + Encoding.Unicode.GetString(data));
                    }
                }
                this.Reporter.Write("");
            } else {
                this.Reporter.Write(KeyPath + " にはValueがありませんでした。");
            }

            return true;
        }
    }
}
