//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Xaz;

namespace XazEditor
{
	static public class AssetBundleBuilder
	{
		public class Package
		{
			public string name;
			public string group;
			public bool combined;
			public HashSet<string> assets;

			public Package(string name) : this(name, "", "")
			{
			}

			public Package(string name, string asset) : this(name, Path.GetDirectoryName(name), asset)
			{
			}
			public Package(string name, string group, string asset)
			{
				this.name = Path.ChangeExtension(name, XazHelper.XazAssetExtension);
				this.combined = false;
				this.group = group == null ? string.Empty : group;
				this.assets = new HashSet<string>();
				if (!string.IsNullOrEmpty(asset)) {
					this.assets.Add(asset.Replace("\\", "/"));
				}
			}
			public Package(string name, IEnumerable<string> collection, bool combined = true) : this(name, Path.GetDirectoryName(name), collection, combined)
			{
			}
			public Package(string name, string group, IEnumerable<string> collection, bool combined = true)
			{
				this.name = Path.ChangeExtension(name, XazHelper.XazAssetExtension);
				this.combined = combined;
				this.group = group == null ? string.Empty : group;
				var hashSet = new HashSet<string>();
				using (var it = collection.GetEnumerator()) {
					while (it.MoveNext()) {
						hashSet.Add(it.Current.Replace("\\", "/"));
					}
				}
				this.assets = hashSet;
			}

			public string[] GetAssetsArray()
			{
				string[] arr = new string[assets.Count];
				assets.CopyTo(arr);
				return arr;
			}
		}

