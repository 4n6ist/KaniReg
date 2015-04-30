using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.Software {

    class CurrentVersion : ClassBase {

        public CurrentVersion(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\Windows NT\CurrentVersion";
            Description = "現在のWindowsの情報";
        }

        public override bool Process() {

            this.Reporter.Write("Current Version");
            this.Reporter.Write(KeyPath);
            this.Reporter.Write("LastWrite Time " + Library.TransrateTimestamp(this.Key.Timestamp, this.TimeZoneBias, this.OutputUtc));
            this.Reporter.Write("");

            
            RegistryValue[] values = this.Key.GetListOfValues();
            if (null != values) {

                Dictionary<int, List<string>> dictionary = new Dictionary<int,List<string>>();
                List<string> list = new List<string>();
                string data = string.Empty;

                foreach (RegistryValue value in values) {

                    if (null != value.Name && 0 < value.Name.Length) {

                        if ("InstallDate".Equals(value.Name)) {
                            data = Library.TransrateTimestamp((uint)value.GetDataAsObject(), this.TimeZoneBias, this.OutputUtc);
                        } else if (3 == value.Type) {
                            data = Library.ByteArrayToString((byte[])value.GetDataAsObject(), " ");
                        } else {
                            data = value.GetDataAsObject().ToString();
                        }

                        data = value.Name + ":" + data;

                        if (dictionary.ContainsKey(data.Length)) {
                            list = dictionary[data.Length];
                            list.Add(data);
                            dictionary[data.Length] = list;
                        } else {
                            list = new List<string>();
                            list.Add(data);
                            dictionary.Add(data.Length, list);
                        }
                    }
                }

                List<KeyValuePair<int, List<string>>> sorted = new List<KeyValuePair<int, List<string>>>(dictionary); 

                sorted.Sort(
                    delegate(KeyValuePair<int, List<string>> first, KeyValuePair<int, List<string>> next) {
                        return first.Key.CompareTo(next.Key);
                    }
                );

                foreach (KeyValuePair<int, List<string>> pair in sorted) {
                    int count = 0;
                    foreach (string item in pair.Value) {
                        if (0 < count) {
                            this.Reporter.Write("");
                        }
                        this.Reporter.Write("  " + item);
                        count++;
                    }
                }    
            } else {
                Library.WriteNoValue(KeyPath, Reporter);
            }

            return true;
        }
    }
}
