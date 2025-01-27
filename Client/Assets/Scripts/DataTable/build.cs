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
    public class build
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

        /** 功能类别
1.果树
2.货架
3收银台
4机器类**/
        [XmlAttribute("buildType")]
        public int buildType;

        /** 是否显示在升级界面**/
        [XmlAttribute("showlvup")]
        public bool showlvup;

        /** 升级界面排序**/
        [XmlAttribute("sort")]
        public int sort;

        /** 模型表id**/
        [XmlAttribute("modelId")]
        public int modelId;

        public static List<build> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/build");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsbuild table = binaryFormatter.Deserialize(mStream) as allsbuild;
                    return table.builds;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsbuild
    {
        public List<build> builds;
    }
}
