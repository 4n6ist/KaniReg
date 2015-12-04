using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.Sam {

    class BuiltinAliases : ClassBase {

        public BuiltinAliases(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            KeyPath = @"SAM\Domains\Builtin\Aliases";
            Description = "定義済みのグループ名エイリアスの情報";
        }

        public override bool Process() {

            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {

                foreach (RegistryKey subkey in subkeys) {
                    if (subkey.Name.StartsWith("0000")) {

                        RegistryValue value = subkey.GetValue("C");
                        Dictionary<string, string> dictionary = ParseC(value.Data);
                        Reporter.Write("グループ名    : " + dictionary["groupName"] + " [" + dictionary["numUsers"] + "]");
                        Reporter.Write("最終更新日時  : " + Library.TransrateTimestamp(subkey.Timestamp, TimeZoneBias, OutputUtc));
                        Reporter.Write("コメント      : " + dictionary["comment"]);
                        if (0 == Convert.ToUInt32(dictionary["numUsers"])) {
                            Reporter.Write("Users         : なし");
                        } else {
                            List<string> dataList = ParseCUsers(value.Data);
                            if (dataList.Count != Convert.ToUInt32(dictionary["numUsers"])) {
                                Reporter.Write("ParseC実行結果は [" + dictionary["numUsers"] + "]; ParseCUsers実行結果は [" + dataList.Count.ToString() + "]");
                            }
                            Reporter.Write("Users :");
                            foreach (string sid in dataList) {
                                Reporter.Write("  " + sid);
                            }

                        }
                        Reporter.Write("");
                                            }
                }

                Reporter.Write("Analysis Tips:");
                Reporter.Write(" - Well-Known SIDは http://support.microsoft.com/kb/243330 で確認できます。");
                Reporter.Write("     - S-1-5-4  = Interactive");
                Reporter.Write("     - S-1-5-11 = Authenticated Users");
                Reporter.Write(" - User SIDはProfileList pluginの出力結果と照合して利用してください。");
                Reporter.Write("");
            } else {
                Reporter.Write(KeyPath + " にはサブキーがありませんでした。");
            }

            return true;
        }

        private Dictionary<string, string> ParseC(byte[] bytes) {

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            List<uint> list = new List<uint>();
            byte[] header = Library.ExtractArrayElements(bytes, 0, 0x34);
            for (ushort index = 0; index < header.Length; index+=4) {
                list.Add(BitConverter.ToUInt32(Library.ExtractArrayElements(header, index, 4), 0));
            }
            
            dictionary.Add("groupName", Encoding.Unicode.GetString(Library.ExtractArrayElements(bytes ,(0x34 + list[4]),list[5])));
            dictionary.Add("comment", Encoding.Unicode.GetString(Library.ExtractArrayElements(bytes ,(0x34 + list[7]),list[8])));
            dictionary.Add("numUsers", list[12].ToString());

            return dictionary;
        }

        private List<string> ParseCUsers(byte[] bytes) {

            byte[] header = Library.ExtractArrayElements(bytes, 0, 0x34);
            List<uint> list = new List<uint>();
            for (ushort index = 0; index < header.Length; index+=4) {
                list.Add(BitConverter.ToUInt32(Library.ExtractArrayElements(header, index, 4), 0));
            }
                      
            List<string> sidList = new List<string>();
            if (0 < list[12]) {
                uint offset = 0;
                uint count = 0;
                uint signature = 0;
                byte flag = 0;
                for (uint index = 0; index < list[12]; index++) {
                    offset = list[10] + 52 + count;
                    signature = BitConverter.ToUInt32(Library.ExtractArrayElements(bytes, offset, 4), 0);
                    
                    if (0x101 == signature) {
                        flag = Library.ExtractArrayElements(bytes, offset, 1)[0];
                        if (0 == flag) offset++;
                        sidList.Add(TransrateSid(Library.ExtractArrayElements(bytes, offset, 12)));
                        count += 12;
                    } else if (0x501 == signature) {
                        sidList.Add(TransrateSid(Library.ExtractArrayElements(bytes, offset, 28)));
                        count += 28;
                    }
                }
            }
            return sidList;
        }

        private string TransrateSid(byte[] bytes) {
            string revision;
//            my $dashes; 使ってないやん
            string hexString = string.Empty;
            string id = string.Empty;
            string sub = string.Empty;
            List<char> replaced;
            StringBuilder builder;
            string rid = string.Empty;
            string result = string.Empty;

            if (12 > bytes.Length) {
                // Is a SID ever less than 12 bytes?
                // return "SID less than 12 bytes";
                Logger.Write(LogLevel.WARNING, KeyPath + "にてSIDが12バイトに足りませんでした : " + Library.ByteArrayToString(bytes, " "));
                result = null;
            } else if (12 == bytes.Length) {
                revision = Library.ExtractArrayElements(bytes,0,1)[0].ToString();
//                $dashes   = unpack("C",Library.ExtractArrayElements(bytes,1,1));
                hexString = Library.ByteArrayToHexString(Library.ExtractArrayElements(bytes, 2, 6), string.Empty);
                replaced = new List<char>(hexString.ToCharArray());
                builder = new StringBuilder();
                while ('0'.Equals(replaced[0])) {
                    replaced.RemoveAt(0);
                }
                id = new String(replaced.ToArray());
                sub   = BitConverter.ToUInt32(Library.ExtractArrayElements(bytes,8,4), 0).ToString();
                result = "S-" + revision + "-" + id + "-" + sub;
            } else if (12 < bytes.Length) {
                revision = Library.ExtractArrayElements(bytes,0,1)[0].ToString();
//                $dashes   = unpack("C",Library.ExtractArrayElements(bytes,1,1));
                hexString = Library.ByteArrayToHexString(Library.ExtractArrayElements(bytes,2,6), string.Empty);
                replaced = new List<char>(hexString.ToCharArray());
                while ('0'.Equals(replaced[0])) {
                    replaced.RemoveAt(0);
                }
                id = new String(replaced.ToArray());
                // SUBを組み立て
                builder = new StringBuilder();
                for (ushort index = 8; index < bytes.Length - 4; index+=4) {
                    if (0 < builder.Length) { builder.Append("-"); }
                    builder.Append(BitConverter.ToUInt32((Library.ExtractArrayElements(bytes, index, 4)), 0).ToString());
                }
                sub = builder.ToString();
                rid   = BitConverter.ToUInt32((Library.ExtractArrayElements(bytes, 24, 4)), 0).ToString();
                result = "S-" + revision + "-" + id + "-" + sub + "-" + rid;
            }

            // 
            return result;
        }
    }
}
