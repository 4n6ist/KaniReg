using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.NtUser {

    class VncViewer : ClassBase {

        public VncViewer(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\ORL\VNCviewer\MRU";
            Description = "VncViewerの実行履歴";
        }

        public override bool Process() {
               
            RegistryValue[] values = this.Key.GetListOfValues();
            if (null != values && 0 < values.Length) {

                string index = string.Empty;
                Dictionary<string, string> dictionary = new Dictionary<string ,string>();
                foreach (RegistryValue value in values) {
                    if (!"index".Equals(value.Name)) {
                        dictionary.Add(value.Name, value.GetDataAsObject().ToString());
                    } else {
                        index = value.Name;
                    }
                }
              
                this.Reporter.Write("Index = " + index);

                if (0 < index.Length) {
                    char[] tags = index.ToCharArray();
                    foreach (char tag in tags) {
                        this.Reporter.Write("  " + tag  + " -> " + dictionary[tag.ToString()]);
                    }
                }
            }

            return true;
        }
    }
}
