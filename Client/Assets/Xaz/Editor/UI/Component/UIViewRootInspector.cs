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
	[CustomEditor(typeof(Xaz.UIViewRoot), true)]
	public class UIViewRootInspector : ComponentEditor
	{
		protected override bool ShouldDrawProperties()
		{
			return !Application.isPlaying;
		}
		protected override void DrawCustomProperties()
		{
			XazEditorTools.DrawProperty("Default Mask Color", serializedObject, "m_DefaultMaskColor");

			var sp = serializedObject.FindProperty("m_InvisibleMode");
			XazEditorTools.DrawProperty("Invisible Mode", sp);
			if (sp.enumValueIndex == 1) {
				EditorGUI.indentLevel++;
				sp = serializedObject.FindProperty("m_InvisibleLayer");
				sp.intValue = EditorGUILayout.LayerField("Layer", sp.intValue);
				EditorGUI.indentLevel--;
			}
		}
	}
}
