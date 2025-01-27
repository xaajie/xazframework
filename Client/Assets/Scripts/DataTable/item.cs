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
    public class item
    {
        /** 道具ID**/
        [XmlAttribute("id")]
        public int id;

        /** 物品名**/
        [XmlAttribute("name")]
        public string name;

        /** ICON**/
        [XmlAttribute("icon")]
        public string icon;

        /** 图集名称**/
        [XmlAttribute("atlas")]
        public string atlas;

        /** 品质**/
        [XmlAttribute("quality")]
        public int quality;

        /** 道具分类**/
        [XmlAttribute("stype")]
        public int stype;

        /** 描述   **/
        [XmlAttribute("desc")]
        public string desc;

        public static List<item> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/item");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsitem table = binaryFormatter.Deserialize(mStream) as allsitem;
                    return table.items;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsitem
    {
        public List<item> items;
    }
}
