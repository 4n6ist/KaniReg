using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.NtUser {

    class ControlPanel : ClassBase {

        public ControlPanel(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel";
            Description = "コントロールパネルの設定";
        }

        public override bool Process() {
/*
            Reporter.Write("");
            Reporter.Write("Analysis Tip: The RecentTask* entries appear to only be populated through the");
            Reporter.Write("choices in the Control Panel Home view (in Vista).  As each new choice is");
            Reporter.Write("selected, the most recent choice is added as RecentTask1, and each ");
            Reporter.Write("RecentTask* entry is incremented and pushed down in the stack.");
            Reporter.Write("");
*/
            return SimpleProcess();
        }
    }
}
