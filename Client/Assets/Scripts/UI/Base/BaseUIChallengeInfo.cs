//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIChallengeInfo : UIGameView
{
	protected Text m_ChanllengeName;
	protected Button m_CloseBN;
	protected UIImage m_Icon;
	protected RectTransform m_Open;
	protected Button m_EnterBN;
	protected RectTransform m_NoOpen;
	protected Text m_Desc;
	protected Button m_OpenBN;
	protected Text m_BnTxt;
	protected CategoryBox m_Cost;
	protected UIAnim m_Ani;

	string _xprefabPath = "UIChallengeInfo";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_ChanllengeName = components.Get<Text>(0);
		this.m_CloseBN = components.Get<Button>(1);
		this.m_Icon = components.Get<UIImage>(2);
		this.m_Open = components.Get<RectTransform>(3);
		this.m_EnterBN = components.Get<Button>(4);
		this.m_NoOpen = components.Get<RectTransform>(5);
		this.m_Desc = components.Get<Text>(6);
		this.m_OpenBN = components.Get<Button>(7);
		this.m_BnTxt = components.Get<Text>(8);
		this.m_Cost = components.Get<CategoryBox>(9);
		this.m_Ani = components.Get<UIAnim>(10);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
