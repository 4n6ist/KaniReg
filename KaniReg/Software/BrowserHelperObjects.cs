using System;
using System.Collections.Generic;


namespace KaniReg.Software {

    class BrowserHelperObjects : ClassBase {

        public BrowserHelperObjects(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects";
            Description = "使用しているブラウザのアドオン";
        }

        public override bool Process() {

            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            RegistryKey clsIdKey = null;
            string clsIdPath = string.Empty;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            if (null != subkeys && 0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {

                    if (subkey.Name.StartsWith("-")) { continue; }

                    clsIdPath = "Classes\\CLSID\\" + subkey.Name;
                    clsIdKey = RootKey.GetSubkey(clsIdPath);
                    if (null != clsIdKey) {
                        dictionary.Clear();
                        try {
                            dictionary.Add("clsId", clsIdKey.GetValue("(Default)").GetDataAsObject().ToString());
                        } catch(Exception ex) {
                            dictionary.Add("clsId", string.Empty);
                            Logger.Write(LogLevel.ERROR, "\tError getting Class name for CLSID\\" + clsIdKey.Name + " : " + ex.Message);
                        }

                        try {
                            dictionary.Add("module", clsIdKey.GetSubkey("InProcServer32").GetValue("(Default)").GetDataAsObject().ToString());
                        } catch(Exception ex) {
                            dictionary.Add("module", string.Empty);
                            Logger.Write(LogLevel.ERROR, "Error getting Module name for CLSID\\" + clsIdKey.Name + " : " + ex.Message);
                        }

                        try {
                            dictionary.Add("timestamp", Library.TransrateTimestamp(clsIdKey.GetSubkey("InProcServer32").Timestamp, TimeZoneBias, OutputUtc));
                        } catch(Exception ex) {
                            dictionary.Add("timestamp", string.Empty);
                            Logger.Write(LogLevel.ERROR, "\tError getting LastWrite time for CLSID\\" + clsIdKey.Name + " : " + ex.Message);
                        }

                        Reporter.Write(subkey.Name);
                        Reporter.Write("\tクラス       => " + dictionary["clsId"].ToString());
                        Reporter.Write("\tモジュール   => " + dictionary["module"].ToString());
                        Reporter.Write("\t最終更新日時 => " + dictionary["timestamp"].ToString());
                        Reporter.Write("");
                    } else {
                        Reporter.Write(clsIdPath + " は見つかりませんでした。");
                        Reporter.Write("");
                    }
                }
            } else {
                Reporter.Write(KeyPath + " にサブキーはありませんでした。 ブラウザヘルパーオブジェクトは使われていません。");
            }
            return true;
        }
    }
}
