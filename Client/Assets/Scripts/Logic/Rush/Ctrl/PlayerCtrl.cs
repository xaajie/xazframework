using UnityEngine;

public class PlayerCtrl : ActorController
{
    [SerializeField] private AudioClip[] footsteps;
    private AudioSource audioSource;
    public UserScenePlayerData ctrldata;
    public ParticleSystem tailParticle;
    public Transform landArrow;
    protected override void Awake()
    {
        base.Awake();
        Utils.SetActive(landArrow.gameObject, false);
    }

    public void SetCtrlData(UserScenePlayerData info)
    {
        ctrldata = info;
        SetActorType((int)Const.ActorType.Player);
        UpdateAttr();
    }

    public UserScenePlayerData GetCtrlData()
    {
        return ctrldata;
    }

    Vector3 moveDirection;
    public void MoveByDir(Vector3 movement)
    {
        moveDirection = new Vector3(movement.x, 0f, movement.y);
        SetSpineAnimFlip(movement.x);
        agent.MoveByDir(moveDirection);
        SetSpineAnimState(SpineAnimCtrl.SpineAnimState.Run);
        PlayTail();
    }

    public void PlayTail()
    {
        if (tailParticle.isStopped)
        {
            tailParticle.Play();
        }
    }
    public override void UpdateAttr()
    {
        base.UpdateAttr();
        GetCtrlData().RefreshData();
        agent.SetAgentSpeed(ctrldata.GetSpeedVal());
        if (Stack != null)
        {
            Stack.MaxStack = ctrldata.GetCapacity();
        }
    }

    public void OnStep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight < 0.5f) return;

        audioSource.clip = footsteps[Random.Range(0, footsteps.Length)];
        audioSource.Play();
    }

}

