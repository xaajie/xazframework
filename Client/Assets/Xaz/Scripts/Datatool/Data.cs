using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
public class Data
{
    public Data root;
    public Data()
    {
    }

    protected virtual void InitData()
    {

    }

    public void SetData(Object data)
    {
        Data.CopyValue(data, this);
    }

    public override string ToString()
    {
        return Data.ToString(this);
    }

    public static string ToString(object data)
    {
        string result = "";
        if (data is IList || data is Array)
        {
            result += "[";
            foreach (object v in data as IList)
            {
                result += Data.ToString(v) + "\n";
            }
            result += "]";
        }
        else if (data is Data)
        {
            Type type = data.GetType();
            FieldInfo[] fields = type.GetFields();

            foreach (FieldInfo field in fields)
            {
                object v = field.GetValue(data);
                result += field.Name + ":" + Data.ToString(v) + "\n";
            }
        }
        else
        {
            result = data != null ? data.ToString() : null;
        }
        return result;
    }

    public static void CopyValue(object source, object target)
    {
        foreach (FieldInfo fi in source.GetType().GetFields())
        {
            fi.SetValue(target, fi.GetValue(source));
        }

        if (target is Data)
            (target as Data).InitData();
    }

    public static T CreateObject<T>(object source)
    {
        T t = Activator.CreateInstance<T>();
        CopyValue(source, t);
        return t;
    }

    public static List<T> CreateList<T>(IEnumerable sourceList)
    {
        List<T> result = new List<T>();

        Type type = null;

        foreach (object node in sourceList)
        {
            if (type == null)
            {
                type = node.GetType();
            }
            result.Add(CreateObject<T>(node));
        }
        return result;
    }

    public static Dictionary<object, T> ListToDic<T>(IEnumerable sourceList, string fieldId = "id")
    {
        Dictionary<object, T> result = new Dictionary<object, T>();

        Type type = null;
        FieldInfo idField = null;
        foreach (object node in sourceList)
        {
            if (type == null)
            {
                type = node.GetType();
                idField = type.GetField(fieldId);
            }
            T t = CreateObject<T>(node);
            object id = idField.GetValue(node);
            try
            {
                result.Add(id, t);
            }
            catch (Exception)
            {
                Logger.Print(id);
                throw;
            }
            //result.Add(id, t);
        }
        return result;
    }

    public static T DeepClone<T>(T RealObject)
    {
        using (Stream stream = new MemoryStream())
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stream, RealObject);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)serializer.Deserialize(stream);
        }
    }

}
