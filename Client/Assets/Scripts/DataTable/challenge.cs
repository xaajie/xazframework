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
    public class challenge
    {
        /** 编号**/
        [XmlAttribute("id")]
        public int id;

        /** 下一关编号**/
        [XmlAttribute("nextId")]
        public int nextId;

        /** 所属的篇章id**/
        [XmlAttribute("chapterId")]
        public int chapterId;

        /** 名字**/
        [XmlAttribute("name")]
        public string name;

        /** 图片名
Resource/UI/Pic目录**/
        [XmlAttribute("img")]
        public string img;

        /** 关卡文件
Resources/Scene/xx**/
        [XmlAttribute("sceneName")]
        public string sceneName;

        /** 显示每张代表的钱数**/
        [XmlAttribute("permoney")]
        public int permoney;

        /** 补充金币方案
goldsupply表id**/
        [XmlAttribute("goldsupply")]
        public int goldsupply;

        /** 成就组id**/
        [XmlAttribute("agroupId")]
        public int agroupId;

        /** 主角初始配置
actor表id**/
        [XmlAttribute("playerId")]
        public int playerId;

        /** 过关奖励**/
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

        /** 开店消耗**/
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

        /** 店面解锁物件配置表
sceneunlock表的组id**/
        [XmlAttribute("unlockGroupId")]
        public int unlockGroupId;

        /** 全局属性升级配置表组id
**/
        [XmlAttribute("attrup")]
        public List<int> attrup
        {
            get
            {
                if (_attrup != null)
                {
                    return _attrup.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("attrup")]
        public intArray _attrup;

        /** 单位：元/秒**/
        [XmlAttribute("offlineAward")]
        public List<string> offlineAward
        {
            get
            {
                if (_offlineAward != null)
                {
                    return _offlineAward.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("offlineAward")]
        public stringArray _offlineAward;

        /** 描述**/
        [XmlAttribute("desc")]
        public string desc;

        public static List<challenge> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/challenge");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allschallenge table = binaryFormatter.Deserialize(mStream) as allschallenge;
                    return table.challenges;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allschallenge
    {
        public List<challenge> challenges;
    }
}
