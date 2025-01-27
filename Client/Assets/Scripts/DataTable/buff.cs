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
    public class buff
    {
        /** buffid**/
        [XmlAttribute("id")]
        public int id;

        /** buff类型
1.属性类
2.限时拥有角色**/
        [XmlAttribute("buffType")]
        public int buffType;

        /** 属性枚举
（属性表id）
**/
        [XmlAttribute("attrId")]
        public int attrId;

        /** 作用对象
buffType=1.角色类型
2.角色id**/
        [XmlAttribute("buffparm")]
        public int buffparm;

        /** 变更值**/
        [XmlAttribute("attrval")]
        public int attrval;

        /** 生效时间(秒)
-1或不填是永久**/
        [XmlAttribute("effectTime")]
        public int effectTime;

        public static List<buff> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/buff");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsbuff table = binaryFormatter.Deserialize(mStream) as allsbuff;
                    return table.buffs;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsbuff
    {
        public List<buff> buffs;
    }
}
