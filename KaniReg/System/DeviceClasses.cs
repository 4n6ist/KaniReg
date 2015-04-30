using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.SysHive {

    class DeviceClasses : ClassBase {

        public DeviceClasses(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = "ControlSet00" + Current + @"\Control\DeviceClasses";
            Description = "USBストレージ一覧";
        }

        public override bool Process() {

            RegistryKey key = null;
            RegistryKey[] subkeys = null;
            string[] splitted;
            List<TimestampContainer> list = new List<TimestampContainer>();

            string precedent;
            string timestamp;

            // Get devices from the Disk GUID
            key = Key.GetSubkey("{53f56307-b6bf-11d0-94f2-00a0c91efb8b}");

            if (null != key) {
                Reporter.Write("");
                Reporter.Write("[DeviceClasses - Disks]");
                Reporter.Write("キーのパス：" + key.KeyPath);
                Reporter.Write("キーの最終更新日時：" + Library.TransrateTimestamp(key.Timestamp, TimeZoneBias, OutputUtc));
                Reporter.Write("");

                subkeys = key.GetListOfSubkeys();
                if (null != subkeys && 0 < subkeys.Length) {
                    foreach (RegistryKey subkey in subkeys) {
                        if (subkey.Name.Contains("USBSTOR")) {
                            splitted = subkey.Name.Split('#');
                            list.Add(new TimestampContainer(subkey.Timestamp, splitted[4] + "," + splitted[5]));
                        }
                    }

                    list.Sort(
                        delegate (TimestampContainer first, TimestampContainer next) {
                            return next.Timestamp.CompareTo(first.Timestamp);
                        }
                    );

                    precedent = string.Empty;
                    foreach (TimestampContainer container in list) {
                        timestamp = Library.TransrateTimestamp(container.Timestamp, TimeZoneBias, OutputUtc);
                        if (!precedent.Equals(timestamp)) {
                            Reporter.Write(timestamp);
                            precedent = timestamp;
                        }
                        Reporter.Write("\t" + container.Data);
                    }
                }
                else {
                    Reporter.Write("Diskにはサブキーがありませんでした。");
                }
            } else {
                Reporter.Write("Diskのキーは見つかりませんでした。");
            }
            Reporter.Write("");

            // Get devices from the Volume GUID
            key = Key.GetSubkey("{53f56307-b6bf-11d0-94f2-00a0c91efb8b}");

            if (null != key) {
                Reporter.Write("[DeviceClasses - Volumes]");
                Reporter.Write("キーのパス：" + key.KeyPath);
                Reporter.Write("キーの最終更新日時：" + Library.TransrateTimestamp(key.Timestamp, TimeZoneBias, OutputUtc));
                Reporter.Write("");

                subkeys = key.GetListOfSubkeys();

                if (null != subkeys && 0 < subkeys.Length) {

                    list.Clear();

                    foreach (RegistryKey subkey in subkeys) {
                        if (subkey.Name.Contains("RemovableMedia")) {
                            splitted = subkey.Name.Split('#');
                            list.Add(new TimestampContainer(subkey.Timestamp, splitted[5]));
                        }
                    }

                    list.Sort(
                        delegate (TimestampContainer first, TimestampContainer next) {
                            return next.Timestamp.CompareTo(first.Timestamp);
                        }
                    );

                    precedent = string.Empty;
                    foreach (TimestampContainer container in list) {
                        timestamp = Library.TransrateTimestamp(container.Timestamp, TimeZoneBias, OutputUtc);
                        if (!precedent.Equals(timestamp)) {
                            Reporter.Write(timestamp);
                            precedent = timestamp;
                        }
                        Reporter.Write("\tParentPrefixID: " + container.Data);
                    }
                } else {
                    Reporter.Write("Volumesにはサブキーがありませんでした。");
                }
            }
            else {
                Reporter.Write("Volumesのキーは見つかりませんでした。");
            }

            return true;
        }
    }
}
