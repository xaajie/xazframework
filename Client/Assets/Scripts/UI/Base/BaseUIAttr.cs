//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIAttr : UIGameView
{
	protected UITableView m_List;
	protected Text m_Title;
	protected Button m_CloseBN;

	string _xprefabPath = "UIAttr";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_List = components.Get<UITableView>(0);
		this.m_Title = components.Get<Text>(1);
		this.m_CloseBN = components.Get<Button>(2);
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
			public UIImage Buildicon;
			public Text Lvtxt;
			public Button LevelupBN;
			public CategoryBox Cost;
			public RectTransform Adcost;
			public UIImage AttrIcon;
			public Text AttrCurNum;
			public Text Line;
			public Text AttrNum;
			public Text AttrName;
			public UIState Cellstate;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_List.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_List.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Buildicon = components.Get<UIImage>(0);
				cell0.Lvtxt = components.Get<Text>(1);
				cell0.LevelupBN = components.Get<Button>(2);
				cell0.Cost = components.Get<CategoryBox>(3);
				cell0.Adcost = components.Get<RectTransform>(4);
				cell0.AttrIcon = components.Get<UIImage>(5);
				cell0.AttrCurNum = components.Get<Text>(6);
				cell0.Line = components.Get<Text>(7);
				cell0.AttrNum = components.Get<Text>(8);
				cell0.AttrName = components.Get<Text>(9);
				cell0.Cellstate = components.Get<UIState>(10);
				cell = cell0;
			}
			return cell;
		}
	}
}
