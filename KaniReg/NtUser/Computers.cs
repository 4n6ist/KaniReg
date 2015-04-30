using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.NtUser {

    /// <summary>
    /// コンピューター一覧を取得
    /// </summary>
    class Computers : ClassBase {

        public Computers(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComputerDescriptions";
            Description = "現在ネットワーク上で認識可能なコンピューターの一覧";
        }

        public override bool Process() {
            return SimpleProcess();
        }
    }
}
