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
    public class gmorder
    {
        /** **/
        [XmlAttribute("id")]
        public int id;

        /** 命令**/
        [XmlAttribute("order")]
        public string order;

        /** 命令使用描述**/
        [XmlAttribute("desc")]
        public string desc;

        /** 是否前端命令**/
        [XmlAttribute("isClient")]
        public bool isClient;

        public static List<gmorder> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/gmorder");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsgmorder table = binaryFormatter.Deserialize(mStream) as allsgmorder;
                    return table.gmorders;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsgmorder
    {
        public List<gmorder> gmorders;
    }
}
