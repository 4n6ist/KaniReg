using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.NtUser {
    class Mmc : ClassBase {

        public Mmc(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Microsoft Management Console\Recent File List";
            Description = "MMC(Microsoft管理コンソール)によって操作したファイルの履歴";
        }

        public override bool Process() {

            RegistryValue[] values = Key.GetListOfValues();
            string source = string.Empty;
            ushort serial = 0;

            if (null != values && 0 < values.Length) {
                List<KeyValuePair<ushort, string>> list = new List<KeyValuePair<ushort, string>>();
                // Retrieve values and load into a hash for sorting            
                foreach (RegistryValue value in values) {
// Fileのみ(数字なし)がナゾ
                    if (value.Name.StartsWith("File")) {
                        source = value.Name.Substring(4);
                        if (ushort.TryParse(source, out serial)) {
                            list.Add(new KeyValuePair<ushort, string>(serial, value.GetDataAsObject().ToString()));
                        }
                    }
                }

                // ソートを行う
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
                Library.WriteNoValue(KeyPath, Reporter);
            }

            return true;
        }
    }
}
