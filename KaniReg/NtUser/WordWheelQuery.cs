using System;

namespace KaniReg.NtUser {
    class WordWheelQuery : ClassBase {

        public WordWheelQuery(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\WordWheelQuery";
            Description = "Explorer検索履歴(Windows 7)";
        }

        public override bool Process() {

            MruValues.Parse(Key, Reporter, Logger);

            return true;
        }
    }
}
