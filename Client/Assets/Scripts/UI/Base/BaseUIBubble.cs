//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIBubble : UIGameView
{
	protected Button m_OkBN;
	protected Button m_CancelBN;
	protected UIAnim m_Ani;

	string _xprefabPath = "UIBubble";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_OkBN = components.Get<Button>(0);
		this.m_CancelBN = components.Get<Button>(1);
		this.m_Ani = components.Get<UIAnim>(2);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
