using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace KaniReg {

    /// <summary>
    /// interface for plugin classes
    /// </summary>
    public abstract class ClassBase {

        #region Private Members
        private RegistryKey _rootKey;
        private short _timeZoneBias;
        private bool _outputUtc;
        private Reporter _reporter;
        private Logger _logger;
        #endregion

        #region Properties
        protected string KeyPath { set; get; }
        protected string Description { set; get; }
        public bool PrintKeyInBase { set; get; }
        protected RegistryKey RootKey { get { return _rootKey; } }
        protected Reporter Reporter { get { return _reporter; } }
        protected Logger Logger { get { return _logger; } }
        public RegistryKey Key { get; set; }
        public short TimeZoneBias { get { return _timeZoneBias; } }
        public bool OutputUtc { get { return _outputUtc; } }
        public string Current { set; get; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rootKey"></param>
        /// <param name="reporter"></param>
        /// <param name="logger"></param>
        public ClassBase(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger, string currentControlSet = "") {

            if (!string.IsNullOrEmpty(currentControlSet)) {
                Current = currentControlSet;
            }

            Initialize();

            _rootKey = rootKey;
            _timeZoneBias = timeZoneBias;
            _outputUtc = outputUtc;
            _reporter = reporter;
            _logger = logger;

            Reporter.Write(Library.GetClassName(this));
            Reporter.Write("キーの説明：" + Description);
            if (!string.IsNullOrEmpty(KeyPath)) {

                if (PrintKeyInBase) {
                    Reporter.Write("キーのパス：" + KeyPath);
                }
                Key = RootKey.GetSubkey(KeyPath);

                if (null != Key) {
                    if (PrintKeyInBase) {
                        Reporter.Write("キーの最終更新日時：" + Library.TransrateTimestamp(Key.Timestamp, TimeZoneBias, OutputUtc));
                        Reporter.Write("");
                    }
                } else {
                    Reporter.Write("");
                    if (!PrintKeyInBase) {
                        Reporter.Write("検索したキーのパス：" + KeyPath);
                    }
                    Reporter.Write("Keyが見つかりませんでした。");
                    Reporter.Write("");
                }
            }
        }

        protected abstract void Initialize();      

        /// <summary>
        /// abstract method of main transaction
        /// </summary>
        /// <returns>successful or not</returns>
        public abstract bool Process();

        protected bool SimpleProcess() {

            RegistryValue[] values = Key.GetListOfValues();
            if (null != values && 0 < values.Length) {
                foreach (RegistryValue value in values) {
                    Reporter.Write("  " + value.Name + "   " + value.GetDataAsObject().ToString());
                }
            } else {
                Library.WriteNoValue(KeyPath, Reporter);
            }

            return true;
        }

        protected bool ProcessWithSort() {

            List<Container> list = new List<Container>();
            RegistryValue[] values = Key.GetListOfValues();

            if (null != values && 0 < values.Length) {
                foreach (RegistryValue value in values) {
                    list.Add(new Container(value.Name, value.GetDataAsObject().ToString()));
                }

                list.Sort(
                    delegate(Container first, Container next) {
                        return first.Name.CompareTo(next.Name);
                    }
                );

                foreach (Container container in list) {
                    Reporter.Write(container.Name + " -> " + container.Data);
                }

            } else {
                Library.WriteNoValue(KeyPath, Reporter);
            }

            return true;
        }
    }
}
