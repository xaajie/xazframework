//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using UnityEngine;

namespace Xaz
{
    [System.Serializable]
	public class AssetsManifest : ScriptableObject
	{
		[System.Serializable]
		public struct Asset
		{
			[SerializeField]
			public string path;
			[SerializeField]
			public int index;
		}

		[System.Serializable]
		public class Bundle
		{
			[SerializeField]
			public string name;
			[SerializeField]
			public Asset[] assets;
			[SerializeField]
			public string[] scenes;
			[SerializeField]
			public int[] depends;
			[SerializeField]
			public int dependents;
		}

		[SerializeField]
		public Bundle[] bundles;

		[SerializeField]
		public string[] directories;
	}
}
