using System;
using System.Collections.Generic;


namespace KaniReg.Software {

    class Uninstall : ClassBase {

        public Uninstall(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\Windows\CurrentVersion\Uninstall";
            Description = "インストール履歴";
        }

        public override bool Process() {

            this.Reporter.Write("Uninstall");
            this.Reporter.Write(KeyPath);
            this.Reporter.Write("");
        
            Dictionary<double, List<string>> dictionary = new Dictionary<double,List<string>>();
            List<string> list;
            RegistryKey[] subkeys = this.Key.GetListOfSubkeys();
            string display = string.Empty;
            if (null != subkeys && 0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {
                    
                    display = string.Empty;
                    try {
                        display = subkey.GetValue("DisplayName").GetDataAsObject().ToString();
                    } catch {
                        // 放置
                    }

                    display = (string.Empty.Equals(display)) ? subkey.Name : display;

                    if (dictionary.ContainsKey(subkey.Timestamp)) {
                        list = dictionary[subkey.Timestamp];
                        list.Add(display);
                        dictionary[subkey.Timestamp] = list;
                    } else {
                        list = new List<string>();
                        list.Add(display);
                        dictionary.Add(subkey.Timestamp, list);
                    }
                }

                List<KeyValuePair<double, List<string>>> sorted = new List<KeyValuePair<double,List<string>>>(dictionary);

                sorted.Sort(
                    delegate (KeyValuePair<double, List<string>> first, KeyValuePair<double, List<string>> next) {
                        return next.Key.CompareTo(first.Key);
                    }
                );

                string precedent = string.Empty;
                string timestamp = string.Empty;
                foreach (KeyValuePair<double, List<string>> pair in sorted) {

                    timestamp = Library.TransrateTimestamp(pair.Key, this.TimeZoneBias, this.OutputUtc);
                    // TIMESTAMPラベルが変わるときだけ印字
                    if (!precedent.Equals(timestamp)) {
                        this.Reporter.Write(timestamp);
                        precedent = timestamp;
                    }

                    foreach (string item in pair.Value) {
                        this.Reporter.Write("\t" + item);
                    }
                }
            } else {
                this.Reporter.Write(KeyPath + "にはサブキーがありませんでした。");
            }

            return true;
        }
    }
}
