//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIAdBg : UIGameView
{
	protected RawImage m_Bg;

	string _xprefabPath = "UIAdBg";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_Bg = components.Get<RawImage>(0);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
