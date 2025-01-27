//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIChallenge : UIGameView
{
	protected UIFixTableView m_List;
	protected RectTransform m_Node0;
	protected RectTransform m_Node1;
	protected RectTransform m_Node2;
	protected RectTransform m_Node3;
	protected RectTransform m_Node4;
	protected RectTransform m_Node5;
	protected RectTransform m_Node6;
	protected RectTransform m_Node7;
	protected RectTransform m_Node8;
	protected RectTransform m_Node9;
	protected Button m_CloseBN;
	protected PrefabPathLoad m_PrefabLoader;

	string _xprefabPath = "UIChallenge";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_List = components.Get<UIFixTableView>(0);
		this.m_Node0 = components.Get<RectTransform>(1);
		this.m_Node1 = components.Get<RectTransform>(2);
		this.m_Node2 = components.Get<RectTransform>(3);
		this.m_Node3 = components.Get<RectTransform>(4);
		this.m_Node4 = components.Get<RectTransform>(5);
		this.m_Node5 = components.Get<RectTransform>(6);
		this.m_Node6 = components.Get<RectTransform>(7);
		this.m_Node7 = components.Get<RectTransform>(8);
		this.m_Node8 = components.Get<RectTransform>(9);
		this.m_Node9 = components.Get<RectTransform>(10);
		this.m_CloseBN = components.Get<Button>(11);
		this.m_PrefabLoader = components.Get<PrefabPathLoad>(12);
	}

	private Dictionary<Transform, object> mCachedViews = new Dictionary<Transform, object>();
	protected override void OnDestroyed()
	{
		mCachedViews.Clear();
		base.OnDestroyed();
	}

	protected interface Cell { };
	protected Cell GetCellView(BaseTable tableView, BaseTableCell tableCell)
	{
		object cell = null;
		if (mCachedViews.TryGetValue(tableCell.transform, out cell))
			return (Cell)cell;
		if (tableView == m_List)		{
			cell = TV_List.Get(tableCell);
		}
		mCachedViews[tableCell.transform] = cell;
		return (Cell)cell;
	}
	protected class TV_List
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public UIImage Img;
			public UIGray Loc;
			public UIImage LockImg;
			public UIState UIState;
			public Button Bn;
			public Text Txt;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_List.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_List.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Img = components.Get<UIImage>(0);
				cell0.Loc = components.Get<UIGray>(1);
				cell0.LockImg = components.Get<UIImage>(2);
				cell0.UIState = components.Get<UIState>(3);
				cell0.Bn = components.Get<Button>(4);
				cell0.Txt = components.Get<Text>(5);
				cell = cell0;
			}
			return cell;
		}
	}
}
