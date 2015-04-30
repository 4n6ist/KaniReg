using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KaniReg.NtUser {

    class Adobe : ClassBase {

        private const int CURRENT_VERSION = 9;

        private const string READER = "Acrobat Reader";
        private const string ACROBAT = "Adobe Acrobat";

        public Adobe(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }
        
        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Adobe";
            Description = "Adobe Reader 又は Acrobatを使用した直近の閲覧ファイル";
        }

        public override bool Process() {

            // 製品名保持用
            string productName = string.Empty;

            // First, let's find out which version of Adobe Acrobat Reader is installed
            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            RegistryKey key = null;
            foreach (RegistryKey subkey in subkeys) {
                if (READER.Equals(subkey.Name)) {
                    productName = READER;
                    key = subkey;
                    break;
                } else if (ACROBAT.Equals(subkey.Name)) {
                    productName = ACROBAT;
                    key = subkey;
                    break;
                }
            }

            if (null != key) {
                subkeys = key.GetListOfSubkeys();
                key = null;
                if (null != subkeys && 0 < subkeys.Length) {
                    foreach (RegistryKey subkey in subkeys) {
                        if (Regex.IsMatch(subkey.Name, @"[0-9]+\.[0-9]+")) {
                            key = subkey;
                            break;
                        }
                    }

                    // バージョンが見つかった場合は処理
                    if (null != key) {
                        Reporter.Write("Adobe は " + productName + " がインストールされており、バージョンは " + key.Name + " です。");
                        

                        // RecentFileを取得する
                        string keyPath = @"AVGeneral\cRecentFiles";

                        key = key.GetSubkey(keyPath);
                        if (null != key) {
                            Reporter.Write("最近更新したファイルのキーパス：" + key.Name + @"\" + keyPath);
                            Reporter.Write("最終更新日：" + Library.TransrateTimestamp(key.Timestamp, TimeZoneBias, OutputUtc));

                            // サブキーを取得
                            subkeys = key.GetListOfSubkeys();
                            if (null != subkeys && 0 < subkeys.Length) {
                                foreach (RegistryKey subkey in subkeys) {
                                    // なぜShift_JISで入ってるんだ…
                                    // 海外で使えんじゃないか。
                                    string data = Encoding.GetEncoding(0).GetString((byte[])subkey.GetValue("sDI").GetDataAsObject());
                                    data = data.Replace("\0", string.Empty);
                                    Reporter.Write(subkey.Name + "   " + Library.TransrateTimestamp(subkey.Timestamp, TimeZoneBias, OutputUtc) + "  " + data);
                                }
                            } else {
                                Reporter.Write(keyPath + " にはサブキーがありません。");
                            }
                        } else {
                            Reporter.Write("ファイルの編集がありません。");
                        }
                    } else {
                        // でなければ記録して終了
                        Reporter.Write("Adobe " + productName + " のバージョンは見つかりませんでした。");
                    }
                }
            }
   
            return true;
        }
    }
}
