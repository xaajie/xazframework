//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIFlyTip : UIGameView
{
	protected Text m_Desc;

	string _xprefabPath = "UIFlyTip";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_Desc = components.Get<Text>(0);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
