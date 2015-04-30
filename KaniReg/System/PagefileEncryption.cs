using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.SysHive {

    class PagefileEncryption : ClassBase {

        public PagefileEncryption(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet) : base(rootKey, timeZoneBias, outputUtc, reporter, logger, currentControlSet) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = "ControlSet00" + Current + @"\Control\FileSystem";
            Description = "Pagefile.sysの暗号化設定";
        }

        public override bool Process() {

            RegistryValue value = Key.GetValue("NtfsEncryptPagingFile");

            if (null != value) {
                uint setting = (uint)value.GetDataAsObject();
                if (1 == setting) {
                    this.Reporter.Write("NtfsEncryptPagingFile : 1");
                    this.Reporter.Write("RESULT : Pagefile.sysは暗号化されています。");
                } else {
                    this.Reporter.Write("NtfsEncryptPagingFile : 0");
                    this.Reporter.Write("RESULT : Pagefile.sysは暗号化されていません。");
                }
            } else {
                this.Reporter.Write("NtfsEncryptPagingFileというValueは見つかりませんでした。");
            }

            return true;
        }

    }
}
