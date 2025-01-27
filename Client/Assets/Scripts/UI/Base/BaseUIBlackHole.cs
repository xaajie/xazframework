//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIBlackHole : UIGameView
{
	protected Image m_StartMask;
	protected UIAnim m_Ani;

	string _xprefabPath = "UIBlackHole";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_StartMask = components.Get<Image>(0);
		this.m_Ani = components.Get<UIAnim>(1);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
