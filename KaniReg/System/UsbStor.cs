using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.SysHive {

    class UsbStor : ClassBase {

        public UsbStor(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = "ControlSet00" + Current + @"\Enum\USBStor";
            Description = "USBストレージ使用履歴";
        }

        public override bool Process() {

            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            RegistryKey[] endkeys;
            RegistryValue value;

            if (null != subkeys && 0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {
                    Reporter.Write(subkey.Name + " [" + Library.TransrateTimestamp(subkey.Timestamp, TimeZoneBias, OutputUtc) + "]");

                    endkeys = subkey.GetListOfSubkeys();
                    if (null != endkeys && 0 < endkeys.Length) {
                        foreach (RegistryKey endkey in endkeys) {
                            Reporter.Write("    S/N: " + endkey.Name + " [" + Library.TransrateTimestamp(endkey.Timestamp, TimeZoneBias, OutputUtc) + "]");

                            value = endkey.GetValue("FriendlyName");
                            if (null != value) {
                                Reporter.Write("    FriendlyName  : " + value.GetDataAsObject().ToString());
                            } else {
                                Reporter.Write("UsbStor / FriendlyName は見つかりませんでした。");
                            }

                            value = endkey.GetValue("ParentIdPrefix");
                            if (null != value) {
                                Reporter.Write("    ParentIdPrefix  : " + value.GetDataAsObject().ToString());
                            } else {
//                                Reporter.Write("UsbStor / ParentIdPrefix は見つかりませんでした。");
                            }
                        }
                    }
                    Reporter.Write("");
                }
            } else {
                Reporter.Write(KeyPath + " にはサブキーがありませんでした。");
            }

            return true;
        }
    }
}
