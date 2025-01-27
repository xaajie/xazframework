//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIAchivementGet : UIGameView
{
	protected UITableView m_Awalist1;
	protected Button m_AwardBN;
	protected Button m_AdBN;
	protected UITableView m_Awalist2;

	string _xprefabPath = "UIAchivementGet";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_Awalist1 = components.Get<UITableView>(0);
		this.m_AwardBN = components.Get<Button>(1);
		this.m_AdBN = components.Get<Button>(2);
		this.m_Awalist2 = components.Get<UITableView>(3);
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
		if (tableView == m_Awalist1)		{
			cell = TV_Awalist1.Get(tableCell);
		}
		 else if (tableView == m_Awalist2)		{
			cell = TV_Awalist2.Get(tableCell);
		}
		mCachedViews[tableCell.transform] = cell;
		return (Cell)cell;
	}
	protected class TV_Awalist1
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public CategoryBox Box;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_Awalist1.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_Awalist1.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Box = components.Get<CategoryBox>(0);
				cell = cell0;
			}
			return cell;
		}
	}
	protected class TV_Awalist2
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public CategoryBox Box;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_Awalist2.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_Awalist2.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Box = components.Get<CategoryBox>(0);
				cell = cell0;
			}
			return cell;
		}
	}
}
