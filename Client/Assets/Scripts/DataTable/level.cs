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
    public class level
    {
        /** 等级**/
        [XmlAttribute("id")]
        public int id;

        /** 升级所需经验**/
        [XmlAttribute("exp")]
        public int exp;

        /** **/
        [XmlAttribute("gold")]
        public int gold;

        /** **/
        [XmlAttribute("adGold")]
        public int adGold;

        public static List<level> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/level");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allslevel table = binaryFormatter.Deserialize(mStream) as allslevel;
                    return table.levels;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allslevel
    {
        public List<level> levels;
    }
}
