//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
//----------------------------------------------------------------------------
// 置灰组件
//----------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Xaz
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class UIGray : MonoBehaviour, IControl
    {
        private Material m_GrayMaterial;
        /// <summary>
        /// 置灰可点击
        /// </summary>
        [SerializeField]
        private bool m_disableSelect = false;
        /// <summary>
        /// 是否置灰
        /// </summary>
        [SerializeField]
        private bool m_IsGray = false;

        private void Awake()
        {
            m_GrayMaterial = Resources.Load<Material>("Materials/UIDefaultGray");
            SetGray(IsGray);
            SetDisable(m_disableSelect);
        }

        public bool IsGray
        {
            get { return m_IsGray; }
            set
            {
                m_IsGray = value;
                SetGray(IsGray);
            }
        }
        /// <summary>
        /// 是否可点击 true可点击
        /// </summary>
        public bool disableSelect
        {
            get { return m_disableSelect; }
            set
            {
                m_disableSelect = value;
                SetDisable(m_disableSelect);
            }
        }

        private void SetGray(bool isGray)
        {
            Graphic[] graphics = transform.GetComponentsInChildren<Graphic>();
            for (int i = 0; i < graphics.Length; i++)
            {
                if (isGray)
                {
                    graphics[i].material = m_GrayMaterial;
                }
                else
                {
                    graphics[i].material = null;
                }
            }
        }
        /// <summary>
        /// 可否点击
        /// </summary>
        /// <param name="disableSelect"></param>
        private void SetDisable(bool disableSelect)
        {
            Selectable[] selectables = transform.GetComponentsInChildren<Selectable>();
            for (int i = 0; i < selectables.Length; i++)
            {
                selectables[i].interactable = m_IsGray ? disableSelect : true;
            }
            Graphic[] graphics = transform.GetComponentsInChildren<Graphic>();
            for (int i = 0; i < graphics.Length; i++)
            {
                graphics[i].raycastTarget = m_IsGray ? disableSelect : true;
            }
        }
        /// <summary>
        /// 设置置灰
        /// </summary>
        /// <param name="parent">组件对象</param>
        /// <param name="isGray">置灰表现</param>
        /// <param name="">是否支持置灰可点击,只对灰后有效</param>
        static public void SetGray(GameObject parent, bool isGray, bool canclick)
        {
            UIGray gray = parent.GetComponent<UIGray>() ?? parent.AddComponent<UIGray>();
            gray.IsGray = isGray;
            if (isGray)
            {
                gray.disableSelect = canclick;
            }
            else
            {
                gray.disableSelect = true;
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(UIGray))]
    public class UIGrayInspector : Editor
    {
        private SerializedProperty m_IsGrayProperty;
        private SerializedProperty m_disableSelectProperty;
        private void OnEnable()
        {
            m_IsGrayProperty = serializedObject.FindProperty("m_IsGray");
            m_disableSelectProperty = serializedObject.FindProperty("m_disableSelect");
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_IsGrayProperty, new GUIContent("IsGray"));
            EditorGUILayout.PropertyField(m_disableSelectProperty, new GUIContent("disableSelect"));

            if (serializedObject.ApplyModifiedProperties())
            {
                (target as UIGray).IsGray = m_IsGrayProperty.boolValue;
                (target as UIGray).disableSelect = m_disableSelectProperty.boolValue;
            }
        }
    }
#endif
}