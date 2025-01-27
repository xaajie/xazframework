//------------------------------------------------------------
// Xaz Framework
// 多语言管理器
// Feedback: qq515688254
//------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;


public class LanguageData
{
    public string languageName;
    public Dictionary<string, string> localizedValues;
}

public static class Localization
{
	static public bool localizationHasBeenSet = false;
    static LanguageData curLanguage = null;
    // Currently selected language
    static int mLanguageIndex = -1;
    static string mLanguage;
    static List<LanguageData> mDictionary = new List<LanguageData>() { };
    static public string language
    {
        get
        {
            return mLanguage;
        }
        set
        {
            if (mLanguage != value)
            {
                mLanguage = value;
                if (!string.IsNullOrEmpty(value))
                {
                    int languageId = CheckHasLanguage(mLanguage);
                    if(languageId<0)
                    {
                        LoadDictionary(mLanguage);
                    }
                    else
                    {
                        mLanguageIndex = languageId;
                        curLanguage = mDictionary[mLanguageIndex];
                    }
                }
            }
        }
    }

    static private int CheckHasLanguage(string value)
    {
        for(int i = 0; i < mDictionary.Count; i++)
        {
            if(mDictionary[i].languageName == value)
            {
                return i;
            }
        }
        return -1;
    }

    static bool LoadDictionary (string value)
	{
		byte[] bytes = null;
		if (!localizationHasBeenSet)
		{
            TextAsset asset = Xaz.Assets.LoadAsset<TextAsset>(value);//Resources.Load<TextAsset>(value);
            if (asset != null) bytes = asset.bytes;
            localizationHasBeenSet = true;
		}
		if (bytes != null)
		{
            ByteReader reader = new ByteReader(bytes);
            Dictionary<string, string> dictionary = reader.ReadDictionary();
            LanguageData vt = new LanguageData();
            vt.languageName = value;
            vt.localizedValues = dictionary;
            localizationHasBeenSet = false;
            mDictionary.Add(vt);
            mLanguageIndex = mDictionary.Count-1;
            curLanguage = mDictionary[mLanguageIndex];
            return true;
		}
		return false;
	}


	static public string Gets (string key)
	{
		string val;
		if (mLanguageIndex != -1 && curLanguage!=null && curLanguage.localizedValues.TryGetValue(key, out val))
		{
            return val;
		}
		else return key;
	}

	static public string Format (string key, params object[] parameters) { return string.Format(Get(key), parameters); }

//----------------------------------以下为项目自定义内容，以上为NGUI原组件内容--------------------------------------

    //获得图片字图集名称
    static public string GetFontAtlas()
    {
        return Localization.Get("fontImageAtlas");
    }

    public static void RefreshAll()
    {
        Localize[] result = GameObject.FindObjectsOfType<Localize>();
        for (int i = 0; i < result.Length; i++)
        {
            result[i].Refresh();
        }
    }
}
