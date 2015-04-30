using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.NtUser {

    class RunMru : ClassBase {

        public RunMru(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU";
            Description = "「ファイル名を指定して実行」の履歴";
        }

        public override bool Process() {

            MruValues.Parse(this.Key, this.Reporter, this.Logger, true);

            return true;
        }
    }
}
