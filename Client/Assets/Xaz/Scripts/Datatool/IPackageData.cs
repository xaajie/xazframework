using UnityEngine;
using System.Collections;

public interface IPackageData
{
	string GetName();
	string GetDesc();
	string GetIcon();
    string GetAtlas();
    int GetQuality();
    int GetID();
    //小类
	int GetSType();
    int GetNum();
    //大类
    int GetItemType();

    int GetOwnNum();

}
