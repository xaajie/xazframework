//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace XazEditor
{
    static public class XazEditorHelper
	{
		static public string temporaryCachePath
		{
			get
			{
				return Application.dataPath + "/../XazTemp";
			}
		}

        static public string GetAssetRelativePath(string path)
        {
            path = Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
            if (path.IndexOf("XazAssets") >= 0 || path.IndexOf("ResourcesAB") >= 0 || path.IndexOf("Resources") >= 0)
            {
                var a = path.Split('/');
                for (int i = a.Length - 2; i >= 0; i--)
                {
                    if (a[i] == "XazAssets" || a[i] == "Resources" || a[i] == "ResourcesAB")
                    {
                        return string.Join("/", a, i + 1, a.Length - i - 1);
                    }
                }
            }
            return path;
        }

		static public void CreateOrReplacePrefab(GameObject go, string targetPath, ReplacePrefabOptions options = ReplacePrefabOptions.ConnectToPrefab)
		{
			GameObject prefab = AssetDatabase.LoadAssetAtPath(targetPath, typeof(GameObject)) as GameObject;
			if (prefab != null) {
				PrefabUtility.ReplacePrefab(go, prefab, options);
			} else {
				Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
				PrefabUtility.CreatePrefab(targetPath, go, options);
			}
		}

		public static T[] GetAssetsAtPath<T>(string path, bool recursive) where T : UnityEngine.Object
		{
			List<T> list = new List<T>();
			string[] files = Directory.GetFiles(path, "*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			foreach (string file in files) {
				T t = AssetDatabase.LoadAssetAtPath<T>(file);
				if (t != null) {
					list.Add(t);
				}
			}
			return list.ToArray();
		}

		public static void Process(string cmd, string arguments)
		{
			// compress
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
			startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
			startInfo.FileName = cmd;
			startInfo.Arguments = arguments;
			process.StartInfo = startInfo;
			process.Start();
		}
	}
}
