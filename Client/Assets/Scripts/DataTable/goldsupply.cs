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
    public class goldsupply
    {
        /** 自增id**/
        [XmlAttribute("id")]
        public int id;

        /** 金额区间**/
        [XmlAttribute("config")]
        public List<string> config
        {
            get
            {
                if (_config != null)
                {
                    return _config.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("config")]
        public stringArray _config;

        public static List<goldsupply> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/goldsupply");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsgoldsupply table = binaryFormatter.Deserialize(mStream) as allsgoldsupply;
                    return table.goldsupplys;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsgoldsupply
    {
        public List<goldsupply> goldsupplys;
    }
}
