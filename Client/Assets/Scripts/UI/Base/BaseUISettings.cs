//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUISettings : UIGameView
{
	protected Button m_CloseBN;
	protected Toggle m_SoundTo;
	protected Toggle m_MusicTo;
	protected Toggle m_ShakeTo;
	protected Button m_RestartBN;
	protected Button m_BackBN;
	protected Button m_ServicerBN;
	protected Button m_ClearBN;
	protected UIAnim m_Ani;

	string _xprefabPath = "UISettings";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_CloseBN = components.Get<Button>(0);
		this.m_SoundTo = components.Get<Toggle>(1);
		this.m_MusicTo = components.Get<Toggle>(2);
		this.m_ShakeTo = components.Get<Toggle>(3);
		this.m_RestartBN = components.Get<Button>(4);
		this.m_BackBN = components.Get<Button>(5);
		this.m_ServicerBN = components.Get<Button>(6);
		this.m_ClearBN = components.Get<Button>(7);
		this.m_Ani = components.Get<UIAnim>(8);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
