using System;
using System.Collections.Generic;


namespace KaniReg.Software {

    class AppPaths : ClassBase {

        public AppPaths(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\Windows\CurrentVersion\App Paths";
            Description = "インストールされたアプリケーションのパス";
        }

        public override bool Process() {

            Dictionary<double, List<string>> dictionary = new Dictionary<double, List<string>>();
            List<string> list;
            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            RegistryValue value = null;
            string data = string.Empty;
            if (null != subkeys && 0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {

                    value = subkey.GetValue("(Default)");
                    if (null != value) {
                        data = value.GetDataAsObject().ToString();
                    } else {
                        data = string.Empty;
                    }
                    data = subkey.Name + " [" + data + "]";

                    if (dictionary.ContainsKey(subkey.Timestamp)) {
                        list = dictionary[subkey.Timestamp];
                        list.Add(data);
                        dictionary[subkey.Timestamp] = list;
                    } else {
                        list = new List<string>();
                        list.Add(data);
                        dictionary.Add(subkey.Timestamp, list);
                    }
                }
                
                List<KeyValuePair<double, List<string>>> sorted = new List<KeyValuePair<double, List<string>>>(dictionary);
                sorted.Sort(
                    delegate (KeyValuePair<double, List<string>> first, KeyValuePair<double, List<string>> next) {
                        return next.Key.CompareTo(first.Key);
                    }
                );

                string precedent = string.Empty;
                string gmtdate = string.Empty;
                foreach (KeyValuePair<double, List<string>> pair in sorted) {
                    gmtdate = Library.TransrateTimestamp(pair.Key, TimeZoneBias, OutputUtc);
                    if (!precedent.Equals(gmtdate)) {
                        Reporter.Write(gmtdate);
                        precedent = gmtdate;
                    }
                    foreach (string item in pair.Value) {
                        Reporter.Write("  " + item);
                    }
                }
            } else {
                Reporter.Write(KeyPath + " にはサブキーがありませんでした。");
            }

            return true;
        }
    }
}
