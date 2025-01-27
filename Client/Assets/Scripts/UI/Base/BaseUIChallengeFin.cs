//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIChallengeFin : UIGameView
{
	protected Button m_CloseBN;
	protected UIAnim m_Ani;
	protected Text m_Desc;
	protected Text m_Desc_1;

	string _xprefabPath = "UIChallengeFin";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_CloseBN = components.Get<Button>(0);
		this.m_Ani = components.Get<UIAnim>(1);
		this.m_Desc = components.Get<Text>(2);
		this.m_Desc_1 = components.Get<Text>(3);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
