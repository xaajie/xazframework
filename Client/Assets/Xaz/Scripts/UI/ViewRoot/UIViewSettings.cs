//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using UnityEngine;

namespace Xaz
{
    public class UIViewSettings : MonoBehaviour
	{
		[SerializeField]
		private UIStyle m_Style;

        [SerializeField]
        internal bool needSafeArea = false;

        [SerializeField]
        internal bool openAuido = true;

        [SerializeField]
		internal bool overrideMode = false;

		[SerializeField]
		internal UIViewRoot.InvisibleMode invisibleMode;

		[SerializeField]
		internal int invisibleLayer;

		private UIStyle m_RuntimeStyle;

		public UIStyle style
		{
			get
			{
				return m_RuntimeStyle;
			}
			internal set
			{
				m_Style = value;
			}
		}

		public bool IsNeedCountSafe()
		{
			return needSafeArea;
		}

        void Awake()
		{
			m_RuntimeStyle = m_Style;
		}
	}
}
