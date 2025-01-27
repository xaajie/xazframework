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
    public class order
    {
        /** 自增id**/
        [XmlAttribute("id")]
        public int id;

        /** 订单池子id**/
        [XmlAttribute("poolId")]
        public int poolId;

        /** 订单类型**/
        [XmlAttribute("ordertype")]
        public int ordertype;

        /** 权重**/
        [XmlAttribute("ratio")]
        public int ratio;

        /** 订单货物ID
(id_权重)**/
        [XmlAttribute("needIem")]
        public List<string> needIem
        {
            get
            {
                if (_needIem != null)
                {
                    return _needIem.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("needIem")]
        public stringArray _needIem;

        /** 订单货物数目**/
        [XmlAttribute("neednum")]
        public int neednum;

        /** 顾客形象
actor表id**/
        [XmlAttribute("guestId")]
        public int guestId;

        /** 结算后的表情图
{非常满意图;一般;不想评价}**/
        [XmlAttribute("face")]
        public List<string> face
        {
            get
            {
                if (_face != null)
                {
                    return _face.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("face")]
        public stringArray _face;

        public static List<order> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/order");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsorder table = binaryFormatter.Deserialize(mStream) as allsorder;
                    return table.orders;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsorder
    {
        public List<order> orders;
    }
}
