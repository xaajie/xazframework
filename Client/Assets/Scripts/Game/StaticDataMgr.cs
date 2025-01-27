//------------------------------------------------------------
// Xaz Framework
// 静态表管理器
// Feedback: qq515688254
//------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Table;
using UnityEngine;

public class StaticDataMgr : Singleton<StaticDataMgr>
{

    public Dictionary<object, challenge> challengeInfo;
    public Dictionary<object, mainshow> mainshowInfo;
    public Dictionary<object, item> itemInfo;
    public Dictionary<object, currency> currencyInfo;
    public Dictionary<object, actor> actorInfo;
    public Dictionary<object, build> buildInfo;
    public Dictionary<object, buildlv> buildlvInfo;
    public Dictionary<object, product> productInfo;
    public Dictionary<object, productlv> productlvInfo;
    public Dictionary<object, level> levelInfo;
    public Dictionary<object, order> orderInfo;
    public Dictionary<object, gmorder> gmorderInfo;
    public Dictionary<object, sceneunlock> sceneunlockInfo;
    public Dictionary<object, audio> audioInfo;
    public Dictionary<object, model> modelInfo;
    public Dictionary<object, attr> attrInfo;
    public Dictionary<object, attrup> attrupInfo;
    public Dictionary<object, goldsupply> goldsupplyInfo;
    public Dictionary<object, shop> shopInfo;
    public Dictionary<object, shopset> shopsetInfo;
    public Dictionary<object, achivement> achivementInfo;
    public Dictionary<object, buff> buffinfo;
    public Dictionary<object, happening> happeningInfo;
    public Dictionary<object, novice> noviceInfo;
    public Dictionary<object, challengelv> challengelvInfo;
    public IEnumerator Init()
    {
        goldsupplyInfo = Data.ListToDic<goldsupply>(goldsupply.LoadBytes(), "id");
        audioInfo = Data.ListToDic<audio>(audio.LoadBytes(), "id");
        modelInfo = Data.ListToDic<model>(model.LoadBytes(), "id");
        gmorderInfo = Data.ListToDic<gmorder>(gmorder.LoadBytes(), "id");
        attrInfo = Data.ListToDic<attr>(attr.LoadBytes(), "id");
        attrupInfo = Data.ListToDic<attrup>(attrup.LoadBytes(), "id");
        challengeInfo = Data.ListToDic<challenge>(challenge.LoadBytes(), "id");
        yield return XazHelper.waitFrame;
        mainshowInfo = Data.ListToDic<mainshow>(mainshow.LoadBytes(), "id");

        itemInfo = Data.ListToDic<item>(item.LoadBytes(), "id");

        currencyInfo = Data.ListToDic<currency>(currency.LoadBytes(), "id");
        achivementInfo = Data.ListToDic<achivement>(achivement.LoadBytes(), "id");
        yield return XazHelper.waitFrame;
        actorInfo = Data.ListToDic<actor>(actor.LoadBytes(), "id");
        levelInfo = Data.ListToDic<level>(level.LoadBytes(), "id");

        orderInfo = Data.ListToDic<order>(order.LoadBytes(), "id");

        buildInfo = Data.ListToDic<build>(build.LoadBytes(), "id");
        buildlvInfo = Data.ListToDic<buildlv>(buildlv.LoadBytes(), "id");

        productInfo = Data.ListToDic<product>(product.LoadBytes(), "id");
        productlvInfo = Data.ListToDic<productlv>(productlv.LoadBytes(), "id");
        yield return XazHelper.waitFrame;
        sceneunlockInfo = Data.ListToDic<sceneunlock>(sceneunlock.LoadBytes(), "id");
        shopInfo = Data.ListToDic<shop>(shop.LoadBytes(), "id");
        shopsetInfo = Data.ListToDic<shopset>(shopset.LoadBytes(), "id");
        buffinfo = Data.ListToDic<buff>(buff.LoadBytes(), "id");
        happeningInfo = Data.ListToDic<happening>(happening.LoadBytes(), "id");
        noviceInfo = Data.ListToDic<novice>(novice.LoadBytes(), "id");
        challengelvInfo = Data.ListToDic<challengelv>(challengelv.LoadBytes(), "id");
    }

    public string GetModelPath(int id)
    {
        model info;
        StaticDataMgr.Instance.modelInfo.TryGetValue(id, out info);
        return info.prefabpath;
    }

    public int GetAttrTableId(int attrGroupId, int level)
    {
        foreach (attrup cha in StaticDataMgr.Instance.attrupInfo.Values)
        {
            if (cha.groupId == attrGroupId &&cha.level == level)
            {
                return cha.id;
            }
        }
        return -1;
    }
    public  void Release()
    {
        challengeInfo.Clear();
        mainshowInfo.Clear();
    }

}
