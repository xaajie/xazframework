//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIMainBottom : UIGameView
{
	protected UITableView m_Toplist;
	protected Button m_SettingBN;
	protected UIAnim m_Ani;

	string _xprefabPath = "UIMainBottom";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_Toplist = components.Get<UITableView>(0);
		this.m_SettingBN = components.Get<Button>(1);
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
		if (tableView == m_Toplist)		{
			cell = TV_Toplist.Get(tableCell);
		}
		mCachedViews[tableCell.transform] = cell;
		return (Cell)cell;
	}
	protected class TV_Toplist
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public Slider Lvbar;
			public CategoryBox Box;
			public Button Add;
			public UITweenText Num;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_Toplist.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_Toplist.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Lvbar = components.Get<Slider>(0);
				cell0.Box = components.Get<CategoryBox>(1);
				cell0.Add = components.Get<Button>(2);
				cell0.Num = components.Get<UITweenText>(3);
				cell = cell0;
			}
			return cell;
		}
	}
}