		static public void BuildAssetBundles(string version, string outPath, List<Package> packages)
		{
			BuildAssetBundles(version, outPath, packages, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
		}

		static public void BuildAssetBundles(string version, string outPath, List<Package> packages, BuildAssetBundleOptions options)
		{
			BuildAssetBundles(version, outPath, packages, options, EditorUserBuildSettings.activeBuildTarget);
		}

		static public void BuildAssetBundles(string version, string outPath, List<Package> packages, BuildAssetBundleOptions options, BuildTarget target)
		{
			if (EditorUserBuildSettings.activeBuildTarget != target) {
				Debug.LogErrorFormat("当前平台为[{0}]，请切换到目标平台[{1}]", EditorUserBuildSettings.activeBuildTarget, target);
				return;
			}

			// 生成依赖包
			PackageOptimizer optimizer = new PackageOptimizer(packages);
			packages = optimizer.packages;
			var packageData = optimizer.GeneratePackageData();
			var cacheFile = string.Format("{0}/ABBuilderCache_{1}_{2}", XazEditorTools.temporaryCachePath, target, version);
			if (File.Exists(cacheFile)) {
				var cacheData = JsonUtility.FromJson<PackageData>(FileUtil.ReadString(cacheFile));
				if (cacheData != null) {
					packages = packageData.GetModifiesPackages(packages, cacheData);
				}
			}

			if (packages != null && packages.Count > 0) {
				var builder = new StringBuilder();
				builder.AppendLine("Build...");
				foreach (var pkg in packages) {
					builder.AppendLine("Pkg: " + pkg.name);
					foreach (var asset in pkg.assets) {
						builder.AppendLine("\t" + asset);
					}
				}
				Debug.Log(builder.ToString());
				Debug.LogFormat("Total:{0}, Build:{1}", packageData.packageInfos.Count, packages.Count);
				//Debug.Log(JsonUtility.ToJson(packageData));
				// Build AssetBundles
				string oPath = XazEditorTools.temporaryCachePath + "/AssetBundles";
				if (Directory.Exists(oPath)) {
					Directory.Delete(oPath, true);
				}
				Directory.CreateDirectory(oPath);

				var manifestFilePath = "Assets/" + XazHelper.XazAssetIndexName + ".asset";
				if (File.Exists(manifestFilePath)) {
					File.Delete(manifestFilePath);
				}
				var manifest = packageData.GenerateAssetManifest();
				AssetDatabase.CreateAsset(manifest, manifestFilePath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				packages.Add(new Package(XazHelper.XazAssetIndexName, manifestFilePath));

				var assetBundleManifest = BuildPipeline.BuildAssetBundles(oPath, packages.ConvertAll((p) => {
					var list = new List<string>(p.assets);
					return new AssetBundleBuild() {
						assetBundleName = p.name,
						assetNames = list.Exists((s) => s.ToLower().EndsWith(".unity")) ? list.FindAll((s) => s.ToLower().EndsWith(".unity")).ToArray() : list.ToArray()
					};
				}).ToArray(), BuildAssetBundleOptions.DeterministicAssetBundle | options, target);

				foreach (var package in packages) {
					FileUtil.CopyFile(oPath + "/" + package.name.ToLower(), outPath + "/" + package.name, true);
				}

				// 删除过期的文件
				foreach (var path in Directory.GetFiles(outPath, "*.*", SearchOption.AllDirectories)) {
					var name = path.Replace("\\", "/").Substring(outPath.Length + 1);
					if (name == XazHelper.XazAssetIndexName)
						continue;

					if (!Array.Exists(manifest.bundles, (b) => b.name == name)) {
						Debug.Log("Deleting (" + path + ") ...");
						File.Delete(path);
					}
				}
			}

			// Save CacheData
			FileUtil.WriteString(cacheFile, JsonUtility.ToJson(packageData));
		}

		[Serializable]
		public class PackageData
		{
			[Serializable]
			public class AssetInfo
			{
				[SerializeField]
				public string name;
				[SerializeField]
				public string md5;
				[SerializeField]
				public bool exportable;
			}

			[Serializable]
			public class PackageInfo
			{
				[SerializeField]
				public string name;
				[SerializeField]
				public List<AssetInfo> assetInfos;
				[SerializeField]
				public List<string> depends;
				[SerializeField]
				public List<string> reverseDepends;
			}

			[SerializeField]
			public List<PackageInfo> packageInfos;

			private Dictionary<string, PackageInfo> ToDictionary()
			{
				Dictionary<string, PackageInfo> packageInfoMap = new Dictionary<string, PackageInfo>();
				foreach (var packageInfo in packageInfos) {
					packageInfoMap.Add(packageInfo.name, packageInfo);
				}
				return packageInfoMap;
			}

			public List<Package> GetModifiesPackages(List<Package> packages, PackageData cacheData)
			{
				var dataMap = this.ToDictionary();
				var cacheDataMap = cacheData.ToDictionary();

				HashSet<string> modifies = new HashSet<string>();
				foreach (var packageInfo in packageInfos) {
					if (modifies.Contains(packageInfo.name))
						continue;
					PackageInfo cacheInfo = null;
					bool modified = !cacheDataMap.TryGetValue(packageInfo.name, out cacheInfo);
					if (!modified) {
						if (cacheInfo.depends.Count != packageInfo.depends.Count) {
							modified = true;
						} else if (!packageInfo.depends.TrueForAll((s) => cacheInfo.depends.Contains(s))) {
							modified = true;
						} else if (cacheInfo.assetInfos.Count != packageInfo.assetInfos.Count) {
							modified = true;
						} else {
							Dictionary<string, string> cacheAssetMap = new Dictionary<string, string>();
							foreach (var assetInfo in cacheInfo.assetInfos) {
								cacheAssetMap[assetInfo.name] = assetInfo.md5;
							}
							foreach (var assetInfo in packageInfo.assetInfos) {
								string md5 = null;
								if (!cacheAssetMap.TryGetValue(assetInfo.name, out md5) || md5 != assetInfo.md5) {
									modified = true;
									break;
								}
							}
						}
					}
					if (modified) {
						Queue<PackageInfo> depends = new Queue<PackageInfo>();
						depends.Enqueue(packageInfo);
						while (depends.Count > 0) {
							var p = depends.Dequeue();
							modifies.Add(p.name);
							foreach (var depend in p.depends) {
								if (modifies.Contains(depend))
									continue;
								depends.Enqueue(dataMap[depend]);
							}
						}
					}
				}
				if (modifies.Count > 0) {
					return packages.Count == modifies.Count ? packages : packages.FindAll((p) => modifies.Contains(p.name));
				}
				return null;
			}

			public Xaz.AssetsManifest GenerateAssetManifest()
			{
				var bundles = new List<Xaz.AssetsManifest.Bundle>();
				var directories = new List<string>();
				var assets = new List<Xaz.AssetsManifest.Asset>();
				var depends = new List<int>();
				var scenes = new List<string>();

				var dict = new Dictionary<string, int>();
				for (int i = 0; i < packageInfos.Count; i++) {
					dict[packageInfos[i].name] = i;
				}

				var token = "/XazAssets/";
				var tokenLength = token.Length;
				for (int i = 0; i < packageInfos.Count; i++) {
					assets.Clear();
					scenes.Clear();
					depends.Clear();

					var packageInfo = packageInfos[i];
					var bundle = new Xaz.AssetsManifest.Bundle() {
						name = packageInfo.name,
						dependents = packageInfo.reverseDepends.Count
					};

					foreach (var depend in packageInfo.depends) {
						depends.Add(dict[depend]);
					}
					foreach (var assetInfo in packageInfo.assetInfos) {
						if (assetInfo.exportable) {
							var index = 0;
							var assetName = assetInfo.name;
							var p = assetName.LastIndexOf(token);
							if (p >= 0) {
								var dir = assetName.Substring(0, p + tokenLength);
								assetName = assetName.Substring(p + tokenLength);
								index = directories.IndexOf(dir) + 1;
								if (index <= 0) {
									directories.Add(dir);
									index = directories.Count;
								}
							}
							assets.Add(new AssetsManifest.Asset() { path = assetName, index = index });

							// Scenes
							if (assetInfo.name.ToLower().EndsWith(".unity")) {
								scenes.Add(Path.GetFileNameWithoutExtension(assetInfo.name));
							}
						}
					}

					bundle.assets = assets.ToArray();
					bundle.scenes = scenes.ToArray();
					bundle.depends = depends.ToArray();
					bundles.Add(bundle);
				}

				var manifest = ScriptableObject.CreateInstance<Xaz.AssetsManifest>();
				manifest.bundles = bundles.ToArray();
				manifest.directories = directories.ToArray();

				return manifest;
			}
		}

		private class PackageOptimizer
		{
			public abstract class Asset
			{
				public Asset(string name)
				{
					this.name = name;
				}
				public string name
				{
					get; private set;
				}
				//public virtual long size
				//{
				//	get;
				//}

				private HashSet<Package> m_Packages;
				public virtual HashSet<Package> packages
				{
					get
					{
						if (m_Packages == null) {
							m_Packages = new HashSet<Package>();
						}
						return m_Packages;
					}
				}
				public virtual Package package
				{
					get;
					set;
				}
			}

			public class SingleAsset : Asset
			{
				public SingleAsset(string name) : base(name) { }
			}

			public class CombineAsset : Asset
			{
				public CombineAsset(string name) : this(name, null)
				{
				}
				public CombineAsset(string name, HashSet<string> assets) : base(name)
				{
					this.assets = assets ?? new HashSet<string>();
				}
				public HashSet<string> assets
				{
					get; private set;
				}
			}
			public class ReferenceAsset : Asset
			{
				private Asset m_Asset;
				public ReferenceAsset(string name, Asset asset) : base(name)
				{
					m_Asset = asset;
				}
				public override HashSet<Package> packages
				{
					get
					{
						return m_Asset.packages;
					}
				}
				public override Package package
				{
					get
					{
						return m_Asset.package;
					}
				}
			}

			private Dictionary<string, string> m_ExportableAssets = new Dictionary<string, string>();
			private HashSet<string> m_NonTaggedTextures = new HashSet<string>();
			private Dictionary<string, Asset> m_AssetInfos = new Dictionary<string, Asset>();
			private Dictionary<string, HashSet<string>> m_CombinedAssets = new Dictionary<string, HashSet<string>>();
			// 资源的反向依赖列表
			private Dictionary<string, HashSet<string>> m_AssetReverseDependencies = new Dictionary<string, HashSet<string>>();

			public List<Package> packages
			{
				get; private set;
			}

			public PackageOptimizer(List<Package> packages)
			{
				foreach (var p in packages) {
					if (p.combined) {
						m_CombinedAssets.Add("[CP]" + p.name, new HashSet<string>(p.assets));
					}
					foreach (var asset in p.assets) {
						CollectDependencies(asset, p, true);
					}
				}

#if UNITY_5_4_OR_NEWER // || UNITY_5_5 || UNITY_5_6 || UNITY_2017
				FixedUnitySpriteTextureBug();
#endif
				GenerateCombinedAssets();
				//CheckCircularDependencies();
				GenerateRawPackages();
				// TODO: 预测包大小
				ConvertCombinedToAssets();
			}

			public PackageData GeneratePackageData()
			{
				Dictionary<string, List<string>> allReverseDepends = new Dictionary<string, List<string>>();

				var packageData = new PackageData() { packageInfos = new List<PackageData.PackageInfo>() };
				foreach (var package in packages) {
					List<string> reverseDepends = null;
					if (!allReverseDepends.TryGetValue(package.name, out reverseDepends)) {
						reverseDepends = new List<string>();
						allReverseDepends[package.name] = reverseDepends;
					}
					var packageInfo = new PackageData.PackageInfo() {
						name = package.name,
						assetInfos = new List<PackageData.AssetInfo>(),
						depends = new List<string>(),
						reverseDepends = reverseDepends
					};
					packageData.packageInfos.Add(packageInfo);
					var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
					foreach (var asset in package.assets) {
						var bytes = FileUtil.ReadBytes(asset);
						// test atlas处理 todo
                        if (bytes == null)
                        {
							Debug.Log(asset);
							continue;
						}
						if (File.Exists(asset + ".meta")) {
							ArrayUtility.AddRange(ref bytes, FileUtil.ReadBytes(asset + ".meta"));
						}
						packageInfo.assetInfos.Add(new PackageData.AssetInfo() {
							name = asset,
							exportable = m_ExportableAssets.ContainsKey(asset),
							md5 = BitConverter.ToString(md5.ComputeHash(bytes)).Replace("-", "").ToLower()
						});
						foreach (var path in GetDependencies(asset, false)) {
							if (package.assets.Contains(path))
								continue;
							Asset assetInfo = null;
							if (m_AssetInfos.TryGetValue(path, out assetInfo)) {
								var name = assetInfo.package.name;
								if (!packageInfo.depends.Contains(name)) {
									packageInfo.depends.Add(name);
									if (!allReverseDepends.TryGetValue(name, out reverseDepends)) {
										reverseDepends = new List<string>();
										allReverseDepends[name] = reverseDepends;
									}
									reverseDepends.Add(packageInfo.name);
								}
							}
						}
					}
				}
				return packageData;
			}

			// 修复当Texture类型为Sprite时，多种类型（Material、RawImage、...）和Texture不能分包的bug
			private void FixedUnitySpriteTextureBug()
			{
				Queue<string> queue = new Queue<string>();
				foreach (var tex in m_NonTaggedTextures) {
					HashSet<string> list = new HashSet<string>();
					// 循环查找依赖关系
					queue.Clear();
					queue.Enqueue(tex);
					while (queue.Count > 0) {
						var str = queue.Dequeue();
						if (str.ToLower().EndsWith(".unity"))  // 场景文件不可与Texture一起
							continue;
						list.Add(str);
						HashSet<string> reverseDepends;
						if (m_AssetReverseDependencies.TryGetValue(str, out reverseDepends)) {
							foreach (var depend in reverseDepends) {
								if (queue.Contains(depend))
									continue;
								queue.Enqueue(depend);
							}
						}
					}
					if (list.Count > 0) {
						m_CombinedAssets.Add("[TP]" + list.GetHashCode(), list);
					}
				}
			}

			private void GenerateCombinedAssets()
			{
				var array = new HashSet<string>[m_CombinedAssets.Count];
				m_CombinedAssets.Values.CopyTo(array, 0);

				for (int i = array.Length - 1; i > 0; i--) {
					var a = array[i];
					for (int j = 0; j < i; j++) {
						var b = array[j];
						if (a.IsIntersectOf(b)) {
							b.UnionWith(a);
							array[i] = null;
							break;
						}
					}
				}
				for (int i = 0; i < array.Length; i++) {
					var arr = array[i];
					if (arr != null) {
						var name = "[PP]" + arr.GetHashCode();
						var combineAsset = new CombineAsset(name);
						foreach (var v in arr) {
							Asset info = null;
							if (m_AssetInfos.TryGetValue(v, out info)) {
								combineAsset.packages.UnionWith(info.packages);
								m_AssetInfos[v] = new ReferenceAsset(v, combineAsset);
							}
							combineAsset.assets.Add(v);
						}
						m_AssetInfos.Add(name, combineAsset);
					}
				}
			}

			private void GenerateRawPackages()
			{
				var names = new List<string>();
				var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
				Dictionary<string, Package> packages = new Dictionary<string, Package>();
				Package scenePackage = new Package("##Scene##");
				Package shaderPackage = new Package(XazHelper.XazUnityShadersName); // 特定名称，同XazHelper.XazUnityShadersName
				foreach (var kv in m_AssetInfos) {
					var assetInfo = kv.Value;
					if (assetInfo is ReferenceAsset)
						continue;

					if (assetInfo.GetType() == typeof(SingleAsset)) {
						if (assetInfo.name.ToLower().EndsWith(".unity")) {
							assetInfo.packages.Add(scenePackage);
						} else if (assetInfo.name.ToLower().EndsWith(".shader")) { // 所有shader放到一个package里
							assetInfo.packages.Clear();
							assetInfo.packages.Add(shaderPackage);
						}
					}

					var packageName = string.Empty;
					if (assetInfo.packages.Count == 1) {
						foreach (var pkg in assetInfo.packages) {
							packageName = pkg.name;
							break;
						}
					} else {
						names.Clear();
						foreach (var pkg in assetInfo.packages) {
							//if (!names.Contains(pkg.group)) {
							//	names.Add(pkg.group);
							//}
							names.Add(pkg.name);
						}
						names.Sort();
						packageName = string.Join("_", names.ToArray());
					}
					Package package = null;
					if (!packages.TryGetValue(packageName, out package)) {
						package = new Package(assetInfo.packages.Count == 1 ? packageName : string.Format("SharedAssets/{0}{1}", BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(packageName))).Replace("-", "").ToLower(), XazHelper.XazAssetExtension));
						//package = new Package(packageName);
						packages.Add(packageName, package);
					}
					package.assets.Add(assetInfo.name);
					assetInfo.package = package;
				}

				var builder = new StringBuilder();
				foreach (var kv in packages) {
					builder.AppendLine("Raw Pkg: " + kv.Value.name + "(" + kv.Key + ")");
					foreach (var asset in kv.Value.assets) {
						builder.AppendLine("\t" + asset);
					}
				}
				Debug.Log(builder.ToString());
				this.packages = new List<Package>(packages.Values);
			}

			private string[] GetDependencies(string pathName, bool recursive)
			{
				var depends = AssetDatabase.GetDependencies(pathName, recursive);
				if (pathName.ToLower().EndsWith(".unity")) {
					var list = new List<string>(depends);
					for (int i = 0; i < list.Count; i++) {
						var path = list[i];
						if (path.EndsWith("LightingData.asset")) { // Editor only objects cannot be included in AssetBundles (LightingData)
							if (!recursive) {
								foreach (var p in AssetDatabase.GetDependencies(path, false)) {
									if (p == path || p == pathName)
										continue;
									list.Add(p);
								}
							}
							list.RemoveAt(i);
							break;
						}
					}
					depends = list.ToArray();
				}
				return depends;
			}

			private void ConvertCombinedToAssets()
			{
				HashSet<string> usedNames = new HashSet<string>();
				foreach (var package in packages) {
					usedNames.Add(package.name);
					var arr = new string[package.assets.Count];
					package.assets.CopyTo(arr);
					foreach (var asset in arr) {
						Asset assetInfo = null;
						if (m_AssetInfos.TryGetValue(asset, out assetInfo)) {
							if (assetInfo is CombineAsset) {
								package.assets.Remove(asset);
								package.assets.UnionWith(((CombineAsset)assetInfo).assets);
							}
						}
					}
				}

				// 优化Package的名字
				foreach (var package in packages) {
					if (package.name.StartsWith("SharedAssets/")) {
						var pkgName = "";
						bool hasScene = false;
						foreach (var s in package.assets) {
							string name;
							if (m_ExportableAssets.TryGetValue(s, out name)) {
								if (!hasScene) {
									hasScene = s.ToLower().EndsWith(".unity");
								}
								if (string.IsNullOrEmpty(pkgName)) {
									pkgName = name;
								} else if (pkgName != name) {
									pkgName = string.Empty;
									break;
								}
							}
						}
						if (!string.IsNullOrEmpty(pkgName)) {
							if (usedNames.Contains(pkgName)) {
								if (!hasScene)
									continue;
								var p = packages.Find((pkg) => pkg.name == pkgName);
								if (p != null) {
									p.name = package.name;
								}
							}
							Debug.LogFormat("Pkg Name: {0} => {1}", package.name, pkgName);
							usedNames.Add(pkgName);
							package.name = pkgName;
						}
					}
				}

				var builder = new StringBuilder();
				foreach (var pkg in packages) {
					builder.AppendLine("Pkg: " + pkg.name);
					foreach (var asset in pkg.assets) {
						builder.AppendLine("\t" + asset);
					}
				}
				Debug.Log(builder.ToString());
			}

			private void CollectDependencies(string assetPath, Package package, bool exportable)
			{
				if (exportable) {
					string oldName;
					if (m_ExportableAssets.TryGetValue(assetPath, out oldName)) {
						if (oldName != package.name) {
							m_ExportableAssets[assetPath] = string.Empty;
						}
					} else {
						m_ExportableAssets.Add(assetPath, package.name);
					}
				}
				if (CollectAssetPath(assetPath, package)) {
					foreach (var path in GetDependencies(assetPath, false)) {
						if (path != assetPath) {
							var lpath = path.ToLower();
							if (lpath.EndsWith(".cs") || lpath.EndsWith(".js") || lpath.EndsWith(".dll")) {
								// 忽略脚本文件
								continue;
							}
							// 记录依赖关系
							HashSet<string> hashSet = null;
							if (!m_AssetReverseDependencies.TryGetValue(path, out hashSet)) {
								hashSet = new HashSet<string>();
								m_AssetReverseDependencies[path] = hashSet;
							}
							hashSet.Add(assetPath);
							// 递归查找依赖
							CollectDependencies(path, package, false);
						}
					}
				}
			}

			private bool CollectAssetPath(string assetPath, Package package)
			{
				Asset assetInfo = null;
				if (!m_AssetInfos.TryGetValue(assetPath, out assetInfo)) {
					var importer = TextureImporter.GetAtPath(assetPath) as TextureImporter;
					if (importer != null) {
						if (importer.textureType == TextureImporterType.Sprite) {
							var tag = importer.spritePackingTag;
							if (!string.IsNullOrEmpty(tag)) {
								tag = "[SP]" + tag;
								HashSet<string> combined = null;
								if (!m_CombinedAssets.TryGetValue(tag, out combined)) {
									combined = new HashSet<string>();
									m_CombinedAssets.Add(tag, combined);
								}
								combined.Add(assetPath);
							} else {
								m_NonTaggedTextures.Add(assetPath);
							}
						}
					}
					assetInfo = new SingleAsset(assetPath);
					m_AssetInfos.Add(assetPath, assetInfo);
				}
				return assetInfo.packages.Add(package);
			}
		}
	}

	static class EnumerableExtension
	{
		static public bool IsIntersectOf<T>(this ICollection<T> first, ICollection<T> second)
		{
			ICollection<T> a, b;
			if (first.Count > second.Count) {
				b = first;
				a = second;
			} else {
				a = first;
				b = second;
			}
			foreach (var o in a) {
				if (b.Contains(o)) {
					return true;
				}
			}
			return false;
		}
	}
}
