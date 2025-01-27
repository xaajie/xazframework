//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUILvup : UIGameView
{
	protected PrefabPathLoad m_EffectNode;
	protected CategoryBox m_Box;
	protected CategoryBox m_Box2;
	protected Button m_AwaBN1;
	protected CategoryBox m_Awabox1;
	protected Button m_AwaBN2;
	protected CategoryBox m_Awabox2;
	protected UIAnim m_Ani;

	string _xprefabPath = "UILvup";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_EffectNode = components.Get<PrefabPathLoad>(0);
		this.m_Box = components.Get<CategoryBox>(1);
		this.m_Box2 = components.Get<CategoryBox>(2);
		this.m_AwaBN1 = components.Get<Button>(3);
		this.m_Awabox1 = components.Get<CategoryBox>(4);
		this.m_AwaBN2 = components.Get<Button>(5);
		this.m_Awabox2 = components.Get<CategoryBox>(6);
		this.m_Ani = components.Get<UIAnim>(7);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
