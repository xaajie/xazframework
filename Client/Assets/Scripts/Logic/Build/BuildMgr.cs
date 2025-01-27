//----------------------------------------------------------------------------
//-- BuildMgr模块管理
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using Table;

public class BuildMgr
{
    public Dictionary<int, UserBuildvInfoData> buildlvInfos = new Dictionary<int, UserBuildvInfoData>();
    private Dictionary<int, int> buildidstep_maxlv = new Dictionary<int, int>();
    private Dictionary<int ,int> build_maxlv = new Dictionary<int, int>();

    public BuildMgr()
    {
        foreach (buildlv cha in StaticDataMgr.Instance.buildlvInfo.Values)
        {
            int key = GetKey(cha.buildId, cha.level);
            UserBuildvInfoData info = new UserBuildvInfoData(cha.buildId, cha.level, cha);
            buildlvInfos.Add(key, info);
            int keystep = GetKey(cha.buildId, cha.step);
            if (buildidstep_maxlv.ContainsKey(keystep))
            {
                if (buildidstep_maxlv[keystep] < cha.level)
                {
                    buildidstep_maxlv[keystep] = cha.level;
                }
            }
            else
            {
                buildidstep_maxlv.Add(keystep, cha.level);
            }
            if (build_maxlv.ContainsKey(cha.buildId))
            {
                if (build_maxlv[cha.buildId] < cha.level)
                {
                    build_maxlv[cha.buildId] = cha.level;
                }
            }
            else
            {
                build_maxlv.Add(cha.buildId, cha.level);
            }
        }
    }

    private static int keyRatio = 1000;
    public int GetKey(int id, int lv)
    {
        return id * keyRatio + lv;
    }

    public UserBuildvInfoData GetBuildlvInfo(int id, int lv)
    {
        int key = GetKey(id, lv);
        if (buildlvInfos.ContainsKey(key))
        {
            return buildlvInfos[key];
        }
        return null;
    }

    public int GetBuildMaxLv(int key)
    {
        if (build_maxlv.ContainsKey(key))
        {
            return build_maxlv[key];
        }
        return 0;
    }
    public int GetStepmaxLv(int id, int step)
    {
        int key = GetKey(id, step);
        if (buildidstep_maxlv.ContainsKey(key))
        {
            return buildidstep_maxlv[key];
        }
        return 0;

    }
    public List<UserSceneBuildData> GetShowLvList()
    {
        List<UserSceneBuildData> uilds = new List<UserSceneBuildData>();
        List<int> insertId = new List<int>();
        for (int i = 0; i < RushManager.Instance.builds.Count; i++)
        {
            UserSceneBuildData info = RushManager.Instance.builds[i].GetCtrlData();
            if (info.GetInfo().IsCanShowLvup() && insertId.IndexOf(info.GetBuildID()) == -1)
            {
                insertId.Add(info.GetBuildID());
                uilds.Add(info);
            }
        }
        //uilds.Sort((a, b) => a.GetInfo().GetSort().CompareTo(b.GetInfo().GetSort()));
        //   uilds.Sort()
        return uilds;
    }

    public void Release()
    {
        buildlvInfos.Clear();
    }

}
