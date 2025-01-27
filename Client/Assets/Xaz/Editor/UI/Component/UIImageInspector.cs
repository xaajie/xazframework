//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
//----------------------------------------------------
// Xaz: A Framework For Unity
//----------------------------------------------------

using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using Xaz;
namespace XazEditor
{
    [CustomEditor(typeof(Xaz.UIImage), true)]
	public class UIImageInspector : UnityEditor.UI.ImageEditor
	{
		public override void OnInspectorGUI()
		{
			XazEditorTools.SetLabelWidth(120f);
			GUILayout.Space(3f);
			serializedObject.Update();

			EditorGUILayout.BeginHorizontal();
            SerializedProperty sp = XazEditorTools.DrawProperty("UI Atlas", serializedObject, "m_Atlas");
            SpriteAtlas atlas = sp.objectReferenceValue as SpriteAtlas;
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			sp = serializedObject.FindProperty("m_SpriteName");
            EditorGUILayout.LabelField("Sprite Name", GUILayout.Width(116f));
			if (GUILayout.Button(string.IsNullOrEmpty(sp.stringValue) ? "Select" : sp.stringValue, "DropDown")) {
				UIAtlasSpritesWindow.Show(atlas, serializedObject, sp);
			}
			EditorGUILayout.EndHorizontal();
            XazEditorTools.DrawProperty("Force Native Size", serializedObject, "m_ForceNativeSize");
            XazEditorTools.DrawProperty("Show Before Load", serializedObject, "m_ShowBeforeLoad");
            XazEditorTools.DrawProperty("Atlas Name(默认不填)", serializedObject, "atlasName");
            serializedObject.ApplyModifiedProperties();

			base.OnInspectorGUI();
		}

        [MenuItem("GameObject/UI/UIImage")]
        static private void UIImageCreatev()
        {
            if (Selection.activeTransform)
            {
                GameObject go = new GameObject("UIImage");
                go.transform.tag = UIViewExporter.UIPropertyTagName;
                go.AddComponent<UIImage>();
                go.transform.SetParent(Selection.activeTransform);
                go.GetComponent<UIImage>().rectTransform.localScale = Vector3.one;
                go.GetComponent<UIImage>().rectTransform.localPosition = Vector3.zero;
            }
        }
    }
}
