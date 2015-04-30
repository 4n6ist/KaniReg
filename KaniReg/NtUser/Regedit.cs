using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.NtUser {

    class Regedit : ClassBase {

        public Regedit(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit";
            Description = "Regeditの最終閲覧キー";
        }

        public override bool Process() {

            // Get Last Registry key opened in RegEdit
            RegistryValue value = this.Key.GetValue("LastKey");
            if (null != value) {
                this.Reporter.Write("RegEdit で最後に閲覧した値 -> " + value.GetDataAsObject().ToString());
            }

            return true;
        }
    }
}
