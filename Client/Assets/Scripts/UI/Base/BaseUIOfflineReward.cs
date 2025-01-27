//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIOfflineReward : UIGameView
{
	protected Button m_CloseBN;
	protected Text m_Desc;
	protected Text m_TimeTxt;
	protected CategoryBox m_Box;
	protected CategoryBox m_Adbox;
	protected Text m_AddNum;
	protected Button m_GetBN;
	protected Button m_AdGetBN;
	protected UIAnim m_Ani;

	string _xprefabPath = "UIOfflineReward";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_CloseBN = components.Get<Button>(0);
		this.m_Desc = components.Get<Text>(1);
		this.m_TimeTxt = components.Get<Text>(2);
		this.m_Box = components.Get<CategoryBox>(3);
		this.m_Adbox = components.Get<CategoryBox>(4);
		this.m_AddNum = components.Get<Text>(5);
		this.m_GetBN = components.Get<Button>(6);
		this.m_AdGetBN = components.Get<Button>(7);
		this.m_Ani = components.Get<UIAnim>(8);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
