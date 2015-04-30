using System;
using System.Collections.Generic;


namespace KaniReg.Software {

    class ImageFile : ClassBase {

        public ImageFile(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\Windows NT\CurrentVersion\Image File Execution Options";
            Description = "実行ファイルを実行する際に自動起動するデバッガの設定";
        }

        public override bool Process() {
            
            RegistryKey[] subkeys = Key.GetListOfSubkeys();
            if (null != subkeys && 0 < subkeys.Length) {
                List<CombinedContainer> list = new List<CombinedContainer>();
                string debugger = string.Empty;
                foreach (RegistryKey subkey in subkeys) {

                    if (!subkey.Name.StartsWith("Your Image File Name Here without a path", StringComparison.CurrentCultureIgnoreCase)) {
                        try {
                            debugger = subkey.GetValue("Debugger").GetDataAsString();
                        } catch {
                            // If the eval{} throws an error, it's b/c the Debugger value isn't
                            // found within the key, so we don't need to do anything w/ the error
                        }
                        if (!string.Empty.Equals(debugger)) {
                            Reporter.Write("\t" + subkey.Name + "  " +
                                Library.TransrateTimestamp(subkey.Timestamp, TimeZoneBias, OutputUtc) + " -> " + debugger);
                        }
                    }
                }
                
                if (0 < list.Count) {
                    foreach (CombinedContainer container in list) {
                    }
                } else {
                    Reporter.Write("デバッガは設定されていません。");
                }
            } else {
                Reporter.Write(KeyPath + " にはサブキーがありません。");
            }
            return true;
        }
    }
}
