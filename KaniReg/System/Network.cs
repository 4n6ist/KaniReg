using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.SysHive {

    class Network : ClassBase {

        public Network(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"ControlSet00" + Current + @"\Control\Network\{4D36E972-E325-11CE-BFC1-08002BE10318}";
            Description = "ネットワーク接続設定";
        }

        public override bool Process() {

            // Get all of the subkey names
            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {

                // connection values用
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                RegistryValue[] connectionValues;

                foreach (RegistryKey subkey in subkeys) {

                    dictionary.Clear();

                    // Descriptionsはスキップ
                    if ("Descriptions".Equals(subkey.Name)) { continue; }

                    RegistryKey connectionKey = Key.GetSubkey(subkey.Name + "\\Connection");
                    if (null != connectionKey) {
                        this.Reporter.Write("Interface " + subkey.Name);
                        this.Reporter.Write("LastWrite time " + Library.TransrateTimestamp(connectionKey.Timestamp, this.TimeZoneBias, this.OutputUtc));

                        connectionValues = connectionKey.GetListOfValues();
                        foreach (RegistryValue value in connectionValues) {
                            dictionary.Add(value.Name, value.GetDataAsObject().ToString());
                        }

                        this.Reporter.Write("\tName              = " + dictionary["Name"]);
                        if (dictionary.ContainsKey("PnpInstanceID")) {
                            this.Reporter.Write("\tPnpInstanceID      = " + dictionary["PnpInstanceID"]);
                        }

                        if (dictionary.ContainsKey("MediaSubType")) {
                            this.Reporter.Write("\tMediaSubType      = " + dictionary["MediaSubType"]);
                        }
                        if (dictionary.ContainsKey("IpCheckingEnabled")) {
                            this.Reporter.Write("\tIpCheckingEnabled = " + dictionary["IpCheckingEnabled"]);
                        }                          
                    }
                    this.Reporter.Write("");
                }
                    
            } else {
                this.Reporter.Write(KeyPath + "にはサブキーはありませんでした。");
            }            

            return true;
        }
    }
}
