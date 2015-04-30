using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.NtUser {

    class MountPoints2 : ClassBase {

        public MountPoints2(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\MountPoints2";
            Description = "";
        }

        public override bool Process() {
            RegistryKey[] subkeys = this.Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {

                Dictionary<string, double> volumeDictionary = new Dictionary<string, double>();
                Dictionary<string, double> driveDictionary = new Dictionary<string, double>();
                Dictionary<string, double> remoteDictionary = new Dictionary<string, double>();

                string name;
                foreach (RegistryKey subkey in subkeys) {
                    
                    name = subkey.Name;
                    System.Text.RegularExpressions.Regex regx = new System.Text.RegularExpressions.Regex(@"^[A-Z]");
                    if (name.StartsWith("{")) {
                        volumeDictionary.Add(name, subkey.Timestamp);
                    } else if (regx.IsMatch(name)) {
                        driveDictionary.Add(name, subkey.Timestamp);
                    } else if (name.StartsWith(@"#")) {
                        remoteDictionary.Add(name, subkey.Timestamp);
                    } else {
                        this.Reporter.Write("  Key name = " + name);
                    }
                }
                
                this.Reporter.Write("");
                this.Reporter.Write("  Drives:");
                foreach (KeyValuePair<string, double> pair in driveDictionary) {
                    this.Reporter.Write("    " + pair.Key + "  " + Library.TransrateTimestamp(pair.Value, this.TimeZoneBias, this.OutputUtc));
                }
                this.Reporter.Write("");
                this.Reporter.Write("  Volumes:");
                foreach (KeyValuePair<string, double> pair in volumeDictionary) {
                    this.Reporter.Write("    " + pair.Key + "  " + Library.TransrateTimestamp(pair.Value, this.TimeZoneBias, this.OutputUtc));
                }
                this.Reporter.Write("");
                this.Reporter.Write("  Remote Drives:");
                foreach (KeyValuePair<string, double> pair in remoteDictionary) {
                    this.Reporter.Write("    " + pair.Key + "  " + Library.TransrateTimestamp(pair.Value, this.TimeZoneBias, this.OutputUtc));
                }
            } else {
                Library.WriteNoValue(KeyPath, Reporter);
            }

            return true;
        }
    }
}
