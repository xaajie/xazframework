using System;
using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// Excel2CsBytesTool
/// Excel可以配置的数组类型：string[] int[] bool[] 
/// 可自行扩展
/// </summary>
namespace Table
{

    [SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class stringArray
    {
        [XmlElementAttribute("item")]
        public List<string> item { get; set; }
    }

    [SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class intArray
    {
        [XmlElementAttribute("item")]
        public List<int> item { get; set; }
    }

    [SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class boolArray
    {
        [XmlElementAttribute("item")]
        public List<bool> item { get; set; }
    }
}
