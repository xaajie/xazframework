//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;

namespace Xaz
{
    public class UITabButton : Toggle
    {
        [SerializeField]
        private UIState m_State;

        [SerializeField]
        private string m_OnStateName;
        [SerializeField]
        private string m_OffStateName;

        protected UITabButton()
        {
            onValueChanged.AddListener(ChangeState);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ChangeState(false);
        }

        public void SetStateTarget(UIState vt)
        {
            m_State = vt;
            m_OnStateName = vt.states[0].name;
            m_OffStateName = vt.states[1].name;
        }
#endif

        protected override void Start()
        {
            base.Start();
            ChangeState(isOn);
        }

        private void ChangeState(bool isOn)
        {
            if (m_State)
            {
                if (isOn)
                    m_State.SetState(m_OnStateName);
                else
                    m_State.SetState(m_OffStateName);
            }
        }

        public void SetIsTabOnWithoutNotify(bool isOn)
        {
            SetIsOnWithoutNotify(isOn);
            ChangeState(isOn);
        }
    }
}
