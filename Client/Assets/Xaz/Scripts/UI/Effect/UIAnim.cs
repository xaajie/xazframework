//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//  ui界面上动效控制器
//  @author xiejie 
//----------------------------------------------

using System;
using System.Collections;
using UnityEngine;


namespace Xaz
{
    
    public class UIAnim : MonoBehaviour, IControl
    {

        public enum UIAnimType
        {
            //界面关闭条件outro
            ViewClose = 99,
            //初始触发
            INIT = 1,
            //cell选中触发select
            CellClick = 2,
        }

        
        public delegate void OnAniBackEvent(string vt);

        public Animator ani;
        private bool canPlay = true;
        public bool isAutoPlay = false;
        public OnAniBackEvent onPlayFinishEvent;
        private static string aniKey = "uistate";
        private Coroutine co;
        void Start()
        {
            if (ani == null)
            {
                ani = this.gameObject.GetComponent<Animator>();
            }
            if (isAutoPlay)
            {
                SetUIAnimState((int)UIAnimType.INIT);
            }
        }

        public void ResetAni()
        {
            if (co != null)
            {
                StopCoroutine(co);
                co = null;
            }
            if (ani)
            {
                ani.SetInteger(aniKey, (int)UIAnim.UIAnimType.INIT);
            }
        }
        public void PlayFinishEvent(string aid)
        {
            if (onPlayFinishEvent != null)
            {
                onPlayFinishEvent(aid);
            }
        }

        public void SetUIAnimState(int vt, string statename = null, OnAniBackEvent callback = null)
        {
            if (!canPlay)
            {
                return;
            }
            if (ani && ani.enabled)
            {
                ani.SetInteger(aniKey, vt);
                if (callback != null && statename!=null)
                {
                    co =StartCoroutine(WaitAnimFinish(ani, statename, callback));
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(statename);
                }
            }
        }

        private  IEnumerator WaitAnimFinish(Animator anim, string animName, OnAniBackEvent OnPlayFinish)
        {
            int stateid = Animator.StringToHash(animName);
            
            //等待动画播放完成
            while (anim.HasState(0, stateid) && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 || !anim.GetCurrentAnimatorStateInfo(0).IsName(animName)))
            {
                yield return XazHelper.waitFrame;
            }

            if (OnPlayFinish != null)
            {
                OnPlayFinish(animName);
            }
            co = null;
        }

        void OnEnable()
        {
            canPlay = true;
            //ani.Play(curStateName, 0, 0);
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