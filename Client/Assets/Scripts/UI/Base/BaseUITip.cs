//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUITip : UIGameView
{
	protected UIFixTableView m_Fixlist;

	string _xprefabPath = "UITip";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_Fixlist = components.Get<UIFixTableView>(0);
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
			public UIState UIState;
			public Image Bg;
			public Image TimeSpt;
			public Text Name;
			public UIImage Icon;
			public Text QualityStr;
			public UIRichText Desc;
			public UIRichText Desc2;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_Fixlist.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_Fixlist.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.UIState = components.Get<UIState>(0);
				cell0.Bg = components.Get<Image>(1);
				cell0.TimeSpt = components.Get<Image>(2);
				cell0.Name = components.Get<Text>(3);
				cell0.Icon = components.Get<UIImage>(4);
				cell0.QualityStr = components.Get<Text>(5);
				cell0.Desc = components.Get<UIRichText>(6);
				cell0.Desc2 = components.Get<UIRichText>(7);
				cell = cell0;
			}
			return cell;
		}
	}
}
