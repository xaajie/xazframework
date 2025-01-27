//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace XazEditor
{
    public class FileUtil
	{
		/// Get md5 checksum of the specified file.
		/// If the specified file is not exists, return null.
		/// 
		static public string GetMD5Hash(string filePath)
		{
			if (!File.Exists(filePath))
				return null;

			return GetMD5Hash(File.ReadAllBytes(filePath));
		}

		static public string GetMD5Hash(byte[] buffer)
		{
			if (buffer == null)
				return null;

			MD5 md5 = new MD5CryptoServiceProvider();
			return BitConverter.ToString(md5.ComputeHash(buffer)).Replace("-", "").ToLower();
		}

		static public void MakeDirs(string path, bool pathIsFile = true)
		{
			if (pathIsFile) {
				path = Path.GetDirectoryName(path);
			}
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
		}

		static public byte[] ReadBytes(string filePath)
		{
			if (!File.Exists(filePath))
				return null;

			return File.ReadAllBytes(filePath);
		}

		static public string ReadString(string filePath)
		{
			if (!File.Exists(filePath))
				return null;

			return File.ReadAllText(filePath, Encoding.UTF8);
		}

		static public void WriteBytes(string filePath, byte[] data)
		{
			MakeDirs(filePath);
			File.WriteAllBytes(filePath, data);
		}

		static public void WriteString(string filePath, string content)
		{
			MakeDirs(filePath);

			File.WriteAllText(filePath, content.Replace(Environment.NewLine, "\n"), Encoding.UTF8);

			//using (var sw = new StreamWriter(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite), Encoding.UTF8)) {
			//	sw.Write(content.Replace(Environment.NewLine, "\n"));
			//}
		}

		static public void CopyFile(string from, string to, bool overwrite)
		{
			MakeDirs(to);
			File.Copy(from, to, overwrite);
		}

        /// <summary>
        /// 获得代码有效语句，去掉注释后的
        /// </summary>
        /// <param name="path"></param>

        static public string GetValidLuaFile(string path, Dictionary<string, string> lualist)
        {
            string filex = "";
            if (!lualist.ContainsKey(path))
            {
                filex = ReadString(path);
                // 找到所有多行注释中的文本，并将其替换为空字符串 
                filex = Regex.Replace(filex, "--\\[\\[(.|\\n)*?\\]\\]", "");
                // 将所有单行注释（以`--`开头的行）删除
                filex = Regex.Replace(filex, "--.*(\r\n|\n|\r)", "");
                lualist.Add(path, filex);
            }
            else
            {
                lualist.TryGetValue(path, out filex);
            }
            return filex;
        }

    }
}
