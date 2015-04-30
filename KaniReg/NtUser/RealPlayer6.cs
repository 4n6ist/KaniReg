using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.NtUser {

    class RealPlayer6 : ClassBase {

        private const string TAG = "MostRecentClips";

        public RealPlayer6(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\RealNetworks\RealPlayer\6.0\Preferences";
            Description = "RealPlayer6の再生履歴";
        }

        public override bool Process() {

            Reporter.Write(KeyPath);
            Reporter.Write("LastWrite Time " + Library.TransrateTimestamp(Key.Timestamp, TimeZoneBias, OutputUtc));

            SortedDictionary<ushort, Container> dictionary = new SortedDictionary<ushort, Container>();

            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {
                    if (subkey.Name.StartsWith(TAG)) {
                        dictionary.Add(Convert.ToUInt16(subkey.Name.Replace(TAG, string.Empty)),
                            new Container(subkey.Name, subkey.GetValue("(Default)").GetDataAsString()));
                        //timestamp使ってないやん･･･
                        //$rpkeys{$num}{lastwrite} = $s->get_timestamp();
                    }
                }

                foreach (KeyValuePair<ushort, Container> pair in dictionary) {
                    Reporter.Write("\t" + pair.Value.Name + " -> " + pair.Value.Data);
                }
            } else {
                Reporter.Write(KeyPath + " にはサブキーはありませんでした。");
            }

            return true;
        }
    }
}
