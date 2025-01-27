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
    public class model
    {
        /** 模型id**/
        [XmlAttribute("id")]
        public int id;

        /** 资源预制件路径**/
        [XmlAttribute("prefabpath")]
        public string prefabpath;

        public static List<model> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/model");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsmodel table = binaryFormatter.Deserialize(mStream) as allsmodel;
                    return table.models;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsmodel
    {
        public List<model> models;
    }
}
