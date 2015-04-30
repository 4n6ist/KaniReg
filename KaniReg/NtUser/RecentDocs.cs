using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.NtUser {

    class RecentDocs : ClassBase {

        public RecentDocs(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\RecentDocs";
            Description = "最近使ったファイル一覧";
        }

        public override bool Process() {

            Reporter.Write("RecentDocs - recentdocs");
            Reporter.Write("**All values printed in MRUList\\MRUListEx order.");
            Reporter.Write(KeyPath);

            // 共通メソッドをコール
            WriteKeyValues(KeyPath, Key);

            // Get RecentDocs subkeys' values        
            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {
                    Reporter.Write(KeyPath + "\\" + subkey.Name);

                    // 共通メソッドをコール
                    WriteKeyValues(KeyPath, subkey);
                }

            } else {
                Reporter.Write(KeyPath + "にはサブキーがありませんでした。");
            }

            return true;
        }
        

        private Dictionary<string, object> GetRdValues(RegistryKey key) {
            Dictionary<string,object> dictionary = new Dictionary<string,object>();
            List<string> dataList = new List<string>();
            RegistryValue[] values = key.GetListOfValues();

            if (null != values && 0 < values.Length) {
                string name = string.Empty;
                object data;
                char[] mruValues;
                uint index = 0;

                foreach (RegistryValue value in values) {
                    name = value.Name;
                    data = value.GetDataAsObject();

                    if (null != name) {
                        if (name.StartsWith("MRUList")) {

                            if ("MRUList".Equals(name)) {
                                mruValues = data.ToString().ToCharArray();
                                foreach (char item in mruValues) {
                                    dataList.Add(item.ToString());
                                }
                            } else if ("MRUListEx".Equals(name)) {
                                data = (byte[])data;
                                for (uint i = 0; i < ((byte[])data).Length / 4; i++) {
                                    index =  BitConverter.ToUInt32(Library.ExtractArrayElements((byte[])data, i * 4, 4), 0);
                                    if (0xFFFFFFFF != index) {
                                        dataList.Add(index.ToString());
                                    }
                                }
                            }
// ちょっと荒技･･･
                            if (!dictionary.ContainsKey(name)) {
                                dictionary.Add(name, dataList);
                            }
                        } else {
                            if (typeof(byte[]) == data.GetType()) {
                                string content = "";
                                try {
                                    List<byte[]> list = new List<byte[]>();
                                    if (0 == ((byte[])data).Length % 2) {
                                        list = Library.SplitByteData((byte[])data);
                                        if (0 < list.Count) {
                                            content = Encoding.Unicode.GetString(list[0]);
                                        } else {
                                            content = "";
                                        }
                                    } else {
                                        content = "(取得できなかったValue)";
                                        Logger.Write(LogLevel.WARNING, "異常値のためValueの取得ができませんでした。");
                                    }
                                } catch (Exception exception) {
                                    Logger.Write(LogLevel.WARNING, "Valueの取得ができませんでした。:" + exception.Message);
                                    content = "(取得できなかったValue)";
                                }

                                if (!dictionary.ContainsKey(name)) {
                                    dictionary.Add(name, content);
                                }

// ちょっと荒技･･･
                            } else {
                                dictionary.Add(name, data.ToString());
                            }
                        }
                    }
                }

                return dictionary;
            } else {
                return null;
            }
        }


        private void WriteKeyValues(string keyPath, RegistryKey key) {

            // report timestamp
            Reporter.Write("LastWrite Time " + Library.TransrateTimestamp(key.Timestamp, TimeZoneBias, OutputUtc));

            // Get RecentDocs values
            Dictionary<string, object> dictionary = GetRdValues(key);
            if (null != dictionary && 0 < dictionary.Count) {
                string tag = string.Empty;
                if (dictionary.ContainsKey("MRUListEx")) {
                    tag = "MRUListEx";
                } else if (dictionary.ContainsKey("MRUList")) {
                    tag = "MRUList";
                }

                List<string> list = (List<string>)dictionary[tag];
                foreach (string item in list) {
                    Reporter.Write("  " + item + " = " + dictionary[item].ToString());
                }
                Reporter.Write("");
            } else {
                Reporter.Write(keyPath + "にはValueがありませんでした。");
            }
        }
    }
}
