using System;
using System.Collections.Generic;


namespace KaniReg.SysHive {

    class Ide : ClassBase {

        public Ide(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"ControlSet00" + Current + @"\Enum\IDE";
            Description = "IDEストレージ一覧";
        }

        public override bool Process() {
      
            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {
                    Reporter.Write("");
                    Reporter.Write(subkey.Name + " [" + Library.TransrateTimestamp(subkey.Timestamp, TimeZoneBias, OutputUtc) + "]");
                        
                    RegistryKey[] endkeys = subkey.GetListOfSubkeys();
                    if (null != endkeys && 0 < endkeys.Length) {
                        foreach (RegistryKey endkey in endkeys) {
                            Reporter.Write(endkey.Name + " [" + Library.TransrateTimestamp(endkey.Timestamp, TimeZoneBias, OutputUtc) + "]");
                            try {
                                Reporter.Write("FriendlyName : " + endkey.GetValue("FriendlyName").GetDataAsObject().ToString());
                            } catch (Exception ex) {
                                // 原文noopだが一応ログ
                                Logger.Write(LogLevel.ERROR, "error occurred when getting FriendlyName value./" + ex.Message);
                            }
                            Reporter.Write("");
                        }
                    }

                    if ("Control".Equals(subkey.Name)) {
                        RegistryKey key = subkey.GetSubkey(@"DeviceClasses\{53f56307-b6bf-11d0-94f2-00a0c91efb8b}");

                        if (null != key) {
                            Reporter.Write("DeviceClasses - Disks");
                            Reporter.Write(key.Name);

                            List<TimestampContainer> list = new List<TimestampContainer>();
                            string[] splitted = null;
                            RegistryKey[] nestedkeys = key.GetListOfSubkeys();
                            if (null != subkeys && 0 < subkeys.Length) {
                                foreach (RegistryKey nestedKey in nestedkeys) {
                                    if (nestedKey.Name.Contains("IDE")) {
                                        splitted = subkey.Name.Split('#');
                                        list.Add(new TimestampContainer(nestedKey.Timestamp, splitted[4] + "," + splitted[5]));
                                    }
                                }

                                if (0 < list.Count) {
                                    Reporter.Write("");

                                    list.Sort(
                                        delegate(TimestampContainer first, TimestampContainer next) {
                                            return next.Timestamp.CompareTo(first.Timestamp);
                                        }
                                    );

                                    foreach (TimestampContainer container in list) {
                                        Reporter.Write(Library.TransrateTimestamp(container.Timestamp, TimeZoneBias, OutputUtc));
                                        Reporter.Write("\t" + container.Data);
                                    }
                                } else {
                                    Reporter.Write("No IDE subkeys were found.");
                                    return true;
                                }
                            } else {
                                Reporter.Write(key.KeyPath + "にはサブキーはありませんでした。");
                            }
                        } else {
                            Reporter.Write(key.KeyPath + "は見つかりませんでした。");
                        }
                    }
                        
                }

            } else {
                Reporter.Write("サブキーはありませんでした。");
            }
    
            return true;
        }
    }
}
