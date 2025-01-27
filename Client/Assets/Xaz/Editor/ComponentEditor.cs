//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace XazEditor
{
	public class ComponentEditor : Editor
	{
		/// <summary>
		/// Draw the inspector properties.
		/// </summary>

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginDisabledGroup(!ShouldDrawProperties());
			DrawCustomProperties();
			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();
		}

		protected virtual bool ShouldDrawProperties()
		{
			return true;
		}
		protected virtual void DrawCustomProperties()
		{
		}

	}
}
