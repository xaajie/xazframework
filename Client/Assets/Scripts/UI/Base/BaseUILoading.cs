//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUILoading : UIGameView
{
	protected Text m_Txt;
	protected Slider m_Bar;
	protected Button m_ChallengeBN;

	string _xprefabPath = "UILoading";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_Txt = components.Get<Text>(0);
		this.m_Bar = components.Get<Slider>(1);
		this.m_ChallengeBN = components.Get<Button>(2);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
