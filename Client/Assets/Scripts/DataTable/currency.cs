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
    public class currency
    {
        /** 货币属性id**/
        [XmlAttribute("id")]
        public int id;

        /** 名称**/
        [XmlAttribute("name")]
        public string name;

        /** 描述**/
        [XmlAttribute("desc")]
        public string desc;

        /** 属性图标**/
        [XmlAttribute("icon")]
        public string icon;

        /** 图集**/
        [XmlAttribute("atlas")]
        public string atlas;

        /** 品质**/
        [XmlAttribute("quality")]
        public int quality;

        public static List<currency> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/currency");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allscurrency table = binaryFormatter.Deserialize(mStream) as allscurrency;
                    return table.currencys;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allscurrency
    {
        public List<currency> currencys;
    }
}
