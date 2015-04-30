using System;
using System.Collections.Generic;

using System.Text;

namespace KaniReg {

    /// <summary>
    /// 定数管理クラス
    /// </summary>
    class Constants {

        // NT系のファイルSignature
        public const string SIGNATURE_NT = "regf";

        // 95系のファイルSignature
        public const string SIGNATURE_95 = "CREG";

        // キーのSignature
        public const string VALID_KEY_SIGNATURE = "nk";

        // 最初の要素へのオフセット基本値
        public const uint OFFSET_BASE = 0x1000;

        // 値のタイプ
        public const int REG_NONE = 0;
        public const int REG_SZ = 1;
        public const int REG_EXPAND_SZ = 2;
        public const int REG_BINARY = 3;
        public const int REG_DWORD = 4;
        public const int REG_DWORD_BIG_ENDIAN = 5;
        public const int REG_LINK = 6;
        public const int REG_MULTI_SZ = 7;
        public const int REG_RESOURCE_LIST = 8;
        public const int REG_FULL_RESOURCE_DESCRIPTOR = 9;
        public const int REG_RESOURCE_REQUIREMENTS_LIST = 10;
        public const int REG_QWORD = 11;
    }
}
