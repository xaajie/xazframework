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
	[CustomEditor(typeof(Xaz.UIViewSettings), true)]
	public class UIViewSettingsInspector : ComponentEditor
	{
		protected override bool ShouldDrawProperties()
		{
			return !Application.isPlaying;
		}

		protected override void DrawCustomProperties()
		{
			SerializedProperty serializedProperty = serializedObject.FindProperty("m_Style");
            DrawRelativeProperty("Is Popup", serializedProperty, "popup");
            DrawRelativeProperty("Is Topmost", serializedProperty, "topmost");
			var sp = DrawRelativeProperty("Is Overlapped", serializedProperty, "overlapped");
			if (!sp.boolValue) {
				sp = DrawRelativeProperty("Override Color", serializedProperty, "overrideColor");
				if (sp.boolValue) {
					XazEditorTools.BeginIndent();
					DrawRelativeProperty("Mask Color", serializedProperty, "maskColor");
					XazEditorTools.EndIndent();
				}
			}

            XazEditorTools.DrawProperty("Is NeedSafeArea", serializedObject, "needSafeArea");
            XazEditorTools.DrawProperty("Open Auido", serializedObject, "openAuido");
            sp = XazEditorTools.DrawProperty("Override Mode", serializedObject, "overrideMode");
            if (sp.boolValue) {
				XazEditorTools.BeginIndent();
				sp = XazEditorTools.DrawProperty("Invisible Mode", serializedObject, "invisibleMode");
				if (sp.enumValueIndex == 1) {
					XazEditorTools.BeginIndent();
					sp = serializedObject.FindProperty("invisibleLayer");
					sp.intValue = EditorGUILayout.LayerField("Layer", sp.intValue);
					XazEditorTools.EndIndent();
				}
				XazEditorTools.EndIndent();
			}
		}

		SerializedProperty DrawRelativeProperty(string label, SerializedProperty serializedProperty, string propertyName)
		{
			SerializedProperty property = serializedProperty.FindPropertyRelative(propertyName);
			if (property != null) {
				EditorGUI.BeginChangeCheck();
				XazEditorTools.DrawProperty(label, property);
				if (EditorGUI.EndChangeCheck())
					serializedObject.ApplyModifiedProperties();
			}
			return property;
		}
	}
}
