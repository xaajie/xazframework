//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
//----------------------------------------------------
// Xaz: A Framework For Unity
//----------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace XazEditor
{
    [CustomEditor(typeof(Xaz.UIScroller), true)]
	public class UIScrollerInspector : ComponentEditor
	{
		protected override void DrawCustomProperties()
		{
			XazEditorTools.SetLabelWidth(120f);

			XazEditorTools.DrawProperty("Direction", serializedObject, "m_Direction");

			SerializedProperty movementType = serializedObject.FindProperty("m_MovementType");
			XazEditorTools.DrawProperty("MovementType", movementType);
			if (movementType.intValue == (int)Xaz.UIScroller.MovementType.Elastic) {
				GUILayout.BeginHorizontal();
				GUILayout.Space(15f);
				XazEditorTools.SetLabelWidth(110f);
				XazEditorTools.DrawProperty("Elasticity", serializedObject, "m_Elasticity");
				XazEditorTools.SetLabelWidth(120f);
				GUILayout.EndHorizontal();
			}

			SerializedProperty inertia = serializedObject.FindProperty("m_Inertia");
			XazEditorTools.DrawProperty("Inertia", inertia);
			if (inertia.boolValue) {
				GUILayout.BeginHorizontal();
				GUILayout.Space(15f);
				XazEditorTools.SetLabelWidth(110f);
				XazEditorTools.DrawProperty("DecelerationRate", serializedObject, "m_DecelerationRate");
				XazEditorTools.SetLabelWidth(120f);
				GUILayout.EndHorizontal();
			}

            XazEditorTools.DrawProperty("DragLock", serializedObject, "m_DragLock");

            XazEditorTools.DrawProperty("scrollBar", serializedObject, "m_ScrollBar");
            XazEditorTools.DrawProperty("scrollBarType", serializedObject, "m_BarShowType");
            base.DrawCustomProperties();
		}
	}
}