using System;
using System.Collections.Generic;


namespace KaniReg.Software {

    class Mrt : ClassBase {

        public Mrt(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\RemovalTools\MRT";
            Description = "悪意のあるソフトウェア削除ツール実行履歴";
        }

        public override bool Process() {

            this.Reporter.Write("Key Path: " + KeyPath);
            this.Reporter.Write("LastWrite Time " + Library.TransrateTimestamp(this.Key.Timestamp, this.TimeZoneBias, this.OutputUtc));
            this.Reporter.Write("");
            
            // version取得試みる
            RegistryValue value = this.Key.GetValue("Version");
            if (null != value) {
                // 上手くいったら書き出す
                this.Reporter.Write("Version: " + this.Key.GetValue("Version").GetDataAsObject().ToString());
            } else {
                // でなければ設定なしを出す
                this.Reporter.Write("Version: 設定なし");
            }
            this.Reporter.Write("");
            this.Reporter.Write("Analysis Tip:  Go to http://support.microsoft.com/kb/891716/ to see when MRT");
            this.Reporter.Write("was last run.  According to the KB article, each time MRT is run, a new GUID");
            this.Reporter.Write("is written to the Version value.");

            return true;
        }
    }
}
