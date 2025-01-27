//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIShop : UIGameView
{
	protected Button m_CloseBN;
	protected UITableView m_List;
	protected UIAnim m_Ani;

	string _xprefabPath = "UIShop";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_CloseBN = components.Get<Button>(0);
		this.m_List = components.Get<UITableView>(1);
		this.m_Ani = components.Get<UIAnim>(2);
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
			public CategoryBox Box;
			public UIImage Icon;
			public RectTransform Awanum;
			public RectTransform Sellout;
			public Button Buy;
			public CategoryBox Costbox;
			public Text InfoTxt;
			public Button Adbuy;
			public Text Adcd;
			public Button FreeBN;
			public UIState Uistate;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_List.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_List.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Box = components.Get<CategoryBox>(0);
				cell0.Icon = components.Get<UIImage>(1);
				cell0.Awanum = components.Get<RectTransform>(2);
				cell0.Sellout = components.Get<RectTransform>(3);
				cell0.Buy = components.Get<Button>(4);
				cell0.Costbox = components.Get<CategoryBox>(5);
				cell0.InfoTxt = components.Get<Text>(6);
				cell0.Adbuy = components.Get<Button>(7);
				cell0.Adcd = components.Get<Text>(8);
				cell0.FreeBN = components.Get<Button>(9);
				cell0.Uistate = components.Get<UIState>(10);
				cell = cell0;
			}
			return cell;
		}
	}
}
