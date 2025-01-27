//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIConfirm : UIGameView
{
	protected Text m_Title;
	protected Text m_Desc;
	protected UIAnim m_Ani;
	protected Button m_CancelBN;
	protected Button m_OkBN;

	string _xprefabPath = "UIConfirm";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_Title = components.Get<Text>(0);
		this.m_Desc = components.Get<Text>(1);
		this.m_Ani = components.Get<UIAnim>(2);
		this.m_CancelBN = components.Get<Button>(3);
		this.m_OkBN = components.Get<Button>(4);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
