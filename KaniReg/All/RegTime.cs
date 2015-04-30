using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.All {

    class RegTime : ClassBase {

        public RegTime(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            Description = "Hive内のタイムスタンプ一覧";
        }

        public override bool Process() {

//            bool result = false;

            List<TimestampContainer2> registrySetList = new List<TimestampContainer2>();
            Traverse(ref registrySetList, RootKey);

            // Queueに指定RootKeyを格納
            Queue queue = new Queue();
            queue.Enqueue(RootKey);
            RegistryKey dequeued;
            RegistryKey[] keys;
            while (queue.Count > 0) {

                // RegistryKey格納QueueからRegistryKeyを取得
                dequeued = (RegistryKey)queue.Dequeue();

                // ディレクトリ配下のRegistryKeyを取得
                try {
                    keys = dequeued.GetListOfSubkeys();
                } catch (Exception ex) {
                    Logger.Write(LogLevel.ERROR, "レジストリキー " + dequeued.Name + " をパース中にエラーが発生しました : " + ex.Message);
                    continue;
                }

                // RegistryKey分ループ処理
                foreach (RegistryKey key in keys) {
                    // dictionary処理
                    Traverse(ref registrySetList, key);
                    queue.Enqueue(key);
                }
            }

            List<TimestampContainer2> list = new List<TimestampContainer2>(registrySetList);
            list.Sort(
                delegate(TimestampContainer2 first, TimestampContainer2 next) {
                    return first.Timestamp.CompareTo(next.Timestamp);
                }
            );

            foreach (TimestampContainer2 registrySet in list) {
                Reporter.Write(Library.TransrateTimestamp(registrySet.Timestamp, TimeZoneBias, OutputUtc) + "\t" + registrySet.Name);
            }

            return true;
        }

        private void Traverse(ref List<TimestampContainer2> registrySetList, RegistryKey key) {
            string name = key.AsString();
            name = name.Replace("$$$PROTO.HIV", string.Empty);
            if (0 < name.Length) {
                name = (name.Split('['))[0];
                registrySetList.Add(new TimestampContainer2(name, key.Timestamp));
            }
        }
    }
}
