using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.SysHive {

    class Services : ClassBase {

        internal readonly uint[] TYPES = {
            0x001, // "Kernel driver",
            0x002, // "File system driver",
            0x010, // "Own_Process",
            0x020, // "Share_Process",
            0x100  // "Interactive");
        };

        internal readonly uint[] START_TYPES = {
            0x00, // "Boot Start",
            0x01, // "System Start",
            0x02, // "Auto Start",
            0x03, // "Manual",
            0x04, // "Disabled");
        };

        public Services(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = "ControlSet00" + Current + @"\Services";
            Description = "サービスとドライバー";
        }

        public override bool Process() {

            // Get all subkeys and sort based on LastWrite times
            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {

                List<ComplexContainer> list = new List<ComplexContainer>();
                Dictionary<string, object> innerDictionary= new Dictionary<string, object>();
                Dictionary<double, List<ComplexContainer>> dictionary = new Dictionary<double, List<ComplexContainer>>();
//                    uint start = uint.MaxValue;
                uint type = 0;

                string precedent;
                string timestamp;

                foreach (RegistryKey subkey in subkeys) {

                    innerDictionary.Clear();
//                        start = uint.MaxValue;

                    try {
                        type = (uint)subkey.GetValue("Type").GetDataAsObject();
                    } catch {
                        type = 0;
                    }

                    if (0x001 == type || 0x002 == type) {
                        continue;
                    }


                    innerDictionary.Add("name", subkey.Name);

                    try {
                        innerDictionary.Add("displayName", subkey.GetValue("DisplayName").GetDataAsObject());
                    } catch {
                        innerDictionary.Add("displayName", string.Empty);
                    }

                    try {
                        innerDictionary.Add("imagePath", subkey.GetValue("ImagePath").GetDataAsObject());
                    } catch {
                        innerDictionary.Add("imagePath", string.Empty);
                    }
/*
どうせstart使ってない
                    try {
                        start = subkey.GetValue("Start").GetData();
                        if (START_TYPES.Contains(start)) {
                            innerDictionary.Add("start", start);
                        }
                    } catch {
                        innerDictionary.Add("start", uint.MaxValue);
                    }
*/                        
                    try {
                        innerDictionary.Add("objectName", subkey.GetValue("ObjectName").GetDataAsObject());
                    } catch {
                        innerDictionary.Add("objectName", string.Empty);
                    }



                    if (dictionary.ContainsKey(subkey.Timestamp)) {
                        list = dictionary[subkey.Timestamp];
                        list.Add(new ComplexContainer(
                            innerDictionary["name"].ToString(), innerDictionary["displayName"].ToString(),
                            innerDictionary["imagePath"].ToString(), //(uint)dictionary["start"],
                            innerDictionary["objectName"].ToString()));
                        dictionary[subkey.Timestamp] = list;
                    } else {
                        list = new List<ComplexContainer>();
                        list.Add(new ComplexContainer(
                            innerDictionary["name"].ToString(), innerDictionary["displayName"].ToString(),
                            innerDictionary["imagePath"].ToString(), //(uint)dictionary["start"],
                            innerDictionary["objectName"].ToString()));
                        dictionary.Add(subkey.Timestamp, list);
                    }
                }

                List<KeyValuePair<double, List<ComplexContainer>>> sorted = new List<KeyValuePair<double, List<ComplexContainer>>>(dictionary);
                sorted.Sort(
                    delegate(KeyValuePair<double, List<ComplexContainer>> first, KeyValuePair<double, List<ComplexContainer>> next) {
                        return next.Key.CompareTo(first.Key);
                    }
                );

                precedent = string.Empty;
                StringBuilder builder;
                foreach (KeyValuePair<double, List<ComplexContainer>> pair in sorted) {

                    timestamp = Library.TransrateTimestamp(pair.Key, TimeZoneBias, OutputUtc);

                    if (!precedent.Equals(timestamp)) {
                        Reporter.Write("");
                        Reporter.Write(timestamp /* + "Z"*/);
                        precedent = timestamp;
                    }

                    foreach (ComplexContainer container in pair.Value) {
                        builder = new StringBuilder();
                        builder.Append("  " + container.Name);

                        if (string.Empty.Equals(container.ImagePath)) {
                            if (!string.Empty.Equals(container.DisplayName)) {
                                builder.Append(" (" + container.DisplayName + ")");
                            }
                        } else {
                            builder.Append(" (" + container.ImagePath + ")");
                        }

                        if (!string.Empty.Equals(container.ObjectName)) {
                            builder.Append(" [" + container.ObjectName + "]");
                        }

                        Reporter.Write(builder.ToString());
                    }
                }
                    
            } else {
                Reporter.Write(KeyPath + " にはサブキーがありませんでした。");
            }            

            return true;
        }
    }
}
