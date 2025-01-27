//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIBookSee : UIGameView
{
	protected RectTransform m_Bg;
	protected Button m_CloseBN;
	protected CategoryBox m_Box;

	string _xprefabPath = "UIBookSee";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_Bg = components.Get<RectTransform>(0);
		this.m_CloseBN = components.Get<Button>(1);
		this.m_Box = components.Get<CategoryBox>(2);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
