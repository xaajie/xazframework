//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUINovice : UIGameView
{
	protected RectTransform m_HandMask;
	protected RawImage m_FirtTip;
	protected Image m_Moveeff;
	protected Image m_Tipbg;
	protected Text m_NoviceTxt;
	protected RectTransform m_Hand;
	protected PrefabPathLoad m_Handeff;
	protected Image m_HandBN;
	protected RectTransform m_OkSpt;
	protected Text m_OkTxt;
	protected Button m_OkBN;
	protected RectTransform m_Arrows ;
	protected Image m_PosArrow;

	string _xprefabPath = "UINovice";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_HandMask = components.Get<RectTransform>(0);
		this.m_FirtTip = components.Get<RawImage>(1);
		this.m_Moveeff = components.Get<Image>(2);
		this.m_Tipbg = components.Get<Image>(3);
		this.m_NoviceTxt = components.Get<Text>(4);
		this.m_Hand = components.Get<RectTransform>(5);
		this.m_Handeff = components.Get<PrefabPathLoad>(6);
		this.m_HandBN = components.Get<Image>(7);
		this.m_OkSpt = components.Get<RectTransform>(8);
		this.m_OkTxt = components.Get<Text>(9);
		this.m_OkBN = components.Get<Button>(10);
		this.m_Arrows  = components.Get<RectTransform>(11);
		this.m_PosArrow = components.Get<Image>(12);
	}

	protected override void OnDestroyed()
	{
		base.OnDestroyed();
	}
}
