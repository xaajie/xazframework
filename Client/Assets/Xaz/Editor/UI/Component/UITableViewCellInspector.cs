//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
//----------------------------------------------------
// Xaz: A Framework For Unity
//----------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Xaz;

namespace XazEditor
{
	[CustomEditor(typeof(Xaz.UITableViewCell), true)]
	public class UITableViewCellInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			XazEditorTools.SetLabelWidth(120f);
			GUILayout.Space(3f);
			serializedObject.Update();

			XazEditorTools.DrawProperty("Identifier", serializedObject, "identifier");
			XazEditorTools.DrawProperty("Mode", serializedObject, "mode");
            XazEditorTools.DrawProperty("fixRect", serializedObject, "fixRect");
            if (XazEditorTools.DrawHeader("States")) {
				XazEditorTools.BeginContents();
				XazEditorTools.DrawProperty("Normal", serializedObject, "m_NormalState");
				XazEditorTools.DrawProperty("Dimmed", serializedObject, "m_DimmedState");
				XazEditorTools.DrawProperty("Selected", serializedObject, "m_SelectedState");
				XazEditorTools.DrawProperty("Disabled", serializedObject, "m_DisabledState");
				XazEditorTools.EndContents();
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
