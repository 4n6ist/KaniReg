using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg.NtUser {
    class TypedPaths : ClassBase {

        public TypedPaths(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
            PrintKeyInBase = true;
            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths";
            Description = "Explorer上でタイプしたパス";
        }

        public override bool Process() {

            RegistryValue[] values = this.Key.GetListOfValues();

            if (null != values && 0 < values.Length) {

                foreach (RegistryValue value in values) {
                    this.Reporter.Write(value.Name.PadRight(6) + ": " + value.GetDataAsString());
                }
            } else {
                Library.WriteNoValue(KeyPath, Reporter);
            }

            return true;
        }
    }
}
