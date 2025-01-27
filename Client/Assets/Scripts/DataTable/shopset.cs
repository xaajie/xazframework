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
    public class shopset
    {
        /** id**/
        [XmlAttribute("id")]
        public int id;

        /** 商店名称**/
        [XmlAttribute("name")]
        public string name;

        /** 商店分类
1普通商店
**/
        [XmlAttribute("shopType")]
        public int shopType;

        /** 商品组id**/
        [XmlAttribute("shopGroupId")]
        public List<int> shopGroupId
        {
            get
            {
                if (_shopGroupId != null)
                {
                    return _shopGroupId.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("shopGroupId")]
        public intArray _shopGroupId;

        /** 功能废置字段
选组策略**/
        [XmlAttribute("selectType")]
        public int selectType;

        /** 每日刷新次数
功能废置字段**/
        [XmlAttribute("freeRefreshTimes")]
        public int freeRefreshTimes;

        /** 刷新CD周期
功能废置字段
生成：https://tool.lu/crontab/ai.html
说明：https://tool.lu/crontab/**/
        [XmlAttribute("cron")]
        public string cron;

        public static List<shopset> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/shopset");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsshopset table = binaryFormatter.Deserialize(mStream) as allsshopset;
                    return table.shopsets;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsshopset
    {
        public List<shopset> shopsets;
    }
}
