using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.NtUser {

    class Paint :ClassBase {

        public Paint(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Applets\Paint\Recent File List";
            Description = "MS Paintで最近見たファイル一覧";
        }

        public override bool Process() {

            // Locate files opened in MS Paint
            RegistryValue[] values = this.Key.GetListOfValues();
            if (null != values && 0 < values.Length) {

                SortedDictionary<string, Container> dictionary = new SortedDictionary<string, Container>();

                // Retrieve values and load into a hash for sorting            
                foreach (RegistryValue value in values) {
                    dictionary.Add(value.Name.Replace("File", string.Empty),
                        new Container(value.Name, value.GetDataAsString()));
                }

                // Report
                foreach (KeyValuePair<string, Container> pair in dictionary) {
                    this.Reporter.Write("  " + pair.Value.Name + " -> " + pair.Value.Data);
                }

            } else {
                Library.WriteNoValue(KeyPath, this.Reporter);
            }            

            return true;
        }
    }
}
