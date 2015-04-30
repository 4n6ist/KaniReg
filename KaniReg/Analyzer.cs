using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace KaniReg
{
	public class Analyzer
	{
		private const string NTUSER = "NTUSER";
		private const string SYSTEM = "SYSTEM";
		private const string SOFTWARE = "SOFTWARE";
		private const string SECURITY = "SECURITY";
		private const string SAM = "SAM";
		private const string ALL = "ALL";
		private const string USRCLASS = "USRCLASS";

		BackgroundWorker _backgroundWorker;
		string _hiveFileName;
		string _configName;
		Reporter _reporter;
		Logger _logger;
		short _timeZoneBias;
		bool _outputUtc;

		public string ErrorMessage
		{
			set;
			get;
		}

		public Analyzer(Arguments arguments, Reporter reporter, Logger logger, BackgroundWorker backgroundWorker)
		{
			_reporter = reporter;
			_logger = logger;
			_hiveFileName = arguments.HiveFileName;
			_configName = arguments.HiveName;
			_timeZoneBias = arguments.TimeZoneBias;
			_outputUtc = arguments.OutputUtc;
			_backgroundWorker = backgroundWorker;
		}

		public bool Process(int processCount)
		{
			// Registryとそこからのルートキーを生成・取得
			Registry registry = new Registry(_hiveFileName);
			RegistryKey rootKey = registry.GetRootKey();
			rootKey.Logger = _logger;
			int errorCount = 0;

			_reporter.Write("-----<<Start:" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ">>---------------------------------");
			_reporter.Write("------------------------------------------------------------");

			string current = string.Empty;
			if (SYSTEM.Equals(_configName, StringComparison.OrdinalIgnoreCase))
			{
				current = rootKey.GetSubkey("Select").GetValue("Current").GetDataAsObject().ToString();
			}

			Type[] types = GetTypes(_configName);

			// 単一ファイル対象の処理を強引に再帰に持っていくための苦し紛れ
			if (types.Length == 0)
			{
				this.ErrorMessage = string.Format("【パース対象外ファイルをスキップ】： {0}", _hiveFileName);
				return false;
			}

			int count = 0;
			foreach (Type type in types)
			{
				if (_backgroundWorker.CancellationPending)
				{
					return false;
				}

				try
				{
					if (!CallClass(rootKey, type, current))
					{
						errorCount++;
					}
				}
				catch (Exception exception)
				{
					// もちろん失敗とする
					errorCount++;
					this.ErrorMessage = exception.Message;
				}

				// Write Border
				_reporter.Write("------------------------------------------------------------");
				count++;

				int progress = (int)(((double)count / (double)types.Length) * 100);
				progress = (100 > progress) ? progress : 100;
				_backgroundWorker.ReportProgress(progress, "Plugins");
			}

			// (一応)正常系を通知
			_reporter.Write("-----<<End:" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ">>----------------------------");

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="plugin"></param>
		private bool CallClass(RegistryKey rootKey, Type type, string current)
		{
			ClassBase instance;
			if (string.IsNullOrEmpty(current))
			{
				instance = (ClassBase)Activator.CreateInstance(type, rootKey, _timeZoneBias, _outputUtc, _reporter, _logger);
			}
			else
			{
				instance = (ClassBase)Activator.CreateInstance(type, rootKey, _timeZoneBias, _outputUtc, _reporter, _logger, current);
			}

			bool result = true;
			if (null != instance.Key || ALL.Equals(_configName))
			{
				result = instance.Process();
			}
			return result;
		}

		private Type[] GetTypes(string hiveName)
		{
			string nameSpace = "";

			if (NTUSER.Equals(hiveName))
			{
				nameSpace = "KaniReg.NtUser";
			}
			else if (SYSTEM.Equals(hiveName))
			{
				nameSpace = "KaniReg.SysHive";
			}
			else if (SOFTWARE.Equals(hiveName))
			{
				nameSpace = "KaniReg.Software";
			}
			else if (SECURITY.Equals(hiveName))
			{
				nameSpace = "KaniReg.Security";
			}
			else if (SAM.Equals(hiveName))
			{
				nameSpace = "KaniReg.Sam";
			}
			else if (ALL.Equals(hiveName))
			{
				nameSpace = "KaniReg.All";
			}
			else if (USRCLASS.Equals(hiveName))
			{
				nameSpace = "KaniReg.UsrClass";
			}
			else
			{
				return new Type[0];
			}

			Assembly assembly = Assembly.GetExecutingAssembly();
			Type[] types = assembly.GetTypes();
			List<Type> typeList = new List<Type>();

			foreach (Type type in types)
			{
				if (type.FullName.StartsWith(nameSpace))
				{
					typeList.Add(type);
				}
			}

			typeList.Sort(
				delegate(Type precedure, Type next)
				{
					return precedure.FullName.CompareTo(next.FullName);
				}
			);

			return typeList.ToArray();
		}
	}
}