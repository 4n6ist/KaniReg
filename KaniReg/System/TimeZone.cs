using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.SysHive {

    class TimeZone : ClassBase {

        public TimeZone(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = "ControlSet00" + Current + @"\Control\TimeZoneInformation";
            Description = "タイムゾーン情報";
        }

        public override bool Process() {

            RegistryValue[] values = Key.GetListOfValues();

            if (null != values && 0 < values.Length) {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();

                foreach (RegistryValue value in values) {
                    dictionary.Add(value.Name, value.GetDataAsObject());
                }
                   
                Reporter.Write("  DaylightName   -> " + dictionary["DaylightName"].ToString());
                Reporter.Write("  StandardName   -> " + dictionary["StandardName"].ToString());

// ひっくり返してみるときちんと時差になる
                string bias = Convert.ToString((uint.MaxValue - (uint)dictionary["Bias"] + 1) / 60 * -1);
                string activeTimeBias = Convert.ToString((uint.MaxValue - (uint)dictionary["ActiveTimeBias"] + 1) / 60 * -1);
                    
                Reporter.Write("  Bias           -> " + dictionary["ActiveTimeBias"].ToString() + " (" + bias + " hours)");
                Reporter.Write("  ActiveTimeBias -> " + dictionary["Bias"].ToString() + " (" + activeTimeBias + " hours)");
                   
            } else {
                Library.WriteNoValue(KeyPath, Reporter);
            }

            return true;
        }
    }
}
