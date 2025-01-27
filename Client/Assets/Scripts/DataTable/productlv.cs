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
    public class productlv
    {
        /** 自增id**/
        [XmlAttribute("id")]
        public int id;

        /** 产物id**/
        [XmlAttribute("buildId")]
        public int buildId;

        /** 等级**/
        [XmlAttribute("level")]
        public int level;

        /** 收益价格**/
        [XmlAttribute("sellPrice")]
        public int sellPrice;

        /** 升级到该级别所需消耗
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

        /** 奖励**/
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

        /** 绑定建筑升级
建筑id_等级**/
        [XmlAttribute("bindBuild")]
        public string bindBuild;

        public static List<productlv> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/productlv");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsproductlv table = binaryFormatter.Deserialize(mStream) as allsproductlv;
                    return table.productlvs;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsproductlv
    {
        public List<productlv> productlvs;
    }
}
