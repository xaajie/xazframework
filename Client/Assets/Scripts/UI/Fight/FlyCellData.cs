using UnityEngine;

public class FlyCellData
{
    public Vector3 localpos;
    public UserCategoryData flyboxData;
    private Vector3 toPos;
    public bool isNeedFly = false;
    public bool isNeeddel = false;
    public bool isBefind = false;
    public int infoId;
    public int sortIdt;
    private static float offetup = 0.5f;
    private static float offetselectup = 0.6f;
    public FlyCellData(UserCategoryData content) : base()
    {
        flyboxData = new UserCategoryData((int)content.itemType, content.itemId, 1);
    }

    //Î»ÖÃÆ«ÒÆ
    public Vector3 GetToPos()
    {
        return toPos + Vector3.up * offetup * (sortIdt);
    }

    public void SetSortId(int v)
    {
        sortIdt = v;
    }
    public Vector3 GetSelectToPos()
    {
        return GetToPos() + Vector3.up * offetselectup;
    }
    public void ChangeFlyInfo(Vector3 from,Vector3 to, int infoIds, int sortId, bool isNeeddels)
    {
        localpos = from;
        toPos = to;
        infoId = infoIds;
        isNeedFly = true;
        isNeeddel = isNeeddels;
        sortIdt = sortId;
    }
}