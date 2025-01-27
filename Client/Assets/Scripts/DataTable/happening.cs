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
    public class happening
    {
        /** id**/
        [XmlAttribute("id")]
        public int id;

        /** 事件类型
新增类型需告知程序**/
        [XmlAttribute("hType")]
        public int hType;

        /** 标题**/
        [XmlAttribute("name")]
        public string name;

        /** 效果描述**/
        [XmlAttribute("desc")]
        public string desc;

        /** 效果参数
(大类_id_数量)
**/
        [XmlAttribute("param")]
        public List<string> param
        {
            get
            {
                if (_param != null)
                {
                    return _param.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("param")]
        public stringArray _param;

        /** 大图**/
        [XmlAttribute("pic")]
        public string pic;

        /** 图标**/
        [XmlAttribute("icon")]
        public string icon;

        /** 图标图集**/
        [XmlAttribute("atlas")]
        public string atlas;

        /** 刷新cd
(秒)**/
        [XmlAttribute("refreshcd")]
        public int refreshcd;

        /** 显示时长
（秒）**/
        [XmlAttribute("duration")]
        public int duration;

        public static List<happening> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/happening");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allshappening table = binaryFormatter.Deserialize(mStream) as allshappening;
                    return table.happenings;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allshappening
    {
        public List<happening> happenings;
    }
}
