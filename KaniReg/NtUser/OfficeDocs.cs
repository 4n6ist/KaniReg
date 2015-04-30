using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace KaniReg.NtUser {

    class OfficeDocs : ClassBase {

        private const int CURRENT_VERSION = 14;

        public OfficeDocs(RegistryKey rootKey, short timeZoneBias, bool outputUtc, Reporter reporter, Logger logger) : base(rootKey, timeZoneBias, outputUtc, reporter, logger) { }

        protected override void Initialize() {
//            KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Map Network Drive MRU";
            Description = "MS Officeのファイル一覧";
        }

        public override bool Process() {

            // First, let's find out which version of Office is installed
            List<int> versionList = new List<int>();
            string versionTag = string.Empty;
            string keyPath;
            for (int ver = 7; ver <= CURRENT_VERSION; ver++) {
                versionTag = ver.ToString("0.0");
                keyPath = @"Software\Microsoft\Office\" + versionTag + @"\Common\Open Find";
                RegistryKey key = RootKey.GetSubkey(keyPath);
                if (null != key) {
                    versionList.Add(ver);
                }
            }

            if (0 < versionList.Count) {
                foreach (int version in versionList) {
                    Reporter.Write("MSOffice version " + version.ToString() + " located.");
                    keyPath = "Software\\Microsoft\\Office\\" + versionTag;
                    RegistryKey officeKey = RootKey.GetSubkey(keyPath);

                    if (null != officeKey) {

                        // Attempt to retrieve Word docs            
                        string[] paths = new string[] {
//                        "Common\\Open Find\\Microsoft Office Word\\Settings\\Save As\\File Name MRU",
//                        "Common\\Open Find\\Microsoft Office Word\\Settings\\File Save\\File Name MRU",
                            @"Common\Open Find\Microsoft Office Word\Settings\名前を付けて保存\File Name MRU",
                            @"Common\Open Find\Microsoft Office Excel\Settings\名前を付けて保存\File Name MRU",
                            @"Common\Open Find\Microsoft Office PowerPoint\Settings\名前を付けて保存\File Name MRU",
                            @"Common\Open Find\Microsoft Office Outlook\Settings\ファイル名を付けて保存\File Name MRU"
                        };

                        RegistryKey branchKey;
                        string[] branchData;
                        bool succeed = false;
                        foreach (string path in paths) {
                            branchKey = officeKey.GetSubkey(path);
                            if (null != branchKey) {
                                Reporter.Write(path);
                                Reporter.Write("LastWrite Time " + Library.TransrateTimestamp(branchKey.Timestamp, TimeZoneBias, OutputUtc));
                                branchData = (string[])branchKey.GetValue("Value").GetDataAsObject();
                                foreach (string datum in branchData) {
                                    Reporter.Write("\t" + datum);
                                }
                                Reporter.Write("");
                                succeed = true;
                            }
                        }

                        if (!succeed) {
                            paths = new string[] {
                                @"Word\File MRU",
                                @"Excel\File MRU",
                                @"PowerPoint\File MRU",
                                @"Outlook\Catalog",
                                @"Outlook\Search\Catalog"
                            };

                            foreach (string path in paths) {
                                branchKey = officeKey.GetSubkey(path);
                                if (null != branchKey) {
                                    RegistryValue[] values = branchKey.GetListOfValues();
                                    if (0 < values.Length) {
                                        Reporter.Write("[" + path + "]");
                                        foreach (RegistryValue value in values) {
                                            Reporter.Write("\t" + value.Name + " : " + value.GetDataAsString());
                                        }
                                    }
                                    succeed = true;
                                    Reporter.Write("");
                                }
                            }
                        }

                        if (!succeed) {
                            Reporter.Write(keyPath + "配下においてはOffice保存先にあたるキーは見つかりませんでした。");
                        }
                    } else {
                        Reporter.Write(keyPath + "にはアクセスできませんでした。");
                    }
                }
            } else {
                Reporter.Write("MSOfficeのバージョンのキーは見つかりませんでした。");
            }

            return true;
        }
    }
}
