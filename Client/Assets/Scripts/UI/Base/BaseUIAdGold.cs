//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIAdGold : UIGameView
{
	protected Button m_CloseBN;
	protected Button m_CancelBN;
	protected Button m_AwardBN;
	protected UIAnim m_Ani;
	protected CategoryBox m_Cost;

	string _xprefabPath = "UIAdGold";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_CloseBN = components.Get<Button>(0);
		this.m_CancelBN = components.Get<Button>(1);
		this.m_AwardBN = components.Get<Button>(2);
		this.m_Ani = components.Get<UIAnim>(3);
		this.m_Cost = components.Get<CategoryBox>(4);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
