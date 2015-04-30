using System;
using System.Collections.Generic;


namespace KaniReg.NtUser {

    class MediaPlayer : ClassBase {

        public MediaPlayer(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\MediaPlayer\Player";
            Description = "Windows Media Playerの再生履歴";
        }

        public override bool Process() {

            // コンフィグにsubkey attribute追加してループ化したい…!!
            WriteSubkeyValues("RecentFileList");
            WriteSubkeyValues("RecentURLList");

            return true;
        }

        private void WriteSubkeyValues(string subkeyName) {

            RegistryKey subkey = Key.GetSubkey(subkeyName);

            if (null != subkey) {
                RegistryValue[] values = subkey.GetListOfValues();
                List<KeyValuePair<ushort, string>> list = new List<KeyValuePair<ushort, string>>();
                if (null != values && 0 < values.Length) {
                    foreach (RegistryValue value in values) {
                        if (value.Name.StartsWith("File")) {
                            list.Add(new KeyValuePair<ushort, string>(Convert.ToUInt16(value.Name.Substring(4)), value.GetDataAsObject().ToString()));
                        }
                    }

                    list.Sort(
                        delegate(KeyValuePair<ushort, string> first, KeyValuePair<ushort, string> next) {
                            return first.Key.CompareTo(next.Key);
                        }
                    );

                    Reporter.Write("サブキー：" + subkeyName);
                    foreach (KeyValuePair<ushort, string> pair in list) {
                        Reporter.Write("\tFile" + pair.Key.ToString() + " => " + pair.Value);
                    }

                } else {
                    Reporter.Write("サブキー：" + subkeyName + "にはValueがありませんでした。");
                }
            } else {
                Reporter.Write("サブキー：" + subkeyName + "は見つかりませんでした。");
            }

            Reporter.Write("");
        }
    }
}
