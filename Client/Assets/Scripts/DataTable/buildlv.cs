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
    public class buildlv
    {
        /** 自增id**/
        [XmlAttribute("id")]
        public int id;

        /** 功能建筑id**/
        [XmlAttribute("buildId")]
        public int buildId;

        /** 等级**/
        [XmlAttribute("level")]
        public int level;

        /** 阶段**/
        [XmlAttribute("step")]
        public int step;

        /** 功能建筑等级名称**/
        [XmlAttribute("name")]
        public string name;

        /** 模型贴图
atlas/Scene/**/
        [XmlAttribute("modelImg")]
        public string modelImg;

        /** ICON**/
        [XmlAttribute("icon")]
        public string icon;

        /** 图集名称**/
        [XmlAttribute("atlas")]
        public string atlas;

        /** buildType=1
产出速度x/毫秒
buildType=3
结算速度x/毫秒
**/
        [XmlAttribute("speed")]
        public int speed;

        /** 耐久
x毫秒之后维修**/
        [XmlAttribute("fixcd")]
        public int fixcd;

        /** 生产cd
毫秒**/
        [XmlAttribute("productcd")]
        public int productcd;

        /** 原材料category
（产品id_数量;产品id_数量）**/
        [XmlAttribute("rawitem")]
        public List<string> rawitem
        {
            get
            {
                if (_rawitem != null)
                {
                    return _rawitem.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("rawitem")]
        public stringArray _rawitem;

        /** 产物id
**/
        [XmlAttribute("productItem")]
        public int productItem;

        /** 绑定产物售价**/
        [XmlAttribute("productSellPrice")]
        public int productSellPrice;

        /** 原材料能放置的最大容量**/
        [XmlAttribute("rawlimitnum")]
        public List<int> rawlimitnum
        {
            get
            {
                if (_rawlimitnum != null)
                {
                    return _rawlimitnum.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("rawlimitnum")]
        public intArray _rawlimitnum;

        /** 放置产物
最大容量**/
        [XmlAttribute("limitnum")]
        public int limitnum;

        /** 升级需看广告**/
        [XmlAttribute("costAd")]
        public bool costAd;

        /** 升级消耗
（大类category）
3鱼，2钱**/
        [XmlAttribute("cost")]
        public List<string> cost
        {
            get
            {
                if (_cost != null)
                {
                    return _cost.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("cost")]
        public stringArray _cost;

        /** 升级奖励**/
        [XmlAttribute("award")]
        public List<string> award
        {
            get
            {
                if (_award != null)
                {
                    return _award.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("award")]
        public stringArray _award;

        /** 升级属性变动类型
（属性表id）**/
        [XmlAttribute("lvupAttr")]
        public List<int> lvupAttr
        {
            get
            {
                if (_lvupAttr != null)
                {
                    return _lvupAttr.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("lvupAttr")]
        public intArray _lvupAttr;

        /** 是否检测突发事件标
**/
        [XmlAttribute("checknotice")]
        public bool checknotice;

        public static List<buildlv> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/buildlv");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsbuildlv table = binaryFormatter.Deserialize(mStream) as allsbuildlv;
                    return table.buildlvs;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsbuildlv
    {
        public List<buildlv> buildlvs;
    }
}
