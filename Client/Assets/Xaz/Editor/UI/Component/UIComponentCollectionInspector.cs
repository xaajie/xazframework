//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XazEditor
{
	[CustomEditor(typeof(Xaz.UIComponentCollection), true)]
	public class UIComponentCollectionInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			XazEditorTools.SetLabelWidth(120f);
			GUILayout.Space(3f);

			EditorGUI.BeginDisabledGroup(true);

			SerializedProperty components = serializedObject.FindProperty("components");
			EditorGUILayout.LabelField("count:" + components.arraySize);
			for (int i = 0; i < components.arraySize; i++) {
				GUILayout.Space(2f);
				SerializedProperty item = components.GetArrayElementAtIndex(i);
				EditorGUILayout.ObjectField(item.objectReferenceValue, typeof(Component), true);
			}

			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
