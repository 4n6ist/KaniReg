using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.NtUser {

    class LogonUserName : ClassBase {

        public LogonUserName(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer";
            Description = "ログオンユーザー名";
        }

        public override bool Process() {
            RegistryValue value = Key.GetValue("Logon User Name");
            if (null != value) {
                Reporter.Write("Logon User Name = " + value.GetDataAsObject().ToString());
            } else {
                Library.WriteNoValue(KeyPath, Reporter);
            }

            return true;
        }
    }
}
