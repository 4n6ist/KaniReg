using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.SysHive {

    class ComputerName : ClassBase {

        public ComputerName(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"ControlSet00" + Current + @"\Control\ComputerName\ComputerName";
            Description = "コンピューター名";
        }

        public override bool Process() {

            string name = Key.GetValue("ComputerName").GetDataAsString();
            Reporter.Write("ComputerName = " + name);

            return true;
        }
    }
}
