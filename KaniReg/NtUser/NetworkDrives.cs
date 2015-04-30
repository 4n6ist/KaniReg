using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.NtUser {
    class NetworkDrives : ClassBase {

        public NetworkDrives(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Map Network Drive MRU";
            Description = "ネットワークドライブに設定されているパスの一覧";
        }

        public override bool Process() {

            RegistryValue[] values = this.Key.GetListOfValues();
            if (null != values && 0 < values.Length) {

                Dictionary<string, string> dictionary = new Dictionary<string, string>();

                // Retrieve values and load into a hash for sorting            
                foreach (RegistryValue value in values) {
                    // Print sorted content to report file            
                    if ("MRUList".Equals(value.Name)) {
                        this.Reporter.Write("  MRUList = " + value.GetDataAsObject().ToString());
                    }
                    dictionary.Add(value.Name, value.GetDataAsObject().ToString());
                }

                // Report
                char[] chars = dictionary["MRUList"].ToCharArray();
                foreach (char tag in chars) {
                    this.Reporter.Write("  " + tag.ToString() + "   " + dictionary[tag.ToString()].ToString());
                }

            } else {
                Library.WriteNoValue(KeyPath, Reporter);
            }

            return true;
        }
    }
}
