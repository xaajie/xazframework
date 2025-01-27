//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//  ui组件触发动效连接器
//  @author xiejie 
//----------------------------------------------

using System;
using System.Collections;
using UnityEngine;

namespace Xaz
{
    public class UIAnimComp : MonoBehaviour
    {
        public delegate void OnAniBackEvent(string vt);

        public Animator ani;
        private static string aniKey = "uistate";
        private string curStateName = "";
        private Coroutine co;

        private bool canPlay = true;
        void Start()
        {
            if (ani == null)
            {
                ani = this.gameObject.GetComponent<Animator>();

            }
            //SetUIAnimCompState(1);
        }


        public void SetUIAnimCompState(int vt, string statename=null, Action callback = null)
        {
            if (!canPlay)
            {
                return;
            }
            if (vt == (int)UIAnim.UIAnimType.INIT)
            {
                ResetAni();
            }
            if (ani)
            {
                ani.SetInteger(aniKey, vt);
                if (!string.IsNullOrEmpty(statename))
                {
                    curStateName = statename;
                    co = StartCoroutine(WaitAnimFinish(ani, statename, callback));
                }
            }
        }

        private IEnumerator WaitAnimFinish(Animator anim, string animName, Action OnPlayFinish)
        {
            //等待动画播放完成
            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 || !anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
            {
                yield return XazHelper.waitFrame;
            }

            anim.SetInteger(aniKey, (int)UIAnim.UIAnimType.INIT);
            curStateName = Const.nullStr;
            if (OnPlayFinish != null)
            {
                OnPlayFinish();
            }
            co = null;
        }

        void OnEnable()
        {
            canPlay = true;
            //ani.Play(curStateName, 0, 0);
        }

        private void ResetAni()
        {
            if (co != null)
            {
                StopCoroutine(co);
                co = null;
            }
            if (curStateName != "" && ani)
            {
                ani.SetInteger(aniKey, (int)UIAnim.UIAnimType.INIT);
                //ani.Play(curStateName, 0, 1);
                //ani.Update(0);
                curStateName = Const.nullStr;
            }
        }
        void OnDisable()
        {
            canPlay = false;
            ResetAni();
        }

        void OnDestroy()
        {
            canPlay = false;
            ResetAni();
        }
    }
}