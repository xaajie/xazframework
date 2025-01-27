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
    public class actor
    {
        /** ID**/
        [XmlAttribute("id")]
        public int id;

        /** 角色类型
101 玩家
201 店员
202 厨师
301 顾客
401 守卫
501 收银员**/
        [XmlAttribute("actorType")]
        public int actorType;

        /** 初始移动速度**/
        [XmlAttribute("walkspeed")]
        public int walkspeed;

        /** 容量**/
        [XmlAttribute("Capacity")]
        public int Capacity;

        /** 睡觉cd(毫秒)**/
        [XmlAttribute("sleepCd")]
        public int sleepCd;

        /** 名称**/
        [XmlAttribute("name")]
        public string name;

        /** 功能性描述
（帮助玩家知道具体作用）**/
        [XmlAttribute("funcDesc")]
        public string funcDesc;

        /** 描述**/
        [XmlAttribute("desc")]
        public string desc;

        /** 是否检测突发事件标**/
        [XmlAttribute("checknotice")]
        public bool checknotice;

        /** 图集**/
        [XmlAttribute("atlas")]
        public string atlas;

        /** 图标
**/
        [XmlAttribute("icon")]
        public string icon;

        /** 头像**/
        [XmlAttribute("headIcon")]
        public string headIcon;

        /** 模型表id**/
        [XmlAttribute("modelId")]
        public int modelId;

        /** 建筑id
（摆货）**/
        [XmlAttribute("putBuildIds")]
        public List<int> putBuildIds
        {
            get
            {
                if (_putBuildIds != null)
                {
                    return _putBuildIds.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("putBuildIds")]
        public intArray _putBuildIds;

        /** 建筑id
（原料供应）
**/
        [XmlAttribute("rawBuildIds")]
        public List<int> rawBuildIds
        {
            get
            {
                if (_rawBuildIds != null)
                {
                    return _rawBuildIds.item;
                }
                return null;
            }
        }
        [XmlElementAttribute("rawBuildIds")]
        public intArray _rawBuildIds;

        public static List<actor> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/actor");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsactor table = binaryFormatter.Deserialize(mStream) as allsactor;
                    return table.actors;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsactor
    {
        public List<actor> actors;
    }
}
