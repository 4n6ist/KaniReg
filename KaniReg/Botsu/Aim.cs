using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.Botsu {
    class Aim : ClassBase {

        internal struct Conteiner {
            private string _name;
            private string _data;

            internal Conteiner(string name, string data) {
                _name = name;
                _data = data;
            }

            internal string Name {
                set { _name = value; }
                get { return _name; }
            }

            internal string Data {
                set { _data = value; }
                get { return _data; }
            }
        }

        public Aim(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            KeyPath = @"Software\America Online\AOL Instant Messenger (TM)\CurrentVersion\Users";
            Description = "AOL Instant Messengerの設定情報";
        }

        public override bool Process() {

            RegistryKey[] subkeys = Key.GetListOfSubkeys();

            if (null != subkeys && 0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {
                    string user = subkey.Name;
                    Reporter.Write("User: " + user + " [" + Library.TransrateTimestamp(subkey.Timestamp, TimeZoneBias, OutputUtc) + "]");

                    string LOGIN = "Login";
                    string RECENT = "recent IM ScreenNames";
                    string RECENT2 = "recent ScreenNames";

                    RegistryKey[] userKeys = subkey.GetListOfSubkeys();
                    foreach (RegistryKey userkey in userKeys) {
                        string userKeyName = userkey.Name;

                        userKeyName = userKeyName.Replace(LOGIN, string.Empty);
                        // See if we can get the encrypted password

                        if (0 < userKeyName.Length) {
                            string password = userkey.GetValue("Password1").GetDataAsObject().ToString();
                            if (null != password && !string.Empty.Equals(password)) {
                                Reporter.Write("Pwd: " + password);
                            }
                        }

                        // See if we can get recent folks they've chatted with...                    
                        if (RECENT.Equals(userKeyName) || RECENT2.Equals(userKeyName)) {

                            RegistryValue[] values = userkey.GetListOfValues();
                            if (null != values && 0 < values.Length) {
                                Reporter.Write(user + "\\" + userKeyName);

                                List<Conteiner> list = new List<Conteiner>();
                                foreach (RegistryValue value in values) {
                                    list.Add(new Conteiner(value.Name, value.GetDataAsObject().ToString()));
                                }

                                // ソートを行う
                                list.Sort(
                                    delegate(Conteiner first, Conteiner next) {
                                        return first.Name.CompareTo(next.Name);
                                    }
                                );

                                // Report
                                foreach (Conteiner container in list) {
                                    Reporter.Write("\t\t" + container.Name + " -> " + container.Data);
                                }
                            } else {
                                // No values                            
                            }
                        }
                    }
                    Reporter.Write("");
                }
            } else {
                Library.WriteNoValue(KeyPath, Reporter);
            }

            return true;
        }
    }
}
