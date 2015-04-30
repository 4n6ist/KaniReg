using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.Software {

    class Winlogon : ClassBase {

        public Winlogon(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Microsoft\Windows NT\CurrentVersion\Winlogon";
            Description = "ログオンユーザー情報";
        }

        public override bool Process() {
          
            RegistryValue[] values = Key.GetListOfValues();
            if (null != values && 0 < values.Length) {
                Dictionary<int, List<string>> dictionary = new Dictionary<int,List<string>>();
                List<string> list = new List<string>();
                StringBuilder builder;
                object data;
                int length = 0;
                string convertedData = string.Empty;

                foreach (RegistryValue value in values) {

                    if (!string.Empty.Equals(value.Name)) {
                        builder = new StringBuilder();
                        data = value.GetDataAsObject();

//const斬ってたはずだが
                        if (Constants.REG_BINARY == value.Type) {
                            length = ((byte[])data).Length;
                            foreach (byte item in (byte[])data) {
                                if (0 < builder.Length) { builder.Append(" "); }
                                builder.Append(item.ToString("X2"));
                            }
                            convertedData = builder.ToString();
                        } else {
                            convertedData = data.ToString();
                        }

                        if (dictionary.ContainsKey(length)) {
                            list = dictionary[length];
                            list.Add(value.Name + " = " + convertedData);
                            dictionary[length] = list;
                        } else {
                            list.Clear();
                            list.Add(value.Name + " = " + convertedData);
                            dictionary.Add(length, list);
                        }
                    }
                }
                
                List<KeyValuePair<int, List<string>>> sorted = new List<KeyValuePair<int,List<string>>>(dictionary);
                sorted.Sort(
                    delegate (KeyValuePair<int, List<string>> first, KeyValuePair<int, List<string>> next) {
                        return first.Key.CompareTo(next.Key);
                    }
                );

                foreach (KeyValuePair<int, List<string>> pair in sorted) {
                    foreach (string item in pair.Value) {
                        Reporter.Write("  " + item);
                    }
                }    
                
                Reporter.Write("");
                Reporter.Write("Analysis Tips: UserInit と Shell のVALUEはユーザーがログオンしたときに実行されます。");
                
            }
            else {
                Reporter.Write(KeyPath + " にはVALUEがありませんでした。");
            }

            return true;
        }
    }
}
