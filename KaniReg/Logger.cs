using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KaniReg {

    public class Logger {

        private const string DEFAULT_FORMAT = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// ファイル名の長さ
        /// </summary>
        private const int MAX_PATH_LENGTH = 255;

        /// <summary>
        /// ログレベルの長さ(最大長さ)
        /// enum LogLevelの最大の長さを指定する
        /// </summary>
        private const short LEVEL_LENGTH = 8;
      
        /// <summary>
        /// ログファイルのパス
        /// </summary>
		private string _logFile;

		/// <summary>
		/// Error log file path
		/// </summary>
		private string _errorLog;   

        /// <summary>
        /// 
        /// </summary>
        private string _format;

        /// <summary>
        /// 
        /// </summary>
        private Encoding _encoding;

        /// <summary>
        /// 
        /// </summary>
		//public Logger(string logFile) {

		//	if (MAX_PATH_LENGTH < logFile.Length) {
		//		throw new ArgumentException("File path is too long.");
		//	}

		//	_logFile = logFile;
		//	_format = DEFAULT_FORMAT;
		//	_encoding = Encoding.UTF8;
		//}

		/// <summary>
		/// すでにツギハギだらけで本格的な対応を組み込む意義はあまりないのでツギハギ追加
		/// エラーログ分割対応
		/// </summary>
		public Logger(string logFile)
		{
			if (MAX_PATH_LENGTH < logFile.Length)
			{
				throw new ArgumentException("File path is too long.");
			}

			_logFile = logFile;
			_format = DEFAULT_FORMAT;
			_encoding = Encoding.UTF8;

			this._errorLog =
				logFile.Replace(".log", "") + "_Error.log";
		}

        public Logger(string logFile, string format) {
            if (MAX_PATH_LENGTH < logFile.Length) {
                throw new ArgumentException("File path is too long.");
            }
            _logFile = logFile;
            _format = format;
            _encoding = Encoding.UTF8;
        }

        public Logger(string logFile, string format, Encoding encoding) {
            if (MAX_PATH_LENGTH < logFile.Length) {
                throw new ArgumentException("File path is too long.");
            }
            _logFile = logFile;
            _format = format;
            _encoding = encoding;
        }

        /// <summary>
        /// 生成されたオブジェクトからログを書き出す
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
		public void Write(LogLevel level, string message)
		{
			string datetime = DateTime.Now.ToString(_format);

			if (level == LogLevel.ERROR)
			{
				File.AppendAllText(_errorLog, datetime + LevelFormatter(level) + message + "\r\n", Encoding.UTF8);
			}
			else
			{
				File.AppendAllText(_logFile, datetime + LevelFormatter(level) + message + "\r\n", Encoding.UTF8);
			}
		}


        /// <summary>
        /// 自由に呼び出せる書き出し(デフォルトフォーマット)
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="message"></param>
		//public static void Write(LogLevel level, string logFile, string message) {
		//	if (MAX_PATH_LENGTH < logFile.Length) {
		//		throw new ArgumentException("File path is too long.");
		//	}
		//	string datetime = DateTime.Now.ToString(DEFAULT_FORMAT);
		//	File.AppendAllText(logFile, datetime + LevelFormatter(level) + message + "\r\n", Encoding.UTF8);
		//}

        /// <summary>
        /// 自由に呼び出せる書き出し(フォーマット指定)
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="message"></param>
		//public static void Write(LogLevel level, string logFile, string message, string format) {
		//	if (MAX_PATH_LENGTH < logFile.Length) {
		//		throw new ArgumentException("File path is too long.");
		//	}
		//	string datetime =  DateTime.Now.ToString(format);
		//	File.AppendAllText(logFile, datetime + LevelFormatter(level) + message + "\r\n", Encoding.UTF8);
		//}

        /// <summary>
        /// 自由に呼び出せる書き出し(エンコード指定)
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="message"></param>
		//public static void Write(LogLevel level, string logFile, string message, string format, Encoding encoding) {
		//	if (MAX_PATH_LENGTH < logFile.Length) {
		//		throw new ArgumentException("File path is too long.");
		//	}
		//	string datetime = DateTime.Now.ToString(format);
		//	File.AppendAllText(logFile, datetime + LevelFormatter(level) + message + "\r\n", encoding);
		//}

        /// <summary>
        /// ログレベルを同一形態・長さに調整する
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static string LevelFormatter(LogLevel level) {
            return (" [" + level.ToString() + "] ").PadRight(LEVEL_LENGTH + 4);
        }
    }
}
