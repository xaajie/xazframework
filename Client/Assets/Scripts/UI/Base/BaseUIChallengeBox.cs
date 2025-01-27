//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIChallengeBox : UIGameView
{
	protected RectTransform m_Content;
	protected Image m_Bg;
	protected Text m_Txt;

	string _xprefabPath = "UIChallengeBox";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_Content = components.Get<RectTransform>(0);
		this.m_Bg = components.Get<Image>(1);
		this.m_Txt = components.Get<Text>(2);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
