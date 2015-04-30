using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.SysHive {

    class SafeBoot : ClassBase {

        public SafeBoot(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = "ControlSet00" + Current + @"\Control\SafeBoot";
            Description = "SafeBootキーの存在確認、このキーが存在していない場合はウィルス感染の危険性があります。";
        }

        public override bool Process() {
               
            this.Reporter.Write("SafeBoot Keyは問題なく存在しています。");

            return true;
        }
    }
}
