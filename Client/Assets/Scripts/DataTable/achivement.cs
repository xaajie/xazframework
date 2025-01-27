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
    public class achivement
    {
        /** id**/
        [XmlAttribute("id")]
        public int id;

        /** 组id**/
        [XmlAttribute("groupId")]
        public int groupId;

        /** 成就类型
新增类型需告知程序**/
        [XmlAttribute("aType")]
        public int aType;

        /** 成就显示标题**/
        [XmlAttribute("name")]
        public string name;

        /** 成就描述**/
        [XmlAttribute("desc")]
        public string desc;

        /** 成就参数
（末位是分母显示值，非计数型需补1）**/
        [XmlAttribute("param")]
        public List<string> param
        {
            get
            {
                if (_param != null)
                {
                    return _param.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("param")]
        public stringArray _param;

        /** 成就图标**/
        [XmlAttribute("icon")]
        public string icon;

        /** 图标图集**/
        [XmlAttribute("atlas")]
        public string atlas;

        /** 排序
小靠前**/
        [XmlAttribute("sort")]
        public int sort;

        /** 完成奖励**/
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

        /** 完成奖励广告领取**/
        [XmlAttribute("adAward")]
        public List<string> adAward
        {
            get
            {
                if (_adAward != null)
                {
                    return _adAward.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("adAward")]
        public stringArray _adAward;

        public static List<achivement> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/achivement");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsachivement table = binaryFormatter.Deserialize(mStream) as allsachivement;
                    return table.achivements;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsachivement
    {
        public List<achivement> achivements;
    }
}
