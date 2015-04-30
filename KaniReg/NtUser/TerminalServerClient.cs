using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.NtUser {

    class TerminalServerClient : ClassBase {

        public TerminalServerClient(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Terminal Server Client\Default";
            Description = "リモートデスクトップアクセス状況";
        }

        public override bool Process() {

            RegistryValue[] values = this.Key.GetListOfValues();
            List<KeyValuePair<ushort, string>> list = new List<KeyValuePair<ushort, string>>();
            if (null != values && 0 < values.Length) {

                string tag;
                ushort serial;
                string name;
                foreach (RegistryValue value in values) {
                    name = value.Name;

                    if (name.StartsWith("MRU")) {
                        tag = name.Substring(3);
                        if (ushort.TryParse(tag, out serial)) {
                            list.Add(new KeyValuePair<ushort, string>(serial, value.GetDataAsObject().ToString()));
                        }
                    }
                }

                list.Sort(
                    delegate(KeyValuePair<ushort, string> first, KeyValuePair<ushort, string> next) {
                        return first.Key.CompareTo(next.Key);
                    }
                );

                foreach (KeyValuePair<ushort, string> pair in list) {
                    this.Reporter.Write("  MRU" + pair.Key.ToString() + " -> " + pair.Value);
                }
            } else {
                Library.WriteNoValue(KeyPath, Reporter);
            }

            return true;
        }
    }
}
