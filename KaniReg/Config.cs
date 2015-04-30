using System;
using System.Collections.Generic;
using System.Text;

namespace KaniReg {

	// This class is not used anymore

	//public class Config {
	//	public struct Plugin {
	//		// local values
	//		private string _name;
	//		private string _keyPath;
	//		private string _description;

	//		/// <summary>
	//		/// property of plugin name
	//		/// </summary>
	//		public string Name {
	//			set { _name = value; }
	//			get { return _name; }
	//		}

	//		/// <summary>
	//		/// キーのパスを取得するプロパティ
	//		/// </summary>
	//		public string KeyPath {
	//			set { _keyPath = value; }
	//			get { return _keyPath; }
	//		}


	//		/// <summary>
	//		/// 説明文を取得するプロパティ
	//		/// </summary>
	//		public string Description {
	//			set { _description = value; }
	//			get { return _description; }
	//		}
	//	}

	//	/// <summary>
	//	/// read config-XML
	//	/// </summary>
	//	/// <param name="configName">plugin aggrigated config name</param>
	//	/// <returns>listed plugin config properties</returns>
	//	public static Config.Plugin[] Read(string configName) {
	//		string fileName = System.Windows.Forms.Application.StartupPath + @"\configs\" + configName + ".xml";

	//		Config.Plugin[] plugins = null;

	//		if (System.IO.File.Exists(fileName)) {
	//			System.IO.FileStream stream = new System.IO.FileStream(fileName, System.IO.FileMode.Open);
	//			System.Xml.Serialization.XmlSerializer serializer =
	//				new System.Xml.Serialization.XmlSerializer(
	//					typeof(Config.Plugin[]), new System.Xml.Serialization.XmlRootAttribute("Plugins"));
	//			plugins = (Config.Plugin[])serializer.Deserialize(stream);
	//			stream.Close();
	//		} else {
	//			return null;
	//		}
	//		return plugins;
	//	}
	//}
}
