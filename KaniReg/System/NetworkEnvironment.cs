using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.SysHive {

    class NetworkEnvironment : ClassBase {

        public NetworkEnvironment(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = "ControlSet00" + Current + @"\Control\Network\{4D36E972-E325-11CE-BFC1-08002BE10318}";
            Description = "ネットワーク環境";
        }

        public override bool Process() {

            // Get all of the subkey names
            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {

                RegistryValue[] values;
                Dictionary<string, TimestampContainer2> dictionary = new Dictionary<string,TimestampContainer2>();
                Dictionary<string, string> innerDictionary = new Dictionary<string,string>() ;

                foreach (RegistryKey subkey in subkeys) {

                    if ("Descriptions".Equals(subkey.Name)) { continue; }

                    RegistryKey connectionKey = Key.GetSubkey(subkey.Name + "\\Connection");

                    if (null != connectionKey) {

                        innerDictionary.Clear();                         

                        values = connectionKey.GetListOfValues();
                        foreach (RegistryValue value in values) {
                            innerDictionary.Add(value.Name, value.GetDataAsObject().ToString());
                        }

                        // See what the active NICs were on the system; "active" based on PnpInstanceID having
                        // a string value
                        // Get the GUID of the interface, the name, and the LastWrite time of the Connection
                        // key                        
                        if (innerDictionary.ContainsKey("PnpInstanceID") &&
                            !string.Empty.Equals(innerDictionary["PnpInstanceID"])) {
                            dictionary.Add(subkey.Name,
                                new TimestampContainer2(innerDictionary["Name"], connectionKey.Timestamp));
                        }
                    }
                }

                Reporter.Write("");

                // access the Tcpip Services key to get the IP address information    
                if (0 < dictionary.Count) {
                    RegistryKey interfaceKey = RootKey.GetSubkey(@"Services\Tcpip\Parameters\Interfaces");

                    if (null != interfaceKey) {

                        Reporter.Write(interfaceKey.KeyPath);
                        Reporter.Write("最終更新日時：" + Library.TransrateTimestamp(interfaceKey.Timestamp, TimeZoneBias, OutputUtc));
                        Reporter.Write("");

                        List<string> list = new List<string>();

                        // Dump the names of the subkeys under Parameters\Interfaces into a hash            
                        subkeys = interfaceKey.GetListOfSubkeys();
                        if (null != subkeys && 0 < subkeys.Length) {
                            foreach (RegistryKey subkey in subkeys) {
                                list.Add(subkey.Name);
                            }
                        }
                           
                        RegistryKey propertyKey;
                        object data = null;
                        string replacedData = string.Empty;
                        StringBuilder builder = new StringBuilder();
                        foreach (KeyValuePair<string, TimestampContainer2> pair in dictionary) {
                            if (list.Contains(pair.Key)) {

                                propertyKey = interfaceKey.GetSubkey(pair.Key);
                                Reporter.Write("Interface " + pair.Key);
                                Reporter.Write("Name: " + pair.Value.Name);
                                Reporter.Write("Control\\Network キーの最終更新日時：" + Library.TransrateTimestamp(pair.Value.Timestamp, TimeZoneBias, OutputUtc));
                                Reporter.Write("Services\\Tcpip キーの最終更新日時：" + Library.TransrateTimestamp(propertyKey.Timestamp, TimeZoneBias, OutputUtc));
                                values = propertyKey.GetListOfValues();
                                    
                                innerDictionary.Clear();

                                foreach (RegistryValue value in values) {
                                    if (value.Name.ToUpper().Contains("DHCP") || "IPAddressv".Equals(value.Name) ||
                                        "SubnetMask".Equals(value.Name) || "DefaultGateway".Equals(value.Name)) {
                                            
                                        data = value.GetDataAsObject();

                                        if (typeof(string[]).Equals(data.GetType())) {

                                            builder = new StringBuilder();
                                            foreach (string item in (string[])data) {
                                                if (0 < builder.Length) { builder.Append(","); }
                                                builder.Append(item);
                                            }

                                            replacedData = builder.ToString();
                                        } else {
                                            replacedData = data.ToString();
                                        }

                                        innerDictionary.Add(value.Name, replacedData);
                                    }
                                }
                                    
                                if (innerDictionary.ContainsKey("EnableDHCP") && "1".Equals(innerDictionary["EnableDHCP"])) {
                                    Reporter.Write("\tDhcpDomain     = " + EvaluateDictionary(innerDictionary, "DhcpDomain"));
                                    Reporter.Write("\tDhcpIPAddress  = " + EvaluateDictionary(innerDictionary, "DhcpIPAddress"));
                                    Reporter.Write("\tDhcpSubnetMask = " + EvaluateDictionary(innerDictionary, "DhcpSubnetMask"));
                                    Reporter.Write("\tDhcpNameServer = " + EvaluateDictionary(innerDictionary, "DhcpNameServer"));
                                    Reporter.Write("\tDhcpServer     = " + EvaluateDictionary(innerDictionary, "DhcpServer"));
                                } else {
                                    Reporter.Write("\tIPAddress      = " + EvaluateDictionary(innerDictionary, "IPAddressv"));
                                    Reporter.Write("\tSubnetMask     = " + EvaluateDictionary(innerDictionary, "SubnetMask"));
                                    Reporter.Write("\tDefaultGateway = " + EvaluateDictionary(innerDictionary, "DefaultGateway"));
                                }
                                    
                            } else {
                                Reporter.Write("インターフェイス「" + pair.Key + "」は " + interfaceKey.KeyPath + " にはありませんでした。");
                            }
                            Reporter.Write("");
                        }
                    }
                } else {
                    //Reporter.Write("No active network interface cards were found.");
                    //Logger.Write("No active network interface cards were found.");
                    Reporter.Write("有効なNICは見つかりませんでした。");
                }
            } else {
                Reporter.Write(KeyPath + " にはサブキーがありませんでした。");
            }            


            return true;
        }

        private string EvaluateDictionary (Dictionary<string, string> dictionary, string key) {
            return (dictionary.ContainsKey(key)) ? dictionary[key] : string.Empty;
        }
    }
}
