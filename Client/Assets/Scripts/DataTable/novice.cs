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
    public class novice
    {
        /** 唯一id**/
        [XmlAttribute("id")]
        public int id;

        /** 下一个新手id**/
        [XmlAttribute("nextId")]
        public int nextId;

        /** 1 UI引导
2 未解锁引导
3 摇杆引导
4 建筑引导**/
        [XmlAttribute("noviceType")]
        public int noviceType;

        /** 引导提示语**/
        [XmlAttribute("desc")]
        public string desc;

        /** 完成条件
1.时间（毫秒）
2.目标点击
3.站到目的地
4.建筑（建筑id）**/
        [XmlAttribute("finishCon")]
        public List<int> finishCon
        {
            get
            {
                if (_finishCon != null)
                {
                    return _finishCon.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("finishCon")]
        public intArray _finishCon;

        /** **/
        [XmlAttribute("finishConAttr")]
        public List<string> finishConAttr
        {
            get
            {
                if (_finishConAttr != null)
                {
                    return _finishConAttr.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("finishConAttr")]
        public stringArray _finishConAttr;

        /** 毫秒**/
        [XmlAttribute("preDelay")]
        public int preDelay;

        public static List<novice> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/novice");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsnovice table = binaryFormatter.Deserialize(mStream) as allsnovice;
                    return table.novices;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsnovice
    {
        public List<novice> novices;
    }
}
