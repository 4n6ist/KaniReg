using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.SysHive {

    class FirewallPolicy : ClassBase {

        public FirewallPolicy(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"ControlSet00" + Current + @"\Services\SharedAccess\Parameters\FirewallPolicy";
            Description = "Windowsファイアーウォール設定";
        }

        public override bool Process() {

            string[] profiles = {"DomainProfile", "StandardProfile"};
            RegistryValue[] values = null;
            Dictionary<string, string> dictionary = new Dictionary<string,string>();
            RegistryKey subkey;

            foreach (string profile in profiles) {
                dictionary.Clear();
                RegistryKey profileKey = Key.GetSubkey(profile);
                if (null != profileKey) {
                    Reporter.Write("Subkey: " + profile);
                    Reporter.Write("LastWrite Time " + Library.TransrateTimestamp(profileKey.Timestamp, TimeZoneBias, OutputUtc));

                    dictionary = Library.ValuesToDictionary(profileKey);
                    values = profileKey.GetListOfValues();

                    if (0 < dictionary.Count) {
                        WriteValues(dictionary);
                    } else {
                        // Reporter.Write(path + " has no values.");
                    }
                    
                    string[] tags = {"RemoteAdminSettings", 
                                   "IcmpSettings",
                                   "GloballyOpenPorts\\List",
                                   "AuthorizedApplications\\List"};
                
                    foreach (string tag in tags) {
                        dictionary.Clear();
                        subkey = profileKey.GetSubkey(tag);
                        dictionary = Library.ValuesToDictionary(subkey);
                        if (0 < dictionary.Count) {
                            Reporter.Write("");
                            Reporter.Write(profile + @"\" + tag);
                            Reporter.Write("LastWrite Time " + Library.TransrateTimestamp(subkey.Timestamp, TimeZoneBias, OutputUtc));
                            WriteValues(dictionary);
                        }
                    }
                } else {
                    Reporter.Write(profile + " は見つかりませんでした。");
                }
            }

            return true;
        }

        private void WriteValues(Dictionary<string, string> dictionary) {
            foreach (KeyValuePair<string, string> pair in dictionary) {
                Reporter.Write("\t" + pair.Key + " -> " + pair.Value);
            }
        }
    }
}
