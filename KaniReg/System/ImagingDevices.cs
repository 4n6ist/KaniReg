using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace KaniReg.SysHive {

    class ImagingDevices : ClassBase {

        public ImagingDevices(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"ControlSet00" + Current + @"\Control\Class\{6BDD1FC6-810F-11D0-BEC7-08002BE2092F}";
            Description = "書き込みデバイス";
        }

        public override bool Process() {
              
            RegistryKey[] subkeys = Key.GetListOfSubkeys();
                
            if (null != subkeys && 0 < subkeys.Length) {
                foreach (RegistryKey subkey in subkeys) {
                    if (Regex.IsMatch(subkey.Name, @"^[0-9]{4}$")) {
                        try {
                            Reporter.Write("  " + subkey.GetValue("FriendlyName").GetDataAsObject());
                        } catch (Exception ex) {
                            Logger.Write(LogLevel.ERROR, "書き込みデバイスの名称を取得できませんでした。" + ex.Message);
                        }
                    }
                }
            } else {
                Reporter.Write(KeyPath + "にはサブキーはありませんでした。");
            }

            return true;
        }
    }
}
