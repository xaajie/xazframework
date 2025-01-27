using System;
using UnityEngine;
using UnityEngine.AI;
using Xaz;


public class ActorController : MonoBehaviour
{
    [SerializeField] private HandStack stack;
    public HandStack Stack => stack;
    private SpineAnimCtrl animator;
    public ActorMoveController agent;
    [HideInInspector] public Const.ActorStep step;
    [HideInInspector] public LayerMask entranceLayer;
    [HideInInspector] public int taskproductId;
    [HideInInspector] public int taskproductNum;
    private Vector3 pickOffsetRight = new Vector3(0.6f,0.5f, -0.1f);
    private Vector3 pickOffsetLeft = new Vector3(0f, 0.5f, -0.6f);
    protected Const.ActorType actorType;
    protected virtual void Awake()
    {
        entranceLayer = 1 << LayerMask.NameToLayer(XazConfig.LayerDefine.SceneDoor);
        agent = new ActorMoveController();
        agent.SetTarget(gameObject);
        stack.SetOwner(this);
        animator = gameObject.GetComponent<SpineAnimCtrl>();
        SetTargetAnim(SpineAnimCtrl.SpineAnimState.Idle);
    }
    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (agent != null)
        {
            agent.UpdateMove();
        }
    }

    public virtual void UpdateAttr()
    {

    }

    public Const.ActorType GetActorType()
    {
        return actorType;
    }

    public void SetActorType(int v)
    {
        actorType = (Const.ActorType)Enum.Parse(typeof(Const.ActorType), v.ToString());
        if (agent != null)
        {
            agent.SetAudio(actorType == Const.ActorType.Player);
        }
    }

    public virtual void SetActorStep(Const.ActorStep st)
    {
        step = st;
        if (actorType == Const.ActorType.Customer)
        {
          RushManager.Instance.RefreshFloating(this);
        }
    }

    public void SetSpineAnimFlip(float horizontal)
    {
        if (animator != null && horizontal!=0)
        {
            stack.transform.localPosition = horizontal > 0 ? pickOffsetRight : pickOffsetLeft;
            animator.SetFlip(horizontal);
        }
    }

    public void AddHandStack(Transform chile,int productId, bool showJump = true)
    {
        Stack.AddToHandStack(chile, productId, showJump);
        SetSpineAnimState(SpineAnimCtrl.SpineAnimState.Idle);
    }

    public void SetTargetAnim(SpineAnimCtrl.SpineAnimState vt)
    {
        SetSpineAnimState(vt);
    }
    protected virtual void SetSpineAnimState(SpineAnimCtrl.SpineAnimState vt)
    {
        if (animator != null)
        {
            if(vt == SpineAnimCtrl.SpineAnimState.Run)
            {
                animator.SetAnimState(stack.Count > 0 ? SpineAnimCtrl.SpineAnimState.PickRun:SpineAnimCtrl.SpineAnimState.Run);
            }
            else if (vt == SpineAnimCtrl.SpineAnimState.Idle)
            {
                animator.SetAnimState(stack.Count > 0 ? SpineAnimCtrl.SpineAnimState.PickIdle : SpineAnimCtrl.SpineAnimState.Idle);
            }
            else
            {
                animator.SetAnimState(vt);
            }
        }
    }
    public void SetCurTaskId(int productid)
    {
        taskproductId = productid;
    }
    public void SetCurTaskNum(int num)
    {
        taskproductNum = num;
    }

    public bool MoveTo(Vector3 toPos)
    {
        Vector3 rawx = (Quaternion.Euler(0, -45, 0)*(toPos - transform.position)).normalized;
        agent.MoveTo(toPos);
        bool canmove = agent.IsMoving();
        if (canmove)
        {
            SetSpineAnimFlip(rawx.z);
            SetSpineAnimState(SpineAnimCtrl.SpineAnimState.Run);

        }
        return canmove;
    }

    public void StopMove()
    {
        if (agent != null)
        {
            agent.StopMove();
        }
        SetSpineAnimState(SpineAnimCtrl.SpineAnimState.Idle);
    }

    public bool IsMoving()
    {
        if (agent != null)
        {
            return agent.IsMoving();
        }
        return false;
    }
    public bool IsProductWorker()
    {
        return Profile.Instance.IsProductWorker((int)actorType);
    }

}

