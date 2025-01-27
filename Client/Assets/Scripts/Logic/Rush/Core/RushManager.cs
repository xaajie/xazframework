using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Table;
using UnityEngine;
public class RushManager :MonoBehaviour
{
    public static RushManager Instance=null;
    [Header("Model")]
    [SerializeField] public PlayerCtrl mainplayer;
    [SerializeField] public Transform objNode;
    [SerializeField] private BuyLandController buyLandres;
    [SerializeField] public TrashBin trashBin;

    [Header("位置配置")]
    [SerializeField] public ScenePosHelp scenePos;

    private UserChallengeShowData curChallenge;

    float spawnTimer = 32;
    float spawnInterval = 30;
    int buildIdt = 0;
    int actoridt = 100;
    [HideInInspector] public List<CustomerCtrl> customers = new List<CustomerCtrl>();
    [HideInInspector] public List<BuyLandController> unlockLands = new List<BuyLandController>();
    [HideInInspector] public List<BuildController> builds = new List<BuildController>();
    [HideInInspector] public List<WorkerCtrl> workers = new List<WorkerCtrl>();
    public bool canUpdate = false;
    int sceneLayerId;
    int actorLayerId;
    private WorkerCtrl _guard;
    private CashBuild _cashBuid;

    void Start()
    {
        Instance = this;
        sceneLayerId = LayerMask.NameToLayer(XazConfig.LayerDefine.SceneObj);
        actorLayerId = LayerMask.NameToLayer(XazConfig.LayerDefine.SceneActor);

    }

    public void EndRush()
    {
        canUpdate = false;
        Destroy(mainplayer.gameObject);
        for (int i = 0; i < customers.Count; i++)
        {
            Destroy(customers[i].gameObject);
        }
        for (int i = 0; i < unlockLands.Count; i++)
        {
            Destroy(unlockLands[i].gameObject);
        }
        for (int i = 0; i < builds.Count; i++)
        {
            Destroy(builds[i].gameObject);
        }
        for (int i = 0; i < workers.Count; i++)
        {
            Destroy(workers[i].gameObject);
        }
        customers.Clear();
        customers = null;
        unlockLands.Clear();
        unlockLands = null;
        builds.Clear();
        builds = null;
        workers.Clear();
        workers = null;
        Instance = null;
    }
    public void StartRush()
    {
        canUpdate = false;
        if (ModuleMgr.ChallengeMgr != null)
        {
            curChallenge = ModuleMgr.ChallengeMgr.GetCurChallege();
            spawnInterval = curChallenge.GenerNextCustormerCd();
            //创建主角
            UserScenePlayerData pt = new UserScenePlayerData();
            pt.SetData(curChallenge.GetUserInfo().player);
            pt.RefreshData();
            mainplayer.SetCtrlData(pt);
            CameraMgr.Instance.SetFollowCam(CameraController.CameraMode.MAIN, mainplayer.transform,false);
            StartCoroutine(HandleCustomerSpawn());
        }
        //初始化建筑
        if (curChallenge != null)
        {
            GenerSceneUnlocks();
            GenerBuilds();
            GenerWorkers();
            ModuleMgr.NoviceMgr.CheckStartNovice();
        }
        // SaveSystem.SaveData<int>(SceneManager.GetActiveScene().buildIndex, "LastSceneIndex");
        //  screenFader.FadeOut();
        //AudioManager.Instance.PlayBGM(backgroundMusic);
        canUpdate = true;
        //创建店员
        //for (int i = 0; i < data.EmployeeAmount; i++) SpawnEmployee();
    }

