using System;
using System.Collections.Generic;
using System.Text;


namespace KaniReg.Software {

    class PortableDevices : ClassBase {

        public PortableDevices(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\Windows Portable Devices\Devices";
            Description = "リムーバブルメディアの設定値";
        }

        public override bool Process() {

            this.Reporter.Write("Portable Devices");
            this.Reporter.Write(KeyPath);
            this.Reporter.Write("LastWrite Time " + Library.TransrateTimestamp(this.Key.Timestamp, this.TimeZoneBias, this.OutputUtc));
            this.Reporter.Write("");

            RegistryKey[] subkeys = this.Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {
                
                string friendlyName;
                string half;
                string[] profiles;
                foreach (RegistryKey subkey in subkeys) {
                    
                    friendlyName = string.Empty;
                    try {
                        friendlyName = subkey.GetValue("FriendlyName").GetDataAsObject().ToString();
                    } catch (Exception ex) {
                        this.Reporter.Write(subkey.Name + " key error: " + ex.Message);
                    }
                    
                    half = string.Empty;
                    if (subkey.Name.StartsWith("##")) {
                        // Windows XP
                        half = subkey.Name.Split(new string[] { "##" }, StringSplitOptions.None)[1];

                        if (subkey.Name.Contains(@"\?\?")) {
                            half = subkey.Name.Split(new string[] { @"\?\?" }, StringSplitOptions.None)[1];
                        }

                        profiles = half.Split('#');
                        this.Reporter.Write("Device    : " + ((1 < profiles.Length) ? profiles[1] : string.Empty));
                        this.Reporter.Write("LastWrite : " + Library.TransrateTimestamp(subkey.Timestamp, this.TimeZoneBias, this.OutputUtc));
                        this.Reporter.Write("SN        : " + ((2 < profiles.Length) ? profiles[2] : string.Empty));
                        this.Reporter.Write("Drive     : " + friendlyName);
                        this.Reporter.Write("");
                    } else {
                        // Windows 7
                        if (subkey.Name.StartsWith("USB")) {
                            profiles = subkey.Name.Split('#');
                            if (1 < profiles.Length) {
                                WriteProperties(profiles[1]);
                            }
                            if (2 < profiles.Length) {
                                WriteSerialNumber(profiles[2]);
                            }
                            this.Reporter.Write("Drive     : " + friendlyName);
                            this.Reporter.Write("LastWrite : " + Library.TransrateTimestamp(subkey.Timestamp, this.TimeZoneBias, this.OutputUtc));
                            this.Reporter.Write("");
                        } else {
                            profiles = subkey.Name.Split('#');
                            if (5 < profiles.Length) {
                                WriteProperties(profiles[5]);
                            }                           
                            if (6 < profiles.Length) {
                                WriteSerialNumber(profiles[6]);
                            }
                            this.Reporter.Write("Drive     : " + friendlyName);
                            this.Reporter.Write("LastWrite : " + Library.TransrateTimestamp(subkey.Timestamp, this.TimeZoneBias, this.OutputUtc));
                            this.Reporter.Write("");
                        }
                    }
                }
            } else {
                this.Reporter.Write(KeyPath + " has no subkeys.");
            }

            return true;
        }

        private void WriteProperties(string propertyColumn) {

            string[] properties = propertyColumn.Split('&');
            foreach (string property in properties) {
                string[] nameValue = property.Split('_');

                if (1 == nameValue.Length) {
                    this.Reporter.Write("Device    : " + property);
                } else {
                    StringBuilder builder = new StringBuilder();
                    for (int count = 1; nameValue.Length > count; count++) {
                        if (0 < builder.Length) {
                            builder.Append("_");
                        }
                        builder.Append(nameValue[count]);
                    }
                    this.Reporter.Write(nameValue[0].PadRight(10, ' ') + ": " + builder.ToString());
                }
            }
        }

        private void WriteSerialNumber(string serialNumberColumn) {
            this.Reporter.Write("SN       : " + ((serialNumberColumn.Contains("&")) ? serialNumberColumn.Split('&')[0] : serialNumberColumn));
        }
    }
}
