using System;
using System.Collections.Generic;


namespace KaniReg.Botsu {

    class Skype : ClassBase {

        public Skype(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            KeyPath = @"Software\Skype\PluginManager";
            Description = "Skypeの設定情報";
        }

        public override bool Process() {

            // ソート用リスト
            List<UshortContainer> list = new List<UshortContainer>();

            // Valueを取得
            RegistryValue[] values = Key.GetListOfValues();

            if (null != values && 0 < values.Length) {
                foreach (RegistryValue value in values) {
                    if ("Language".Equals(value.Name)) {
                        list.Add(new UshortContainer(1, "言語 => " + value.GetDataAsObject().ToString()));
                    } else if ("Locale".Equals(value.Name)) {
                        list.Add(new UshortContainer(2, "ロケーション => " + value.GetDataAsObject().ToString()));
                    } else if ("RecentUsr".Equals(value.Name)) {
                        list.Add(new UshortContainer(3, "ユーザーアカウント => " + value.GetDataAsObject().ToString()));
                    } else if ("Version".Equals(value.Name)) {
                        list.Add(new UshortContainer(0, "バージョン => " + value.GetDataAsObject().ToString()));
                    }
                }

                // ソートする
                list.Sort(
                    delegate(UshortContainer first, UshortContainer next) {
                        return first.Order.CompareTo(next.Order);
                    }
                );

                // 印字
                foreach (UshortContainer container in list) {
                    Reporter.Write(container.Data);
                }
            }

            return true;
        }
    }
}
