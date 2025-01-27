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
    public class mainshow
    {
        /** 唯一标识**/
        [XmlAttribute("id")]
        public int id;

        /** 名称**/
        [XmlAttribute("name")]
        public string name;

        /** 1 底部
**/
        [XmlAttribute("showtype")]
        public int showtype;

        /** 强制隐藏，测试版本屏蔽用**/
        [XmlAttribute("debugClose")]
        public bool debugClose;

        /** **/
        [XmlAttribute("atlas")]
        public string atlas;

        /** 图标
(目前只有底部配置图标有效)**/
        [XmlAttribute("icon")]
        public string icon;

        /** 排序
数字越小越前
(和showtype对应关系)**/
        [XmlAttribute("sortIndex")]
        public int sortIndex;

        /** 功能开启id(连接功能开启表)**/
        [XmlAttribute("systemOpenId")]
        public int systemOpenId;

        /** 对应functionShift表id
（点击界面跳转）**/
        [XmlAttribute("funcGotoId")]
        public int funcGotoId;

        public static List<mainshow> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/mainshow");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsmainshow table = binaryFormatter.Deserialize(mStream) as allsmainshow;
                    return table.mainshows;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsmainshow
    {
        public List<mainshow> mainshows;
    }
}
