//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Xaz
{
	
	public class UIAtlas : MonoBehaviour, ISerializationCallbackReceiver
	{
		public UIAtlas replacement;

		[SerializeField]
		private List<Sprite> m_Sprites = new List<Sprite>();
		[SerializeField]
		private List<string> m_SpriteNames = new List<string>();

		[SerializeField]
		private string m_DirectoryName = string.Empty;
		[SerializeField]
		private string m_ExtensionName = string.Empty;
		[SerializeField]
		private List<string> m_SpritePaths = new List<string>();

		public Dictionary<string, Sprite> m_SpriteDict = new Dictionary<string, Sprite>();
		private Dictionary<string, string> m_SpritePathDict = new Dictionary<string, string>();

		public int spriteCount
		{
			get
			{
#if UNITY_EDITOR
				if (!UnityEditor.EditorApplication.isPlaying) {
					if (!string.IsNullOrEmpty(m_DirectoryName)) {
						return m_Sprites.Count + System.IO.Directory.GetFiles(m_DirectoryName, "*" + m_ExtensionName, System.IO.SearchOption.TopDirectoryOnly).Length;
					}
				}
#endif
				return m_Sprites.Count + m_SpritePaths.Count;
			}
		}

		public Sprite GetSprite(string name)
		{
			if (replacement != null) {
				return replacement.GetSprite(name);
			}
			return GetSpriteByName(name);
		}

		private Sprite GetSpriteByName(string name)
		{
			Sprite sprite = null;
			if (!m_SpriteDict.TryGetValue(name, out sprite)) {
				string spritePath = string.Empty;
				if (!string.IsNullOrEmpty(m_DirectoryName)) {
					spritePath = string.Format("{0}/{1}{2}", m_DirectoryName, name, m_ExtensionName);
				} else {
					m_SpritePathDict.TryGetValue(name, out spritePath);
				}
				if (!string.IsNullOrEmpty(spritePath)) {
#if UNITY_EDITOR
					if (!Application.isPlaying)
						sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
					else
#endif
						sprite = Xaz.Assets.LoadAsset<Sprite>(spritePath);
				}
			}
			return sprite;
		}

#if UNITY_EDITOR
		
		public List<Sprite> GetSprites()
		{
			if (replacement != null)
				return replacement.GetSprites();

			if (string.IsNullOrEmpty(m_DirectoryName) && m_SpritePaths.Count == 0) {
				return m_Sprites;
			}

			List<Sprite> sprites = new List<Sprite>();
			if (!string.IsNullOrEmpty(m_DirectoryName)) {
				foreach (var path in System.IO.Directory.GetFiles(m_DirectoryName, "*" + m_ExtensionName, System.IO.SearchOption.TopDirectoryOnly)) {
					var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
					if (sprite != null) {
						sprites.Add(sprite);
					}
				}
			} else {
				foreach (var path in m_SpritePaths) {
					var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
					if (sprite != null) {
						sprites.Add(sprite);
					}
				}
			}
			sprites.AddRange(m_Sprites);
			return sprites;
		}

		
		public void SetSprites(Sprite[] sprites)
		{
			m_Sprites.Clear();
			m_SpriteNames.Clear();
			m_SpritePaths.Clear();
			m_DirectoryName = null;
			m_ExtensionName = string.Empty;

			if (sprites != null) {
				var tags = new Dictionary<string, List<KeyValuePair<string, Sprite>>>();
				foreach (var sprite in sprites) {
					var spritePath = UnityEditor.AssetDatabase.GetAssetPath(sprite);
					var importer = UnityEditor.TextureImporter.GetAtPath(spritePath) as UnityEditor.TextureImporter;
					var spriteTag = importer.spritePackingTag;
					if (string.IsNullOrEmpty(spriteTag)) {
						spriteTag = "###" + spritePath + "###";
					}
					List<KeyValuePair<string, Sprite>> list;
					if (!tags.TryGetValue(spriteTag, out list)) {
						list = new List<KeyValuePair<string, Sprite>>();
						tags[spriteTag] = list;
					}
					list.Add(new KeyValuePair<string, Sprite>(spritePath, sprite));
				}

				foreach (var entry in tags) {
					if (entry.Value.Count == 1) {
						foreach (var kv in entry.Value) {
							var spritePath = kv.Key;
							var directoryName = Path.GetDirectoryName(spritePath);
							var extension = Path.GetExtension(spritePath);
							m_SpritePaths.Add(spritePath);
							if (m_DirectoryName == null) {
								m_DirectoryName = directoryName;
								m_ExtensionName = extension;
							} else {
								if (m_DirectoryName != directoryName || m_ExtensionName != extension) {
									m_DirectoryName = string.Empty;
									m_ExtensionName = string.Empty;
								}
							}
						}
					} else {
						foreach (var kv in entry.Value) {
							m_Sprites.Add(kv.Value);
							m_SpriteNames.Add(kv.Value.name);
						}
					}
				}

				if (!string.IsNullOrEmpty(m_DirectoryName)) {
					m_SpritePaths.Clear();
				}
			}
		}
#endif

		
		public void OnBeforeSerialize()
		{
		}

		
		public void OnAfterDeserialize()
		{
			m_SpriteDict.Clear();
			m_SpritePathDict.Clear();

			if (m_Sprites.Count != m_SpriteNames.Count)
				throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

			for (int i = m_Sprites.Count - 1; i >= 0; i--) {
				m_SpriteDict[m_SpriteNames[i]] = m_Sprites[i];
			}
			foreach (var name in m_SpritePaths) {
				m_SpritePathDict[System.IO.Path.GetFileNameWithoutExtension(name)] = name;
			}
		}

	}
}
