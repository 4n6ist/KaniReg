using System;
using System.Collections.Generic;


namespace KaniReg.Security {

    class AuditPolicies : ClassBase {

        public AuditPolicies(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            KeyPath = @"Policy\PolAdtEv";
            Description = "ローカルセキュリティポリシーの監査ポリシー設定値";
        }

        public override bool Process() {

            Dictionary<uint, string> dictionary = new Dictionary<uint, string>();
            dictionary.Add(0, "監査しない");
            dictionary.Add(1, "成功");
            dictionary.Add(2, "失敗");
            dictionary.Add(3, "成功/失敗");

            this.Reporter.Write("「ローカルセキュリティ設定」情報");
            this.Reporter.Write(KeyPath);
            this.Reporter.Write("LastWrite Time " + Library.TransrateTimestamp(this.Key.Timestamp, this.TimeZoneBias, this.OutputUtc));
            this.Reporter.Write("");
            
            byte[] data;
            try {
                data = (byte[])this.Key.GetValue("(Default)").GetDataAsObject();
            } catch (Exception ex) {
                this.Reporter.Write("Error occurred getting data from " + KeyPath);
                this.Reporter.Write(" - " + ex.Message);

                return false;
            }

            // Check to see if auditing is enabled
            byte enabled = Library.ExtractArrayElements(data, 0, 1)[0];
            if (1 == enabled) {
                //this.Reporter.Write("Auditing is enabled.");
                this.Reporter.Write("監査が以下の場合に行われるように設定されています。");
                // Get audit configuration settings
                List<uint> list = Library.ByteArrayToUIntList(data);
                this.Reporter.Write("\tシステム イベントの監査                 = " + dictionary[list[1]]);
                this.Reporter.Write("\tログオン イベントの監査                 = " + dictionary[list[2]]);
                this.Reporter.Write("\tオブジェクト アクセスの監査             = " + dictionary[list[3]]);
                this.Reporter.Write("\t特権使用の監査                          = " + dictionary[list[4]]);
                this.Reporter.Write("\tプロセス追跡の監査                      = " + dictionary[list[5]]);
                this.Reporter.Write("\tポリシーの変更の監査                    = " + dictionary[list[6]]);
                this.Reporter.Write("\tアカウント管理の監査                    = " + dictionary[list[7]]);
                this.Reporter.Write("\tディレクトリ サービスのアクセスの監査   = " + dictionary[list[8]]);
                this.Reporter.Write("\tアカウント ログオン イベントの監査      = " + dictionary[list[9]]);
                //this.Reporter.Write("\tAudit System Events        = " + dictionary[list[1]]);
                //this.Reporter.Write("\tAudit Logon Events         = " + dictionary[list[2]]);
                //this.Reporter.Write("\tAudit Object Access        = " + dictionary[list[3]]);
                //this.Reporter.Write("\tAudit Privilege Use        = " + dictionary[list[4]]);
                //this.Reporter.Write("\tAudit Process Tracking     = " + dictionary[list[5]]);
                //this.Reporter.Write("\tAudit Policy Change        = " + dictionary[list[6]]);
                //this.Reporter.Write("\tAudit Account Management   = " + dictionary[list[7]]);
                //this.Reporter.Write("\tAudit Dir Service Access   = " + dictionary[list[8]]);
                //this.Reporter.Write("\tAudit Account Logon Events = " + dictionary[list[9]]);
            } else {
                this.Reporter.Write("**監査は設定されていません。");
                //this.Reporter.Write("**Auditing is NOT enabled.");
            }

            return true;
        }
    }
}
