using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.Botsu {
    class Applets : ClassBase {

        public Applets(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            KeyPath = @"";
            Description = "";
        }

        public override bool Process() {

            RegistryKey[] keys = Key.GetListOfSubkeys();
            SortedDictionary<int, string> dictionary = new SortedDictionary<int,string>();

            foreach (RegistryKey key in keys) {
                if (!"Regedit".Equals(key.Name)) {
                    RegistryKey listKey = key.GetSubkey("Recent File List");
                    if (null != listKey) {
                        RegistryValue[] values = listKey.GetListOfValues();
                        foreach (RegistryValue value in values) {
                            if (value.Name.StartsWith("File")) {
                                dictionary.Add(int.Parse(value.Name.Replace("File", "")), value.GetDataAsString());
                            }
                        }

                        Reporter.Write("[" + key.Name + "]");
                        Reporter.Write("LastWrite " + Library.TransrateTimestamp(key.Timestamp, TimeZoneBias, OutputUtc));
                        foreach (KeyValuePair<int, string> pair in dictionary) {
                            Reporter.Write("  " + pair.Key.ToString() + " -> " + pair.Value);
                        }
                    }

                } else {
                    RegistryValue value = key.GetValue("LastKey");
                    if (null != value) {
                        Reporter.Write("[Regedit]");
                        Reporter.Write("LastWrite " + Library.TransrateTimestamp(key.Timestamp, TimeZoneBias, OutputUtc));
                        Reporter.Write("Regedit LastKey Value->" + value.GetDataAsString());
                    }
                }
                Reporter.Write("\r\n");
            }

            return true;
        }
    }
}
