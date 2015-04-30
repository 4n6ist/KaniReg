using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.NtUser {
    class Acmru : ClassBase {

        public Acmru(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Search Assistant\ACMru";
            Description = "Explorer検索におけるオートコンプリートの履歴\r\n  ->Vista/7の場合はこのKeyがないため、WordWheelQueryを参照してください。";
        }

        public override bool Process() {

            RegistryKey[] subkeys = Key.GetListOfSubkeys();

            if (null != subkeys && 0 < subkeys.Length) {

                foreach (RegistryKey subkey in subkeys) {
                    Reporter.Write(subkey.Name + "[" + Library.TransrateTimestamp(Key.Timestamp, TimeZoneBias, OutputUtc) + "]");

                    subkey.Logger = Logger;
                    RegistryValue[] values = subkey.GetListOfValues();

                    Dictionary<string, string> valueDictionary = new Dictionary<string, string>();
                    foreach (RegistryValue value in values) {
                        valueDictionary.Add(value.Name, value.GetDataAsObject().ToString());
                    }

                    List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>(valueDictionary);
                    list.Sort(
                        delegate(KeyValuePair<string, string> firstPair,
                            KeyValuePair<string, string> nextPair) {
                            return firstPair.Key.CompareTo(nextPair.Key);
                        }
                    );

                    foreach (KeyValuePair<string, string> pair in list) {
                        Reporter.Write("\t" + pair.Key + " -> " + pair.Value); 
                    }
                    Reporter.Write("");
                }
            } else {
                Library.WriteNoValue(KeyPath, Reporter);
                Reporter.Write("Valueがありませんでした。");
            }

            return true;
        }
    }
}
