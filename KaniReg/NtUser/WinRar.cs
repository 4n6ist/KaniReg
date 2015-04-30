using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.NtUser {

    class WinRar : ClassBase {

        public WinRar(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\WinRAR\ArcHistory";
            Description = "WinRarの実行履歴";
        }

        public override bool Process() {
            return ProcessWithSort();
        }
    }
}
