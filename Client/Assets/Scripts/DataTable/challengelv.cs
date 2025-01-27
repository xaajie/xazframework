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
    public class challengelv
    {
        /** 等级**/
        [XmlAttribute("id")]
        public int id;

        /** 星级**/
        [XmlAttribute("star")]
        public int star;

        /** 每波顾客cd
随机范围(秒)
左边包含，右边不包含**/
        [XmlAttribute("customercd")]
        public List<int> customercd
        {
            get
            {
                if (_customercd != null)
                {
                    return _customercd.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("customercd")]
        public intArray _customercd;

        /** 每波顾客个数
随机左边包含，右边不包含**/
        [XmlAttribute("customerpernum")]
        public List<int> customerpernum
        {
            get
            {
                if (_customerpernum != null)
                {
                    return _customerpernum.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("customerpernum")]
        public intArray _customerpernum;

        /** 顾客最多同屏个数
（程序优化测试）**/
        [XmlAttribute("maxCustomer")]
        public int maxCustomer;

        /** 订单池子id**/
        [XmlAttribute("orderpool")]
        public int orderpool;

        public static List<challengelv> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/challengelv");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allschallengelv table = binaryFormatter.Deserialize(mStream) as allschallengelv;
                    return table.challengelvs;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allschallengelv
    {
        public List<challengelv> challengelvs;
    }
}
