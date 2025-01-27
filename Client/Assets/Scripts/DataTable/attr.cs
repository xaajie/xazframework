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
    public class attr
    {
        /** 属性id**/
        [XmlAttribute("id")]
        public int id;

        /** 名称**/
        [XmlAttribute("name")]
        public string name;

        /** 属性图标**/
        [XmlAttribute("icon")]
        public string icon;

        /** 图集**/
        [XmlAttribute("atlas")]
        public string atlas;

        /** 计算方式
1增减值
2. 百分比**/
        [XmlAttribute("countType")]
        public int countType;

        public static List<attr> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/attr");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsattr table = binaryFormatter.Deserialize(mStream) as allsattr;
                    return table.attrs;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsattr
    {
        public List<attr> attrs;
    }
}
