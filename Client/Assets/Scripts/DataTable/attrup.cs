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
    public class attrup
    {
        /** 自增id**/
        [XmlAttribute("id")]
        public int id;

        /** buffid
（buff表）**/
        [XmlAttribute("buffId")]
        public int buffId;

        /** 分组**/
        [XmlAttribute("groupId")]
        public int groupId;

        /** 描述
**/
        [XmlAttribute("desc")]
        public string desc;

        /** 等级
（初始默认0级，角色初始属性值在角色表，从1级填变更值）
**/
        [XmlAttribute("level")]
        public int level;

        /** 升级需看广告**/
        [XmlAttribute("costAd")]
        public bool costAd;

        /** 升级到该级别所需消耗（大类category）
3鱼，2钱**/
        [XmlAttribute("cost")]
        public string cost;

        /** ICON
(空用属性本身图标)**/
        [XmlAttribute("icon")]
        public string icon;

        /** 图集名称
(空用属性本身图集)**/
        [XmlAttribute("atlas")]
        public string atlas;

        /** 排序字段
（只对groupid生效）**/
        [XmlAttribute("sort")]
        public int sort;

        public static List<attrup> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/attrup");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsattrup table = binaryFormatter.Deserialize(mStream) as allsattrup;
                    return table.attrups;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsattrup
    {
        public List<attrup> attrups;
    }
}
