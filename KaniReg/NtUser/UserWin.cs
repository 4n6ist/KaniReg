using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.NtUser {

    class UserWin : ClassBase {

        public UserWin(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows NT\CurrentVersion\Windows";
            Description = "ログオン時プログラム起動設定情報";
        }

        public override bool Process() {

            // 取得用変数を定義
            RegistryValue value = null;
            string transrated = string.Empty;

            // load VALUEの処理
            value = this.Key.GetValue("load");
            if (null != value) {
                transrated = value.GetDataAsObject().ToString();
                this.Reporter.Write("load value = " + transrated);
//                this.Reporter.Write("*空になるはず; anything listed gets run when the user logs in.");
            } else {
                this.Reporter.Write("load VALUEは見つかりませんでした。");
            }
            // 後処理
            value = null;
            transrated = string.Empty;

            // run VALUEの処理
            value = this.Key.GetValue("run");
            if (null != value) {
                transrated = value.GetDataAsObject().ToString();
                this.Reporter.Write("run value = " + transrated);
//                this.Reporter.Write("(*空になるはず; anything listed gets run when the user logs in.)");
            } else {
                this.Reporter.Write("run VALUEは見つかりませんでした。");
            }

            return true;
        }
    }
}
