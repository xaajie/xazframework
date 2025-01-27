//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIMask : UIGameView
{
	protected RectTransform m_Bg;

	string _xprefabPath = "UIMask";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_Bg = components.Get<RectTransform>(0);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
