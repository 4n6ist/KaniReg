using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.Software {

    class NetworkListProfiles : ClassBase {

        public NetworkListProfiles(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = false;
            KeyPath = @"Microsoft\Windows NT\CurrentVersion\NetworkList\Profiles";
            Description = "NetworkListのProfilesの各情報(Vista/7用)";
        }

        public override bool Process() {
       
            RegistryKey[] subkeys = this.Key.GetListOfSubkeys();
            RegistryValue[] values = null;
            string data = string.Empty;

            if (null != subkeys && 0 < subkeys.Length) {
                this.Reporter.Write("GUID毎の情報は以下の通り");
                foreach (RegistryKey subkey in subkeys) {
                    if (subkey.Name.StartsWith("{")) {
                        this.Reporter.Write("\t" + subkey.Name);
                        values = subkey.GetListOfValues();
                        foreach (RegistryValue value in values) {

                            if ("DateLastConnected".Equals(value.Name) || "DateCreated".Equals(value.Name)) {
                                data = this.DecodeDate((byte[])(value.GetDataAsObject()));
                            } else if ("ProfileName".Equals(value.Name) || "Description".Equals(value.Name)) {
                                data = value.GetDataAsObject().ToString();
                            } else {
                                continue;
                            }
                            this.Reporter.Write("\t\t" + value.Name + " => " + data);
                        }
                    }
                }
            }

            return true;
        }

        private string DecodeDate(byte[] dateData) {

            // int型のリストで各時間部品を収集
            List<int> list = new List<int>();

            // 抽出用バイト配列
            byte[] extracted;

            for (ushort index = 0; index < dateData.Length; index+=2) {
                // 4(3番目の要素):曜日は不要と思われるのでスキップ
                if (4 == index) { continue; }
                // 抽出
                extracted = Library.ExtractArrayElements(dateData, index, 2);
                // リストに追加
                list.Add((int)BitConverter.ToInt16(extracted, 0));
            }

            // タイムゾーン設定を引いてやる処理
            // タイムゾーンをウソついてたらダメという制限事項付
            DateTime dateTime = new DateTime(list[0], list[1], list[2], list[3], list[4], list[5]);
			string suffix = " +" + this.TimeZoneBias.ToString("##") + ":00";
			//if (this.OutputUtc) {
			//	dateTime = dateTime.AddHours((double)(-1 * this.TimeZoneBias));
			//	suffix = " (UTC)";
			//}
			suffix = " (Local)"; // そのままローカルとして出すよう変更
            return dateTime.ToString("yyyy/MM/dd HH:mm:ss") + suffix;
        }
    }
}
