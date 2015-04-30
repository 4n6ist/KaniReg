using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.SysHive {

    class MountedDevices : ClassBase {

        public MountedDevices(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"MountedDevices";
            Description = "NTFSフォーマットの記憶デバイス情報";
        }

        public override bool Process() {

            RegistryValue[] values = Key.GetListOfValues();

            if (null != values && 0 < values.Length) {


                // 単純に12文字で取れるときの格納用
                StringBuilder builder = new StringBuilder();
                // Unicode対応用
                Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
                List<string> list;
                string tag = string.Empty;
                
                foreach (RegistryValue value in values) {
                    byte[] data = value.Data;
                    if (12 == data.Length) {
                        builder = builder.Remove(0, builder.Length);
                        foreach (byte item in data) {
                            if (0 < builder.Length) {builder.Append(" ");}
                            builder.Append(item.ToString("X"));
                        }

                        Reporter.Write(value.Name);
                        Reporter.Write("\tDrive Signature = " + builder.ToString());
                    } else if (12 < data.Length) {
                        if (0x5C == data[0] || 0x5F == data[0]) {
                            tag = Encoding.Unicode.GetString(data);
                        } else {
                            tag = Encoding.UTF8.GetString(data);
                        }
                        if (dictionary.ContainsKey(tag)) {
                            list = dictionary[tag];
                            list.Add(value.Name);
                            dictionary[tag] = list;
                        } else {
                            list = new List<string>();
                            list.Add(value.Name);
                            dictionary.Add(tag, list);
                        }
                    } else {
                        Reporter.Write("Data length = " + data.Length.ToString());
                    }
                }
                
                Reporter.Write("");
                foreach (KeyValuePair<string, List<string>> pair in dictionary) {
                    Reporter.Write("Device: " + pair.Key);
                    foreach (string name in pair.Value) {
                        Reporter.Write("\t" + name);
                    }
                    Reporter.Write("");
                }
            } else {
                Library.WriteNoValue(KeyPath, Reporter);
            }

            return true;
        }
    }
}