    void Update()
    {
        if (canUpdate)
        {
            if (waveFin)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= spawnInterval && customers.Count < curChallenge.GetMaxCustomerNum())
                {
                    spawnTimer = 0f;
                    StartCoroutine(HandleCustomerSpawn());
                }
            }
        }
    }


    public void AddDelFloating(object vt, bool isAdd)
    {
        UIMain tar = UIMgr.Get<UIMain>();
        if (tar != null)
        {
            if (isAdd)
            {
                tar.AddFloating(vt);
            }
            else
            {
                tar.RemoveFloating(vt);
            }
        }
    }

    public WorkerCtrl GetGuard()
    {
        if (_guard == null)
        {
            _guard = ModuleMgr.FightMgr.GetActorByType(Const.ActorType.Guard);
        }
        return _guard;
    }

    public CashBuild GetCashBuild()
    {
        if (_cashBuid == null)
        {
            _cashBuid = ModuleMgr.FightMgr.GetBuildByBuildType(Const.BuildType.CashDesk) as CashBuild;
        }
        return _cashBuid;
    }
    public void RefreshFloating(object vt)
    {
        UIMain tar = UIMgr.Get<UIMain>();
        if (tar != null)
        {
            tar.RefreshFloating(vt);
        }
    }

    public void GenerSceneUnlocks()
    {
        List<UserSceneUnlockDataBase> unlocks = curChallenge.GetUserInfo().unlocks;
        for (int i = 0; i < unlocks.Count; i++)
        {
            GenerOneBuyer(unlocks[i]);
        }
    }

    public void GenerBuilds()
    {
        List<UserSceneBuildDataBase> builds = curChallenge.GetUserInfo().builds;
        for (int i = 0; i < builds.Count; i++)
        {
            GenerOneBuild(builds[i]);
        }
    }

    public void GenerWorkers()
    {
        List<UserSceneWorkerDataBase> builds = curChallenge.GetUserInfo().workers;
        if (builds != null)
        {
            for (int i = 0; i < builds.Count; i++)
            {
                GenerOneWorker(builds[i], Vector3.zero);
            }
        }
    }

    public void GenerOneBuyer(UserSceneUnlockDataBase rawdat)
    {
        UserSceneUnlockData dat = new UserSceneUnlockData();
        dat.SetData(rawdat);
        dat.RefreshData();
        if (dat.GetBuyPointId()>=0)
        {
            BuyLandController obj = Instantiate(buyLandres);
            obj.transform.position = GetScenePoint(dat.GetBuyPointId());

            //obj.transform.position = MathUtil.GetRandomPointInCircle(GetScenePoint(dat.GetBuyPointId()), 1);
            RayUtil.SetLayerById(obj.gameObject, sceneLayerId);
            obj.transform.SetParent(objNode);
            obj.SetCtrlData(dat);
            unlockLands.Add(obj);
        }
    }
    public UserSceneWorkerData GenerOneWorker(UserSceneWorkerDataBase rawdat,Vector3 fixpos)
    {
        actoridt++;
        rawdat.uid = actoridt;
        UserSceneWorkerData dat = new UserSceneWorkerData();
        dat.SetData(rawdat);
        dat.RefreshData();
        Action<UnityEngine.Object> LoadAction = (asset) =>
        {
            GameObject obj = GameObject.Instantiate(asset) as GameObject;
            if (obj != null)
            {
                //    var randomCircle = UnityEngine.Random.insideUnitCircle * employeeSpawnRadius;
                //    randomPos = employeePoint.position + new Vector3(randomCircle.x, 0, randomCircle.y);
                //}
                obj.SetActive(false);
                RayUtil.SetLayerById(obj, actorLayerId);
                if (fixpos == Vector3.zero)
                {
                    obj.transform.position = GetScenePoint(dat.bindposId);
                }
                else
                {
                    obj.transform.position = fixpos;
                }
                obj.SetActive(false);
                RayUtil.SetLayerById(obj, actorLayerId);
                WorkerCtrl worker = obj.GetComponent<WorkerCtrl>();
                if (worker == null)
                {
                    worker = obj.AddComponent<WorkerCtrl>();
                }
                obj.transform.SetParent(objNode);
                obj.transform.localRotation = Quaternion.identity;
                worker.SetCtrlData(dat);
                obj.SetActive(true);
                workers.Add(worker);
                if (worker.GetCtrlData().CanShowFloating())
                {
                    AddDelFloating(worker, true);
                }
            }
        };
        GetModelById(dat.GetModelId(), LoadAction);
        return dat;
    }

    public UserSceneBuildData GenerOneBuild(UserSceneBuildDataBase rawdat)
    {
        rawdat.uid = buildIdt++;
        UserSceneBuildData dat = new UserSceneBuildData();
        dat.SetData(rawdat);
        dat.RefreshData();
        Action<UnityEngine.Object> LoadAction = (asset) =>
        {
            GameObject unlocktar = asset as GameObject;
            if (unlocktar != null)
            {
                GameObject obj = Instantiate(unlocktar);
                obj.gameObject.SetActive(true);
                obj.transform.position = GetScenePoint(rawdat.bindposId);
                BuildController build = obj.GetComponent<BuildController>();
                RayUtil.SetLayerById(obj.gameObject, sceneLayerId);
                obj.transform.SetParent(objNode);
                build.SetCtrlData(dat);
                builds.Add(build);
                if (build.CanShowFloating())
                {
                    AddDelFloating(build, true);
                }
            }
        };
        GetModelById(dat.GetInfo().GetModelId(), LoadAction);
        return dat;
    }

    public Vector3 GetScenePoint(int id)
    {
        if(id< scenePos.points.Count)
        {
            return scenePos.points[id];
        }
        else
        {
            Logger.Print("error:位置点配置不足",id);
            return Vector3.zero;
        }
    }

    public void GetModelById(int id, Action<UnityEngine.Object> LoadAction)
    {
        model info;
        StaticDataMgr.Instance.modelInfo.TryGetValue(id, out info);
        if (info != null)
        {
            string url = StaticDataMgr.Instance.GetModelPath(id);
            ResMgr.LoadAssetAsync(url, typeof(GameObject), LoadAction);
        }
    }

    bool waveFin = true;
    IEnumerator HandleCustomerSpawn()
    {
        waveFin = false;
        if (ModuleMgr.FightMgr.GetBuildByBuildType(Const.BuildType.CashDesk) != null)
        {
            int curNum = curChallenge.GetCurCustormerNum();
            int gennum = Math.Min(curChallenge.GetMaxCustomerNum() - customers.Count, curNum);
            bool isFirstWave = customers.Count == 0;
            for (int i = 0; i < gennum; i++)
            {
                int orderId = ModuleMgr.FightMgr.GetNewOrderId(curChallenge.GetPoolGroupId());
                if (orderId <= 0)
                {
                    break;
                }
                yield return new WaitForSeconds((i+1) * 0.2f);
                actoridt++;
                UserCustomerOrderData order = new UserCustomerOrderData(orderId, actoridt);
                Action<UnityEngine.Object> LoadAction = (asset) =>
                {
                    GameObject obj = GameObject.Instantiate(asset) as GameObject;
                    if (obj != null)
                    {
                        obj.SetActive(false);
                        RayUtil.SetLayerById(obj, actorLayerId);
                        CustomerCtrl newCustomer = obj.GetComponent<CustomerCtrl>();
                        if (newCustomer == null)
                        {
                            newCustomer = obj.AddComponent<CustomerCtrl>();
                        }
                        obj.transform.SetParent(objNode);
                        if (isFirstWave)
                        {
                            obj.transform.position = scenePos.spawnPoint0;
                        }
                        else
                        {
                            obj.transform.position = scenePos.spawnPoint;
                        }
                        obj.transform.localRotation = Quaternion.identity;
                        newCustomer.SetCtrlData(order);
                        obj.SetActive(true);
                        customers.Add(newCustomer);
                        AddDelFloating(newCustomer, true);
                    }
                };
                GetModelById(order.GetGuestModelId(), LoadAction);
            }
            spawnInterval = curChallenge.GenerNextCustormerCd();
        }
        waveFin = true;
    }

    public void DelCustomer(CustomerCtrl pt)
    {
        AddDelFloating(pt, false);
        customers.Remove(pt);
        Destroy(pt.gameObject);
    }
    public void DelWorker(WorkerCtrl pt)
    {
        workers.Remove(pt);
        Destroy(pt.gameObject);
    }

    public void DelBuild(BuildController pt)
    {
        if (pt.CanShowFloating())
        {
            AddDelFloating(pt, false);
        }
        builds.Remove(pt);
        Destroy(pt.gameObject);
    }

    public void BuyUnlockable(BuyLandController info)
    {
        AudioMgr.Instance.Play(AudioEnum.lvup);
        info.gameObject.SetActive(false);
        StartCoroutine(BuyUnlockableNext(info));
    }

    IEnumerator BuyUnlockableNext(BuyLandController info)
    {
        UserSceneUnlockData unlockInfo = info.curUnlockDat;
        List<UserCategoryData> unlocktar = unlockInfo.GetUnlockTargets();
        UserSceneWorkerData foucusWorker=null;
        for (int i = 0; i < unlocktar.Count; i++)
        {
            if (unlocktar[i].itemType == Const.Category.BUILD)
            {
                UserSceneBuildDataBase newdat = new UserSceneBuildDataBase();
                newdat.id = unlocktar[i].GetID();
                newdat.bindposId = unlockInfo.GetCreatePointId(i);
                newdat.level = unlockInfo.GetInitBuildlv();
                newdat.ownId = curChallenge.GetID();
                curChallenge.GetUserInfo().builds.Add(newdat);
                UserSceneBuildData newinfo = GenerOneBuild(newdat);
                EffectMgr.Instance.PlayEffectPos("UnlockParticle", GetScenePoint(newdat.bindposId), objNode.transform);
                ModuleMgr.AchivementMgr.UpdateAchivement(Const.AchivementType.UnlockupBuild, new int[] { newinfo.GetBuildID(), 1 });
                ModuleMgr.AchivementMgr.UpdateAchivement(Const.AchivementType.UnlockupBuildType, new int[] { newinfo.GetBuildType() });
            }
            else if (unlocktar[i].itemType == Const.Category.ACTOR)
            {
                UserSceneWorkerDataBase workds = new UserSceneWorkerDataBase();
                workds.id = unlocktar[i].GetID();
                workds.bindposId = unlockInfo.GetCreatePointId(i);
                workds.ownId = curChallenge.GetID();
                curChallenge.GetUserInfo().workers.Add(workds);
                foucusWorker = GenerOneWorker(workds, Vector3.zero);
                UIMgr.Open<UIActorDesc>(uiview => uiview.SetData(foucusWorker));
                EffectMgr.Instance.PlayEffectPos("MagicPillarBlastBlue", GetScenePoint(workds.bindposId), objNode.transform);
                ModuleMgr.AchivementMgr.UpdateAchivement(Const.AchivementType.CountActor, new int[] { foucusWorker.GetActorType() });
            }
        }
        string effetLoopkey = string.Empty;
        if (foucusWorker != null)
        {
            WorkerCtrl actor = null; 
            while (actor == null)
            {
                actor = ModuleMgr.FightMgr.GetActorByUid(foucusWorker.uid);
                if (actor != null && string.IsNullOrEmpty(effetLoopkey))
                {
                    effetLoopkey = EffectMgr.Instance.PlayEffect("movedi", actor.transform, false);
                    CameraMgr.Instance.SetFollowCam(CameraController.CameraMode.FOUCUS, actor.transform, false);
                }
                yield return new WaitForEndOfFrame();
            }
        }
        curChallenge.DelUnlockData(info.curUnlockDat.id);
        if (unlockInfo.GetOpenLevel() > 0)
        {
            curChallenge.GetUserInfo().level = unlockInfo.GetOpenLevel();
        }
        for (int i = 0; i < unlockLands.Count; i++)
        {
            if (unlockLands[i] == info)
            {
                Destroy(unlockLands[i]);
                unlockLands.RemoveAt(i);
                break;
            }
        }
        EventMgr.DispatchEvent(EventEnum.ChallengeInfo_REFRESH);
        int nextUnlockSort = unlockInfo.GetNextUnlockIndex();
        if (unlockLands.Count <= 0)
        {
            if (nextUnlockSort > 0)
            {
                curChallenge.GenerUnlockData(nextUnlockSort);
                GenerSceneUnlocks();
            }
        }
        while (UIMgr.Get<UIActorDesc>()!=null)
        {
            yield return new WaitForEndOfFrame();
        }
        if (!string.IsNullOrEmpty(effetLoopkey))
        {
            EffectMgr.Instance.DelEffect(effetLoopkey);
        }
        if (unlockLands.Count > 0 && !unlockInfo.GetUnlockInfo().noviceNocam)
        {
            CameraMgr.Instance.SetFollowCam(CameraController.CameraMode.FOUCUS, unlockLands[0].transform, false);
            yield return new WaitForSeconds(Const.CamerFocusTime);
        }
        curChallenge.GenerHappenInfo(unlockInfo.GetHappenId());
        foucusWorker = null;
        CameraMgr.Instance.ResetCam();
    }

    public void AdjustMoney(int change)
    {
        if (change > 0)
        {
            ModuleMgr.AwardMgr.ChangeCurrency((int)Const.CurrencyType.GOLD, change, true);
        }
        else
        {
            ModuleMgr.AwardMgr.ChangeCurrency((int)Const.CurrencyType.GOLD, -change, false);
        }
    }

    public BuyLandController GetUnlockById(int unlockId)
    {
        for (int i = 0; i < unlockLands.Count; i++)
        {
            if (unlockLands[i].curUnlockDat.id == unlockId)
            {
                return unlockLands[i];
            }
        }
        return null;
    }
    public BuildController GetBuildById(int buildId)
    {
        for (int i = 0; i < builds.Count; i++)
        {
            BuildController build = builds[i];
            if (build.GetCtrlData().GetBuildID() == buildId)
            {
                return build;
            }
        }
        return null;
    }

    void OnDisable()
    {
        DOTween.KillAll();
    }

}


