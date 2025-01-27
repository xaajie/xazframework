//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Reflection;
using UnityEditor.UI;


[CustomEditor(typeof(CanvasRenderer), false)]
public class UIRawImageInspector :Editor {
	public override void OnInspectorGUI ()
	{
		CanvasRenderer canvasRenderer = (target as CanvasRenderer);
		RawImage rawImage = canvasRenderer.GetComponent<RawImage> ();
		if (rawImage != null && rawImage.texture != null) {
			if (GUILayout.Button ("Set Native Size")) {
				SetNativeSize (rawImage);
			}
		} else {
			base.OnInspectorGUI ();
		}
	}

	public static void SetNativeSize(RawImage rawImage){
		try {
			object[] args = new object[2] { 0, 0 };
			MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
			mi.Invoke(TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(rawImage.texture)) as TextureImporter, args);
			int width = (int)args[0];
			int height = (int)args[1];
			rawImage.rectTransform.sizeDelta = new Vector2 (width, height);
		} catch (System.Exception ex) {

		}
	
	}
}
