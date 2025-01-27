//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Xaz;

namespace XazEditor
{
	public class UIAtlasProcessor : AssetPostprocessor
	{
		public enum TagMode
		{
			None,
			Custom,
			Single,
			Group,
		}

		public struct CompressionSettings
		{
			public int compressionQuality;
#if UNITY_5_5_OR_NEWER
			public bool crunchedCompression;
			public TextureImporterCompression textureCompression;
#endif
		}

		public struct PlatformSettings
		{
			public int maxTextureSize;
			public TextureImporterFormat textureFormat;
			public HashSet<TextureImporterFormat> allowedTextureFormats;
			public bool? allowsAlphaSplit;
			public CompressionSettings? compression;
		}

		public struct SpriteSettings
		{
			public string tag;
			public TagMode tagMode;
			public bool mipmapEnabled;
			public PlatformSettings? android;
			public PlatformSettings? ios;
			public PlatformSettings? standalone;
			public CompressionSettings compression;
#if !UNITY_5_5_OR_NEWER
			public TextureImporterFormat format;
#endif
		}

		public struct AtlasSettings
		{
			public string name;
			public string atlasPath;
			public bool isRoot;
			public bool recursive;
			public bool deleteIfEmpty;
			public List<string> excludes;
			public List<string> includes;
		}

		public struct Rule
		{
			public string assetPath;
			public AtlasSettings? atlasSettings;
			public SpriteSettings spriteSettings;
		}

		static private List<Rule> m_Rules = new List<Rule>();
		static private HashSet<Rule> m_RuleDirtyHashSet = new HashSet<Rule>();

		public static void AddRule(Rule rule)
		{
			m_Rules.Add(rule);
		}

		void OnPreprocessTexture()
		{
			UpdateTextureImporter(assetPath, (TextureImporter)assetImporter, false);
		}

