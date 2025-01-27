//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIGMEnter : UIGameView
{
	protected Button m_ResetBN;
	protected RectTransform m_DebugSpt;
	protected InputField m_InputField;
	protected Button m_GmBN;
	protected Button m_ClosegmBN;
	protected UITabButton m_OneToggle;
	protected UIState m_UIState;
	protected InputField m_BigInputField;
	protected Button m_DebugBN;

	string _xprefabPath = "UIGMEnter";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_ResetBN = components.Get<Button>(0);
		this.m_DebugSpt = components.Get<RectTransform>(1);
		this.m_InputField = components.Get<InputField>(2);
		this.m_GmBN = components.Get<Button>(3);
		this.m_ClosegmBN = components.Get<Button>(4);
		this.m_OneToggle = components.Get<UITabButton>(5);
		this.m_UIState = components.Get<UIState>(6);
		this.m_BigInputField = components.Get<InputField>(7);
		this.m_DebugBN = components.Get<Button>(8);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
