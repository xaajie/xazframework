//【DataTool】Auto Generate code
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

namespace Table
{
    [Serializable]
    public class shop
    {
        /** id**/
        [XmlAttribute("id")]
        public int id;

        /** 组id**/
        [XmlAttribute("shopGroup")]
        public int shopGroup;

        /** 商品名称
**/
        [XmlAttribute("name")]
        public string name;

        /** 商品图标
空：用购买物信息**/
        [XmlAttribute("icon")]
        public string icon;

        /** 商品图集**/
        [XmlAttribute("atlas")]
        public string atlas;

        /** 购买的物品类型
const.category**/
        [XmlAttribute("itemType")]
        public int itemType;

        /** 购买的物品ID**/
        [XmlAttribute("itemId")]
        public int itemId;

        /** 获得的物品
数量**/
        [XmlAttribute("itemNum")]
        public int itemNum;

        /** 
限制购买
类型;次数
1.终身次数
2.每日次数
3 不限量
**/
        [XmlAttribute("limitNum")]
        public List<int> limitNum
        {
            get
            {
                if (_limitNum != null)
                {
                    return _limitNum.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("limitNum")]
        public intArray _limitNum;

        /** 购买类型
1广告
2货币
0无消耗**/
        [XmlAttribute("buyType")]
        public int buyType;

        /** 购买所需物品
原价**/
        [XmlAttribute("cost")]
        public string cost;

        /** 广告购买刷新cd
(秒)**/
        [XmlAttribute("adBuyCD")]
        public int adBuyCD;

        /** 显示优先级
越小越靠前**/
        [XmlAttribute("sort")]
        public int sort;

        public static List<shop> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/shop");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsshop table = binaryFormatter.Deserialize(mStream) as allsshop;
                    return table.shops;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsshop
    {
        public List<shop> shops;
    }
}
