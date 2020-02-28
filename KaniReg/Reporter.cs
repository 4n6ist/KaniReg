using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace KaniReg
{

	public class Reporter
	{

		private MemoryStream _stream;

		public Reporter()
		{
			_stream = new MemoryStream();
		}

		/// <summary>
		/// 標準のUnicodeLEでログを記入
		/// </summary>
		/// <param name="message">メッセージ</param>
		public void Write(string message)
		{
			this.Write(message, Encoding.UTF8);
		}

		/// <summary>
		/// 文字コード指定でログ記入
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="inputEncoding">エンコーディング</param>
		public void Write(string message, Encoding inputEncoding)
		{
			// inputEncodingで文字列をbyte化
			// byte[] source = inputEncoding.GetBytes(message + "\r\n");
			// 余計な文字が混入してくるので出口で消すなら下記
			byte[] source = inputEncoding.GetBytes(message.Replace("\0", "").Replace("\x1A", "") + "\r\n");
			byte[] replaced = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(source));

			// 書き込み
			_stream.Write(replaced, 0, replaced.Length);
		}

		public void Close(string fileName)
		{
			FileStream fileStream;
			try
			{
				fileStream = new FileStream(fileName, FileMode.Create);
			}
			catch
			{
				throw new IOException("アクセス不可なファイルが指定されました。");
			}

			_stream.WriteTo(fileStream);
			fileStream.Flush();
			fileStream.Close();
			fileStream.Dispose();
			_stream.Close();
			_stream.Dispose();
		}

		public void Close()
		{
			_stream.Close();
			_stream.Dispose();
		}
	}
}
