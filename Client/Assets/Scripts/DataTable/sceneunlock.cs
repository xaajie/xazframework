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
    public class sceneunlock
    {
        /** 自增id
**/
        [XmlAttribute("id")]
        public int id;

        /** 组编号**/
        [XmlAttribute("unlockId")]
        public int unlockId;

        /** 解锁购买顺序**/
        [XmlAttribute("sort")]
        public int sort;

        /** 解锁内容**/
        [XmlAttribute("category")]
        public List<string> category
        {
            get
            {
                if (_category != null)
                {
                    return _category.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("category")]
        public stringArray _category;

        /** 解锁内容创建位置**/
        [XmlAttribute("createPos")]
        public List<int> createPos
        {
            get
            {
                if (_createPos != null)
                {
                    return _createPos.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("createPos")]
        public intArray _createPos;

        /** 钱**/
        [XmlAttribute("cost")]
        public int cost;

        /** 场景中占位点
交钱点**/
        [XmlAttribute("posId")]
        public int posId;

        /** 触发的广告事件id**/
        [XmlAttribute("happenId")]
        public int happenId;

        /** 解锁后设定店铺等级**/
        [XmlAttribute("challengeLv")]
        public int challengeLv;

        /** 是否新手期间不移屏幕**/
        [XmlAttribute("noviceNocam")]
        public bool noviceNocam;

        public static List<sceneunlock> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/sceneunlock");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allssceneunlock table = binaryFormatter.Deserialize(mStream) as allssceneunlock;
                    return table.sceneunlocks;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allssceneunlock
    {
        public List<sceneunlock> sceneunlocks;
    }
}
