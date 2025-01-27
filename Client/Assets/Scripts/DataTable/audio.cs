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
    public class audio
    {
        /** 唯一id**/
        [XmlAttribute("id")]
        public int id;

        /** 1 背景乐
2 ui音效**/
        [XmlAttribute("aType")]
        public int aType;

        /** 音频文件名
（不含后缀名）
目录Assets\StreamingAssets\Audio**/
        [XmlAttribute("filename")]
        public string filename;

        public static List<audio> LoadBytes()
        {
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>("DB/audio");
            if (asset != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(asset.bytes, 0, asset.bytes.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    allsaudio table = binaryFormatter.Deserialize(mStream) as allsaudio;
                    return table.audios;
                }
              }
              return null;
        }
    }

    [Serializable]
    public class allsaudio
    {
        public List<audio> audios;
    }
}
