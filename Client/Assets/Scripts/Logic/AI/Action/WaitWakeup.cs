using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using Xaz;

namespace NodeCanvas.Tasks.Actions
{

    [Name("等被叫醒")]
    [Category("动作节点")]
    public class WaitWakeup : ActionTask<WorkerCtrl>
    {
        float dropTimer = 0f;
        float dropInterval = Const.InvFlyTime;
        bool isShowAd = false;
        float distance = 1f;
        private WorkerCtrl othertarget;
 
        protected override void OnExecute()
        {
            isShowAd = false;
            dropTimer = 0f;
            if (!IsGuard())
            {
                othertarget = RushManager.Instance.GetGuard();
            }
        }

        private bool IsGuard()
        {
            return this.agent.GetActorType() == Const.ActorType.Guard;
        }

        private bool IsCloseMainPlayer()
        {
            return Vector3.Distance(this.agent.transform.position, RushManager.Instance.mainplayer.transform.position) <= distance;
        }
        protected override void OnUpdate()
        {
            dropTimer += Time.deltaTime;
            if (dropTimer >= dropInterval)
            {
                dropTimer = 0f;
                if (agent.IsWakeuping)
                {
                    return;
                }
                if (isShowAd)
                {
                    return;
                }
                if (agent.GetCtrlData().IsSleep())
                {
                    if (IsCloseMainPlayer())
                    {
                        if (this.agent.GetActorType() == Const.ActorType.Guard)
                        {
                            if(this.agent.startWakeupGuard > 0)
                            {
                               if ((TimeUtil.GetNowTicks() - agent.startWakeupGuard) > Const.GuardBarDuration)
                                {
                                    this.agent.startWakeupGuard = 0;
                                    isShowAd = true;
                                    ModuleMgr.AdMgr.ClickAd(AdEnum.AdType.Reward_Wakeup, (adtype) => {
                                        isShowAd = false;
                                        agent.WakeupAction();
                                        AudioMgr.Instance.Play(AudioEnum.Wakeup);
                                    });
                                }
                            }
                            else
                            {
                                this.agent.startWakeupGuard = TimeUtil.GetNowTicks();
                            }
                        }
                        else
                        {
                            agent.WakeupAction();
                            AudioMgr.Instance.Play(AudioEnum.Wakeup);
                        }
                    }
                    else
                    {
                        if (this.agent.GetActorType() == Const.ActorType.Guard)
                        {
                            if (this.agent.startWakeupGuard > 0)
                            {
                                this.agent.startWakeupGuard = 0;
                            }
                        }
                    }
                }
                else
                {
                    EndAction(true);
                }

            }
        }

    }
}