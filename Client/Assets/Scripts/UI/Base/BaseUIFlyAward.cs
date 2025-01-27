//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIFlyAward : UIGameView
{
	protected UIFixTableView m_Fixlist;
	protected UIAnim m_Ani;

	string _xprefabPath = "UIFlyAward";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_Fixlist = components.Get<UIFixTableView>(0);
		this.m_Ani = components.Get<UIAnim>(1);
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
		if (tableView == m_Fixlist)		{
			cell = TV_Fixlist.Get(tableCell);
		}
		mCachedViews[tableCell.transform] = cell;
		return (Cell)cell;
	}
	protected class TV_Fixlist
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public RectTransform Gold;
			public UIImage Icon_gold4;
			public UIImage Icon_gold5;
			public UIImage Icon_gold2;
			public UIImage Icon_gold6;
			public UIImage Icon_gold1;
			public UIImage Icon_gold3;
			public RectTransform Tip;
			public CategoryBox Box;
			public Text Num;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_Fixlist.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_Fixlist.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Gold = components.Get<RectTransform>(0);
				cell0.Icon_gold4 = components.Get<UIImage>(1);
				cell0.Icon_gold5 = components.Get<UIImage>(2);
				cell0.Icon_gold2 = components.Get<UIImage>(3);
				cell0.Icon_gold6 = components.Get<UIImage>(4);
				cell0.Icon_gold1 = components.Get<UIImage>(5);
				cell0.Icon_gold3 = components.Get<UIImage>(6);
				cell0.Tip = components.Get<RectTransform>(7);
				cell0.Box = components.Get<CategoryBox>(8);
				cell0.Num = components.Get<Text>(9);
				cell = cell0;
			}
			return cell;
		}
	}
}
