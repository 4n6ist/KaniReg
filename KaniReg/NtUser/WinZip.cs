using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.NtUser {

    class WinZip : ClassBase {

        public WinZip(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Nico Mak Computing\WinZip";
            Description = "WinZipの実行履歴";
        }

        public override bool Process() {

            RegistryKey[] subkeys = this.Key.GetListOfSubkeys();

            RegistryKey extractKey = null;
            RegistryKey filemenuKey = null;

            if (null != subkeys &&  0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {
                    if ("extract".Equals(subkey.Name)) {
                        extractKey = subkey;
                    } else if ("filemenu".Equals(subkey.Name)) {
                        filemenuKey = subkey;
                    }
                }
            }
            
            if (null != extractKey) {
                this.Reporter.Write(KeyPath + "\\extract  [" + Library.TransrateTimestamp(extractKey.Timestamp, this.TimeZoneBias, this.OutputUtc) + "]");
                this.WriteValues("extract", extractKey.GetListOfValues());
                this.Reporter.Write("");
            } else {
                this.Reporter.Write("extract キーは見つかりませんでした。");
            }
            
            if (null != filemenuKey) {
                this.Reporter.Write(KeyPath + "\\filemenu  [" + Library.TransrateTimestamp(filemenuKey.Timestamp, this.TimeZoneBias, this.OutputUtc) + "]");
                this.WriteValues("filemenu", filemenuKey.GetListOfValues());
                this.Reporter.Write("");
            }
            else {
                this.Reporter.Write("filemenu キーは見つかりませんでした。");
            }

            return true;
        }

        private void WriteValues(string tag, RegistryValue[] values) {

            SortedDictionary<string, string> dictionary = new SortedDictionary<string, string>();

            foreach (RegistryValue value in values) {
                dictionary.Add(value.Name.Replace(tag, string.Empty), value.GetDataAsString());
            }

            foreach (KeyValuePair<string, string> pair in dictionary) {
                this.Reporter.Write("  " + tag + pair.Key + " -> " + pair.Value);
            }
        }
    }
}
