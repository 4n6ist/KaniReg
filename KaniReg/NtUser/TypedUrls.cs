using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.NtUser {

    class TypedUrls : ClassBase {

        public TypedUrls(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Internet Explorer\TypedURLs";
            Description = "Internet Explorerのアドレスバー履歴";
        }

        public override bool Process() {

            RegistryValue[] values = Key.GetListOfValues();
            if (null != values && 0 < values.Length) {

                SortedDictionary<ushort, Container> dictionary = new SortedDictionary<ushort, Container>();

                // Retrieve values and load into a hash for sorting
                ushort tag = 0;
                foreach (RegistryValue value in values) {
                    // urlで始まるValueのみを対象とする
                    if (value.Name.StartsWith("url")) {
                        try {
                            tag = Convert.ToUInt16(value.Name.Remove(0,3));
                        } catch (Exception exception) {
                            // 一応Invalid CastやNumber Exception等にも備える
                            Logger.Write(LogLevel.ERROR, exception.Message);
                            continue;
                        }
                        dictionary.Add(tag, new Container(value.Name, value.GetDataAsString()));
                    } else {
                        Logger.Write(LogLevel.INFO, @"Software\Microsoft\Internet Explorer\TypedURLsの" + value.Name + "は適合するValueNameでないためスキップしました。");
                    }
                }

                foreach (KeyValuePair<ushort, Container> pair in dictionary) {
                    Reporter.Write("  " + pair.Value.Name + " -> " + pair.Value.Data);
                }
            } else {
                Library.WriteNoValue(KeyPath, Reporter);
            }

            return true;
        }
    }
}
