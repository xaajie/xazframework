//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Xaz
{

    static public class FileHelper
	{
		static private readonly UTF8Encoding UTF8 = new UTF8Encoding(false);

#if !UNITY_WEBPLAYER
		static public string GetMD5Hash(string path)
		{
			if (!File.Exists(path))
				return string.Empty;

			return GetMD5Hash(File.ReadAllBytes(path));
		}

		static public string GetMD5Hash(byte[] buffer)
		{
			if (buffer == null)
				return string.Empty;

			MD5 md5 = new MD5CryptoServiceProvider();
			return BitConverter.ToString(md5.ComputeHash(buffer)).Replace("-", "").ToLower();
		}

		static public byte[] ReadBytes(string path)
		{
			if (!File.Exists(path))
				return null;

			return File.ReadAllBytes(path);
		}

		static public string ReadString(string path)
		{
			if (!File.Exists(path))
				return null;

			return File.ReadAllText(path, UTF8);
		}

		static public void WriteBytes(string path, byte[] data)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllBytes(path, data);
		}

		static public void WriteString(string path, string content)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllText(path, content, UTF8);
		}

		static public void AppendText(string path, string content)
		{
			if (!File.Exists(path)) {
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			}
			File.AppendAllText(path, content, UTF8);
		}
#endif
	}
}
