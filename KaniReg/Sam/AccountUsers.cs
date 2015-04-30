using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.Sam {

    class AccountUsers : ClassBase {

        public AccountUsers(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"SAM\Domains\Account\Users";
            Description = "ユーザーアカウント";
        }

        public override bool Process() {

            // サブキーを取得
            RegistryKey[] subkeys = this.Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {

                // 代入用変数
                RegistryValue value;
                byte[] data;
                Dictionary<string, string> dictionary = new Dictionary<string,string>();

                // message
                Dictionary<ushort, string> messageDictionary = new Dictionary<ushort, string>();
                messageDictionary.Add(0x0001, "アカウント無効");  // Account Disabled
                messageDictionary.Add(0x0002, "ホームディレクトリ必要");  // Home directory required
                messageDictionary.Add(0x0004, "パスワード認証不要");  // Password not required
                messageDictionary.Add(0x0008, "一時的に重複したアカウント");  // Temporary duplicate account
                messageDictionary.Add(0x0010, "普通ユーザーアカウント");  // Normal user account
                messageDictionary.Add(0x0020, "MNS ログオンユーザーアカウント");  // MNS logon user account"
                messageDictionary.Add(0x0040, "複数ドメイン間信頼済アカウント");  // Interdomain trust account
                messageDictionary.Add(0x0080, "ワークステーション信頼済アカウント");  // Workstation trust account
                messageDictionary.Add(0x0100, "サーバー信頼済アカウント");  // Server trust account
                messageDictionary.Add(0x0200, "パスワード有効");  // Password does not expire
                messageDictionary.Add(0x0400, "自動ロック");  // Account auto locked


                foreach (RegistryKey subkey in subkeys) {
                    //my $ts  = subkey.Timestamp;
                    if (subkey.Name.StartsWith("0000")) {    
                        value = subkey.GetValue("V");
                        data = (byte[])value.GetDataAsObject();
                        dictionary = this.ParseV(data);

                        this.Reporter.Write("ユーザー名           : " + dictionary["name"] + " [" + subkey.Name.Substring(4) + "]");
                        this.Reporter.Write("フルネーム           : " + dictionary["fullname"]);
                        this.Reporter.Write("コメント             : " + dictionary["comment"]);
                        
                        value = subkey.GetValue("F");
                        data = (byte[])value.GetDataAsObject();
                        dictionary = this.ParseF(data);
                        this.Reporter.Write("最終ログイン日時     : " + dictionary["lastLoginDate"]);
                        this.Reporter.Write("パスワード再設定日時 : " + dictionary["resetDate"]);
                        this.Reporter.Write("パスワード失敗日時   : " + dictionary["failDate"]); ;
                        this.Reporter.Write("ログイン回数         : " + dictionary["loginCount"]);

                        foreach(KeyValuePair<ushort, string> pair in messageDictionary) {

                            if (pair.Key == (pair.Key & Convert.ToUInt16(dictionary["code"]))) {
                                this.Reporter.Write("  --> " + pair.Value);
                            }
                        }

                        this.Reporter.Write("");
                    }
                }
            }

            return true;
        }

        private Dictionary<string, string> ParseF(byte[] bytes) {

            Dictionary<string, string> dictinary = new Dictionary<string,string>();

            ulong filetime = 0;
            
            // last login date    
            filetime = BitConverter.ToUInt64(Library.ExtractArrayElements(bytes, 8, 8), 0);
            dictinary.Add("lastLoginDate", Library.TransrateTimestamp(Library.CalculateTimestamp(filetime), this.TimeZoneBias, this.OutputUtc));
            // password reset/acct creation
            filetime = BitConverter.ToUInt64(Library.ExtractArrayElements(bytes, 24, 8), 0);
            dictinary.Add("resetDate", Library.TransrateTimestamp(Library.CalculateTimestamp(filetime), this.TimeZoneBias, this.OutputUtc));
            // Account expires
            filetime = BitConverter.ToUInt64(Library.ExtractArrayElements(bytes, 32, 8), 0);
            string expireDate = string.Empty;
// 値がこれでOKかは不安
            if (filetime != 0x7FFFFFFFFFFFFFFF) {
                expireDate = Library.TransrateTimestamp(Library.CalculateTimestamp(filetime), this.TimeZoneBias, this.OutputUtc);
            }
            dictinary.Add("expireDate", expireDate);
            
            // Incorrect password     
            filetime = BitConverter.ToUInt64(Library.ExtractArrayElements(bytes, 40, 8), 0);
            dictinary.Add("failDate", Library.TransrateTimestamp(Library.CalculateTimestamp(filetime), this.TimeZoneBias, this.OutputUtc));
            dictinary.Add("rid", BitConverter.ToUInt32(Library.ExtractArrayElements(bytes, 48, 4), 0).ToString());
            dictinary.Add("code", BitConverter.ToUInt16(Library.ExtractArrayElements(bytes, 56, 2), 0).ToString());
            dictinary.Add("failedCount", BitConverter.ToUInt16(Library.ExtractArrayElements(bytes, 64, 2), 0).ToString());
            dictinary.Add("loginCount", BitConverter.ToUInt16(Library.ExtractArrayElements(bytes, 66, 2), 0).ToString());

            return dictinary;
        }

        private Dictionary<string, string> ParseV(byte[] bytes) {
            Dictionary<string, string> dictinary = new Dictionary<string,string>();
            byte[] header = Library.ExtractArrayElements(bytes, 0, 44);
            List<uint> list = new List<uint>();
            for (uint index = 0; index < header.Length; index+=4) {
                list.Add(BitConverter.ToUInt32(Library.ExtractArrayElements(header, index, 4), 0));
            }

            dictinary.Add("name", Encoding.Unicode.GetString(Library.ExtractArrayElements(bytes, list[3] + 0xCC, list[4])));

            string fullname = string.Empty;
            if (list[7] > 0) {
                 fullname = Encoding.Unicode.GetString(Library.ExtractArrayElements(bytes, list[6] + 0xCC, list[7]));
            }
            dictinary.Add("fullname", fullname);

            string comment = string.Empty;
            if (list[10] > 0) {
                comment = Encoding.Unicode.GetString(Library.ExtractArrayElements(bytes, list[9] + 0xCC, list[10]));
            }
            dictinary.Add("comment", comment);

            return dictinary;
        }
    }
}
