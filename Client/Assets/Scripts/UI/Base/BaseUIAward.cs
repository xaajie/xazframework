//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIAward : UIGameView
{
	protected PrefabPathLoad m_PrefabLoader;
	protected Button m_OkBN;
	protected Button m_AwardBN;
	protected UITableView m_List;
	protected UIAnim m_GameObject;

	string _xprefabPath = "UIAward";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_PrefabLoader = components.Get<PrefabPathLoad>(0);
		this.m_OkBN = components.Get<Button>(1);
		this.m_AwardBN = components.Get<Button>(2);
		this.m_List = components.Get<UITableView>(3);
		this.m_GameObject = components.Get<UIAnim>(4);
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
			public PrefabPathLoad PrefabLoader_1;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_List.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_List.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Box = components.Get<CategoryBox>(0);
				cell0.PrefabLoader_1 = components.Get<PrefabPathLoad>(1);
				cell = cell0;
			}
			return cell;
		}
	}
}
