using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.Software {
    class NetworkInterfaces : ClassBase {

        public NetworkInterfaces(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\WZCSVC\Parameters\Interfaces";
            Description = "ネットワークカード一覧(SSIDベース)";
        }

        public override bool Process() {

            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {

                RegistryValue[] values;
                byte[] data = null;
                string ssid = string.Empty;

                ulong filetime = ulong.MaxValue;

                foreach (RegistryKey subkey in subkeys) {
                    if (null != subkey.Name) {
                        values = subkey.GetListOfValues();
                        if (null != values && 0 < values.Length) {
                            foreach (RegistryValue value in values) {
                                if (value.Name.StartsWith("Static#")) {
                                    data = (byte[])value.GetDataAsObject();
                                    ssid = Encoding.Unicode.GetString(Library.ExtractArrayElements(data, 0x14, 0x20));

                                    //filetimeでトライ
                                    filetime = BitConverter.ToUInt64(Library.ExtractArrayElements(data, 0x2B8, 0x08), 0);
                                    Reporter.Write("  " + subkey.Name + " SSID : " + ssid +
                                         " [" + Library.TransrateTimestamp(Library.CalculateTimestamp(filetime), TimeZoneBias, OutputUtc) + "]");                                        
                                }
                            }
                        } else {
                            Library.WriteNoValue(subkey.KeyPath, Reporter);
                        }
                    }
                }
            } else {
                Reporter.Write(KeyPath + " にはサブキーがありませんでした。");
            }

            return true;
        }
    }
}
