//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIHappen : UIGameView
{
	protected Text m_Title;
	protected UIImage m_Icon;
	protected Text m_Desc;
	protected Button m_CancelBN;
	protected Button m_AwardBN;
	protected Button m_CloseBN;
	protected Text m_TimeTxt;
	protected UIAnim m_Ani;

	string _xprefabPath = "UIHappen";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_Title = components.Get<Text>(0);
		this.m_Icon = components.Get<UIImage>(1);
		this.m_Desc = components.Get<Text>(2);
		this.m_CancelBN = components.Get<Button>(3);
		this.m_AwardBN = components.Get<Button>(4);
		this.m_CloseBN = components.Get<Button>(5);
		this.m_TimeTxt = components.Get<Text>(6);
		this.m_Ani = components.Get<UIAnim>(7);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
