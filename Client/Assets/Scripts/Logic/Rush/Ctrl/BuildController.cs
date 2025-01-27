using UnityEngine;
using UnityEngine.U2D;
public class BuildController : MonoBehaviour
{
    private SpineAnimCtrl animator;
    [SerializeField] public BaseStack productStack;
    [SerializeField] private Transform[] standpoint;
    private UserSceneBuildData buildDat;
    public int unlockLevel { get => buildDat.level; }
    private string rawName;

    protected virtual void Awake()
    {
        rawName = gameObject.name;
        gameObject.SetActive(false);
        animator = gameObject.GetComponent<SpineAnimCtrl>();
    }

    public Vector3 GetRawStandPosOne()
    {
        return standpoint[0].position;
    }
    public virtual Vector3 GetStandPoint()
    {
        var randomCircle = UnityEngine.Random.insideUnitCircle * 0.5f;
        if (standpoint == null || standpoint.Length <= 0)
        {
            return transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        }
        else
        {
            int index = Random.Range(0, standpoint.Length);
            return standpoint[index].position + new Vector3(randomCircle.x, 0, randomCircle.y);
        }
    }
    protected virtual void Start()
    {
    }

    protected virtual void SetSpineAnimState(SpineAnimCtrl.SpineAnimState vt)
    {
        if (animator != null)
        {
            animator.SetAnimState(vt);
        }
    }
    public void SetCtrlData(UserSceneBuildData info)
    {
        buildDat = info;
        this.transform.name = rawName + info.uid;
        SetSpineAnimState(SpineAnimCtrl.SpineAnimState.build_idle);
        gameObject.SetActive(true);
        UpdateAttr();
    }

    public bool CanShowFloating()
    {
        return buildDat.GetBuildType() == (int)Const.BuildType.Machine; //|| buildDat.GetBuildType() == (int)Const.BuildType.Shelf;
    }
    public UserSceneBuildData GetCtrlData()
    {
        return buildDat;
    }

    public Transform GetProductStatckTransform()
    {
        return productStack.transform;
    }
    public void AddProductStack(ActorController toactor)
    {
        if (productStack.Count < productStack.MaxStack)
        {
            var objToStack = toactor.Stack.RemoveFromStack(GetCtrlData().GetProductId());
            if (objToStack == null) return;

            productStack.AddToStack(objToStack.gameObject, true);
        }
    }

    public void RemoveProductStack(ActorController toactor)
    {
        if (productStack.Count > 0)
        {
            var removedObj = productStack.RemoveFromStack();
            toactor.AddHandStack(removedObj.transform, productStack.productId);
        }
    }
    public int Getuid()
    {
        return GetCtrlData().uid;
    }
    
    private SpriteRenderer _modeltar;
    private SpriteRenderer GetModel()
    {
        if (_modeltar == null)
        {
            Transform mes = transform.Find("model");
            if (mes != null)
            {
                _modeltar = mes.GetComponent<SpriteRenderer>();
                //mes[0].SetTexture("_MainTex", tex.texture);
            }
        }
        return _modeltar;
    }

    public virtual void LevelUp(bool animate = true)
    {
        GetCtrlData().RefreshData();
        UpdateAttr();
    }
    protected virtual void UpdateAttr()
    {
        if (productStack != null)
        {
            productStack.MaxStack = GetCtrlData().GetInfo().GetProductCapacity();
            productStack.productId = GetCtrlData().GetInfo().GetProductId();
        }
        if (GetModel() != null)
        {
            SpriteAtlas messprite = ResMgr.CheckGetSpriteAtlas(Const.AtlasBuild);
            GetModel().sprite = messprite.GetSprite(GetCtrlData().GetInfo().GetModelImg());
        }
    }

}

