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
    public class product
    {
        /** 功能建筑id**/
        [XmlAttribute("id")]
        public int id;

        /** 功能建筑名称**/
        [XmlAttribute("name")]
        public string name;

        /** 描述**/
        [XmlAttribute("desc")]
        public string desc;

        /** ICON**/
        [XmlAttribute("icon")]
        public string icon;

        /** 图集名称**/
        [XmlAttribute("atlas")]
        public string atlas;

        /** 局内预制件名称
Prefabs/product/**/
        [XmlAttribute("prefab")]
        public string prefab;

        public static List<product> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/product");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsproduct table = binaryFormatter.Deserialize(mStream) as allsproduct;
                    return table.products;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsproduct
    {
        public List<product> products;
    }
}
