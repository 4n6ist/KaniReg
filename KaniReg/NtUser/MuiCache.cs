using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.NtUser {

    class MuiCache : ClassBase {

        public MuiCache(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\ShellNoRoam\MUICache";
            Description = "";
        }

        public override bool Process() {

            RegistryValue[] values;
            try {
                values = Key.GetListOfValues();
            } catch (Exception exception) {
                Logger.Write(LogLevel.ERROR, exception.Message);
                return false;
            }

            try {
                if (null != values && 0 < values.Length) {
                    foreach (RegistryValue value in values) {
                        if (value.Name.StartsWith("@") || "LangID".Equals(value.Name)) {
                            continue;
                        }
                        Reporter.Write("\t" + value.Name + " (" + value.GetDataAsObject().ToString() + ")");
                    }
                } else {
                    Library.WriteNoValue(KeyPath, Reporter);
                }
            } catch (Exception exception) {
                Logger.Write(LogLevel.ERROR, exception.Message);
                return false;
            }

            return true;
        }
    }
}
