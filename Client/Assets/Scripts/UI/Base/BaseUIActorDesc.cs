//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIActorDesc : UIGameView
{
	protected Button m_CloseBN2;
	protected Text m_Title;
	protected Button m_CloseBN;
	protected Text m_Desc;
	protected UIState m_Cellstate;
	protected Text m_NameTxt;
	protected Text m_Help;
	protected Text m_Attr;
	protected UIAnim m_Ani;
	protected UIImage m_Icon;

	string _xprefabPath = "UIActorDesc";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_CloseBN2 = components.Get<Button>(0);
		this.m_Title = components.Get<Text>(1);
		this.m_CloseBN = components.Get<Button>(2);
		this.m_Desc = components.Get<Text>(3);
		this.m_Cellstate = components.Get<UIState>(4);
		this.m_NameTxt = components.Get<Text>(5);
		this.m_Help = components.Get<Text>(6);
		this.m_Attr = components.Get<Text>(7);
		this.m_Ani = components.Get<UIAnim>(8);
		this.m_Icon = components.Get<UIImage>(9);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