		static void UpdatePlatformTextureSettings(TextureImporter importer, string platform, PlatformSettings? settings)
		{
			if (settings.HasValue) {
				var platformSettings = settings.Value;
				int maxTextureSize = platformSettings.maxTextureSize;
				if (maxTextureSize == 0) {
					maxTextureSize = importer.maxTextureSize;
				}
#if UNITY_5_5_OR_NEWER
				var importerSettings = importer.GetPlatformTextureSettings(platform);
				importerSettings.overridden = true;
				importerSettings.maxTextureSize = maxTextureSize;
				var format = importerSettings.format;
				var allowedTextureFormats = platformSettings.allowedTextureFormats;
				if (allowedTextureFormats == null || allowedTextureFormats.Count == 0 || !allowedTextureFormats.Contains(format)) {
					importerSettings.format = platformSettings.textureFormat;
				}
				if (platformSettings.compression.HasValue) {
					var compression = platformSettings.compression.Value;
					importerSettings.compressionQuality = compression.compressionQuality;
					importerSettings.crunchedCompression = compression.crunchedCompression;
					importerSettings.textureCompression = compression.textureCompression;
				} else {
					importerSettings.compressionQuality = importer.compressionQuality;
					importerSettings.crunchedCompression = importer.crunchedCompression;
					importerSettings.textureCompression = importer.textureCompression;
				}
				if (platformSettings.allowsAlphaSplit.HasValue) {
					importerSettings.allowsAlphaSplitting = platformSettings.allowsAlphaSplit.Value;
				}
				importer.SetPlatformTextureSettings(importerSettings);
#else
				importer.SetPlatformTextureSettings(platform, maxTextureSize, platformSettings.textureFormat, platformSettings.compression.HasValue ? platformSettings.compression.Value.compressionQuality : 100, platformSettings.allowsAlphaSplit.HasValue ? platformSettings.allowsAlphaSplit.Value : false);
#endif
			} else {
				importer.ClearPlatformTextureSettings(platform);
			}
		}
		static void UpdateTextureImporter(string assetPath, TextureImporter importer, bool reimport)
		{
			foreach (var rule in m_Rules) {
				var relativePath = assetPath.Replace(rule.assetPath + "/", "");
				if (relativePath == assetPath)
					continue;

				var atlasPath = rule.assetPath;
				if (rule.atlasSettings.HasValue) {
					AtlasSettings atlasSettings = rule.atlasSettings.Value;
					int index = relativePath.IndexOf("/");
					if (atlasSettings.isRoot) {
						if (index < 0)
							continue;
						if (atlasSettings.excludes != null && atlasSettings.excludes.Contains(relativePath.Substring(0, index)))
							continue;
						if (atlasSettings.includes != null && !atlasSettings.includes.Contains(relativePath.Substring(0, index)))
							continue;
					}
					if (!atlasSettings.recursive && relativePath.IndexOf("/", index + 1) >= 0)
						continue;
					atlasPath = atlasSettings.isRoot ? (rule.assetPath + "/" + relativePath.Substring(0, index)) : rule.assetPath;
				}

				var settings = rule.spriteSettings;
				importer.textureType = TextureImporterType.Sprite;
				importer.spriteImportMode = SpriteImportMode.Single;
				importer.mipmapEnabled = settings.mipmapEnabled;
				var packingTag = string.Empty;
				if (settings.tagMode == TagMode.Custom) {
					if (!string.IsNullOrEmpty(settings.tag))
						packingTag = settings.tag;
				} else if (settings.tagMode == TagMode.Single) {
					packingTag = "~" + assetPath.Replace('/', '.');
				} else if (settings.tagMode == TagMode.Group) {
					packingTag = atlasPath.Replace('/', '.');
				}
				importer.spritePackingTag = packingTag;
#if UNITY_5_5_OR_NEWER
				importer.textureCompression = settings.compression.textureCompression;
				importer.crunchedCompression = settings.compression.crunchedCompression;
				importer.compressionQuality = settings.compression.compressionQuality;
#else
				importer.textureFormat = settings.format;
#endif

				UpdatePlatformTextureSettings(importer, "Android", settings.android);
				UpdatePlatformTextureSettings(importer, "iPhone", settings.ios);
				UpdatePlatformTextureSettings(importer, "Standalone", settings.standalone);

				if (reimport) {
					importer.SaveAndReimport();
				}
				break;
			}
		}

		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
		{
			if (movedAssets.Length > 0) {
				AssetDatabase.StartAssetEditing();
				foreach (var path in movedAssets) {
					var importer = TextureImporter.GetAtPath(path) as TextureImporter;
					if (importer != null) {
						UpdateTextureImporter(path, importer, true);
					}
				}
				AssetDatabase.StopAssetEditing();
			}
			foreach (var rule in m_Rules) {
				if (rule.atlasSettings.HasValue) {
					if (rule.atlasSettings.Value.isRoot) {
						foreach (var childPath in AssetDatabase.GetSubFolders(rule.assetPath)) {
							AtlasSettings atlasSettings = rule.atlasSettings.Value;
							var dirName = Path.GetFileNameWithoutExtension(childPath);
							if (atlasSettings.excludes == null || !atlasSettings.excludes.Contains(dirName)) {
								bool changed = CheckAssetsChanged(childPath, importedAssets, deletedAssets, movedAssets, movedFromPath);
								if (changed) {
									var childRule = rule;
									if (!string.IsNullOrEmpty(atlasSettings.name)) {
										atlasSettings.name = atlasSettings.name + dirName.ToUpperFirst();
									} else {
										atlasSettings.name = dirName.ToUpperFirst();
									}
									atlasSettings.excludes = null;
									childRule.assetPath = childPath;
									childRule.atlasSettings = atlasSettings;
									m_RuleDirtyHashSet.Add(childRule);
								}
							}
						}
					} else {
						bool changed = CheckAssetsChanged(rule.assetPath, importedAssets, deletedAssets, movedAssets, movedFromPath);
						if (changed) {
							m_RuleDirtyHashSet.Add(rule);
						}
					}
				}
			}
			if (m_RuleDirtyHashSet.Count > 0) {
				EditorApplication.delayCall = () => {
					Debug.Log("<color=olive>[UIAtlas-AutoGenerated] Detect some assets changed, repacking ...</color>");
					Rule[] rules = m_RuleDirtyHashSet.ToArray();
					m_RuleDirtyHashSet.Clear();
					foreach (var rule in rules) {
						AtlasSettings atlasSettings = rule.atlasSettings.Value;
						var atlasPath = atlasSettings.atlasPath + "/" + atlasSettings.name + ".prefab";
						if (Directory.Exists(rule.assetPath)) {
							var sprites = XazEditorHelper.GetAssetsAtPath<Sprite>(rule.assetPath, atlasSettings.recursive);
							if (sprites.Length > 0 || !atlasSettings.deleteIfEmpty) {
								UIAtlasBuilder.Build<Xaz.UIAtlas>(sprites, atlasPath);
								continue;
							}
						}
						AssetDatabase.DeleteAsset(atlasPath);
					}
				};
			}
		}

		private static bool CheckAssetsChanged(string assetPath, string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
		{
			assetPath = assetPath + "/";
			foreach (var p in importedAssets) {
				if (p.Contains(assetPath)) {
					return true;
				}
			}
			foreach (var p in deletedAssets) {
				if (p.Contains(assetPath)) {
					return true;
				}
			}
			foreach (var p in movedAssets) {
				if (p.Contains(assetPath)) {
					return true;
				}
			}
			foreach (var p in movedFromPath) {
				if (p.Contains(assetPath)) {
					return true;
				}
			}
			return false;
		}
	}
}
