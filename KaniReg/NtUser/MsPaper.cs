using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.NtUser {

    class MsPaper : ClassBase {

        public MsPaper(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
//            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComputerDescriptions";
            Description = "MSPaperのファイル一覧";
        }

        public override bool Process() {

            Reporter.Write("MsPaper");
            string keyPath = "Software\\Microsoft";
            RegistryKey key = RootKey.GetSubkey(keyPath);

            if (null != key) {

                RegistryKey[] subkeys = key.GetListOfSubkeys();

                if (null != subkeys && 0 < subkeys.Length) {

                    bool hasValue = false;
                    foreach (RegistryKey subkey in subkeys) {
                        string name = subkey.Name;
                        if (name.StartsWith("MSPaper")) {

                            hasValue = true;
                            string mspKeyPath = subkey.Name + "\\Recent File List";
                            RegistryKey mspKey = key.GetSubkey(mspKeyPath);
                            if (null != mspKey) {
                                Reporter.Write("表す値：MSPaperの直近の履歴");
                                Reporter.Write("キーのパス：" + keyPath + "\\" + mspKeyPath);
                                Reporter.Write("最終更新日：" + Library.TransrateTimestamp(mspKey.Timestamp, TimeZoneBias, OutputUtc));

                                RegistryValue[] values = mspKey.GetListOfValues();
                                if (null != values && 0 < values.Length) {

                                    List<KeyValuePair<ushort, string>> list = new List<KeyValuePair<ushort, string>>();

                                    // Retrieve values and load into a hash for sorting 
                                    string mspName;
                                    string source = string.Empty;
                                    ushort serial;
                                    foreach (RegistryValue value in values) {
                                        mspName = value.Name;
                                        if (mspName.StartsWith("File")) {
                                            source = mspName.Substring(4);
                                            if (ushort.TryParse(source, out serial)) {
                                                list.Add(new KeyValuePair<ushort, string>(serial, value.GetDataAsObject().ToString()));
                                            }
                                        }
                                    }

                                    list.Sort(
                                        delegate(KeyValuePair<ushort, string> first, KeyValuePair<ushort, string> next) {
                                            return first.Key.CompareTo(next.Key);
                                        }
                                    );

                                    // Print sorted content to report file            
                                    foreach (KeyValuePair<ushort, string> pair in list) {
                                        Reporter.Write("  File" + pair.Key.ToString() + " -> " + pair.Value);
                                    }
                                } else {
                                    Reporter.Write(keyPath + "\\" + mspKeyPath + " にはVALUEがありませんでした。");
                                    Reporter.Write("");
                                }
                            } else {
                                Reporter.Write(keyPath + "\\" + mspKeyPath + " キーは見つかりませんでした。");
                                Reporter.Write("");
                            }
                        }
                    }
                    if (!hasValue) {
                        Reporter.Write("SOFTWARE\\Microsoft\\MSPaper* から始まるキーは見つかりませんでした。");
                        Reporter.Write("");
                    }
                } else {
                    // そんな状況ありえへんと思うが
                    Reporter.Write(keyPath + " にはサブキーがありませんでした。");
                }
            } else {
                // そんな状況ありえへんと思うが
                Reporter.Write(keyPath + " キーは見つかりませんでした。");
            }

            return true;
        }
    }
}
