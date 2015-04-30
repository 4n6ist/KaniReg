using System;
using System.Collections.Generic;


namespace KaniReg.Software {

    class WindowsRun : ClassBase {

        public WindowsRun(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Microsoft\Windows\CurrentVersion\Run";
            Description = "ログオン時自動実行プログラム";
        }

        public override bool Process() {

            // ソート用
            List<KeyValuePair<string, string>> list;
            
            Dictionary<string, string> dictionary = Library.ValuesToDictionary(Key);
            if (0 < dictionary.Count) {

                // sort
                list = new List<KeyValuePair<string, string>>(dictionary);
                list.Sort(
                    delegate(KeyValuePair<string, string> first, KeyValuePair<string, string> next) {
                        return first.Key.CompareTo(next.Key);
                    }
                );

                foreach (KeyValuePair<string, string> pair in list) {
                    Reporter.Write("\t" + pair.Key + " -> " + pair.Value);
                }
            } else {
                Library.WriteNoValue(KeyPath, Reporter);
            }
            
            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {
                // Win XP

                foreach (RegistryKey subkey in subkeys) {
                    Reporter.Write("");
                    Reporter.Write(KeyPath + "\\" + subkey.Name);
                    Reporter.Write("最終更新日時: " + Library.TransrateTimestamp(subkey.Timestamp, TimeZoneBias, OutputUtc));
                    dictionary = Library.ValuesToDictionary(subkey);

                    if (0 < dictionary.Count) {
                        // sort
                        list = new List<KeyValuePair<string, string>>(dictionary);
                        list.Sort(
                            delegate(KeyValuePair<string, string> first, KeyValuePair<string, string> next) {
                                return first.Key.CompareTo(next.Key);
                            }
                        );

                        foreach (KeyValuePair<string, string> pair in list) {
                            Reporter.Write("\t" + pair.Key + " -> " + pair.Value);
                        }
                    }
                }
            } else {
                // Win 7 ? NOOP
//                Reporter.Write("");
//                Library.WriteNoValue(KeyPath, Reporter);
            }

            return true;
        }
    }
}
