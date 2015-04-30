using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.NtUser {

    class UserAssist : ClassBase {

        private const int OLD_LENGTH = 16;
        private const int WINDOWS7_LENGTH = 72;

        public UserAssist(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist";
            Description = "ユーザーアシストツールによる、実行ファイルの最終起動時刻、累積起動回数";
        }

        public override bool Process() {

            RegistryKey[] keys = Key.GetListOfSubkeys();

            foreach (RegistryKey key in keys) {

                RegistryKey subkey = key.GetSubkey("Count");

                if (null != subkey) {
                    Reporter.Write("Key=" + key.Name + " / Version=" + key.GetValue("Version").GetDataAsString() + "\r\n");

                    RegistryKey[] innerKeys = key.GetListOfSubkeys();

                    foreach (RegistryKey innerKey in innerKeys) {

                        RegistryValue[] values = innerKey.GetListOfValues();
                        if (null != values && 0 < values.Length) {

                            Dictionary<double, List<string>> dictionary = new Dictionary<double, List<string>>();
                            List<string> list;

                            string name;
                            byte[] data;
                            uint session = 0;
                            uint count = 0;
                            double filetime = 0;


                            foreach (RegistryValue value in values) {

                                name = value.Name;

                                try {
                                    data = (byte[])value.GetDataAsObject();
                                } catch (InvalidCastException invalid) {
                                    Logger.Write(LogLevel.ERROR, invalid.Message);
                                    continue;
                                }

                                if (null != data) {
                                    if (OLD_LENGTH == data.Length) {
                                        session = BitConverter.ToUInt32(data, 0);
                                        count = BitConverter.ToUInt32(data, 4);
                                        filetime = Library.CalculateTimestamp(BitConverter.ToUInt64(data, 8));

                                        // HRZRから始まる名前の物だけ処理
                                        if (name.StartsWith("HRZR")) {
                                            name = Library.TranseleteRot13(name, value.IsAscii);
                                        }

                                        if (5 < count) {
                                            count -= 5;
                                        }
                                    } else if (WINDOWS7_LENGTH == data.Length) {
                                        name = Library.TranseleteRot13(name, value.IsAscii);
                                        count = BitConverter.ToUInt32(data, 4);
                                        filetime = Library.CalculateTimestamp(BitConverter.ToUInt64(data, 60));
                                    }
                                }

                                if (dictionary.ContainsKey(filetime)) {
                                    list = dictionary[filetime];
                                    list.Add(name + " (" + count.ToString() + ")");
                                    dictionary[filetime] = list;
                                } else {
                                    list = new List<string>();
                                    list.Add(name + " (" + count.ToString() + ")");
                                    dictionary.Add(filetime, list);
                                }
                            }

                            // sort
                            List<KeyValuePair<double, List<string>>> sortList =
                                new List<KeyValuePair<double, List<string>>>(dictionary);

                            sortList.Sort(
                                delegate(KeyValuePair<double, List<string>> first, KeyValuePair<double, List<string>> next) {
                                    return next.Key.CompareTo(first.Key);
                                }
                            );

                            foreach (KeyValuePair<double, List<string>> pair in sortList) {
                                Reporter.Write(Library.TransrateTimestamp(pair.Key, TimeZoneBias, OutputUtc));

                                foreach (string value in pair.Value) {
                                    Reporter.Write("\t" + value);
                                }
                            }
                        } else {
                            Library.WriteNoValue(KeyPath, Reporter);
                        }


                    }

                }

                Reporter.Write("\r\n");
            }
            return true;
        }
    }
}
