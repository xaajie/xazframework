//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUILogin : UIGameView
{
	protected UITableView m_List;
	protected UIAnim m_Aninode;
	protected Button m_EnterBN;
	protected Button m_Shopbn;
	protected UITableView m_BarList;

	string _xprefabPath = "UILogin";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_List = components.Get<UITableView>(0);
		this.m_Aninode = components.Get<UIAnim>(1);
		this.m_EnterBN = components.Get<Button>(2);
		this.m_Shopbn = components.Get<Button>(3);
		this.m_BarList = components.Get<UITableView>(4);
	}

	private Dictionary<Transform, object> mCachedViews = new Dictionary<Transform, object>();
	private Dictionary<Transform, object> mCachedSubViews = new Dictionary<Transform, object>();
	protected override void OnDestroyed()
	{
		mCachedViews.Clear();
		mCachedSubViews.Clear();
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
		 else if (tableView == m_BarList)		{
			cell = TV_BarList.Get(tableCell);
TV_BarList.Cell0 cellv = cell as TV_BarList.Cell0;
cellv.Subgroups.onCellInit = OnSubCellInit;
cellv.Subgroups.onCellClick = OnSubCellClick;
		}
		mCachedViews[tableCell.transform] = cell;
		return (Cell)cell;
	}
	protected class TV_List
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public UIImage Icon;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_List.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_List.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Icon = components.Get<UIImage>(0);
				cell = cell0;
			}
			return cell;
		}
	}
	protected class TV_BarList
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public Button FinishBN;
			public UISubGroup Subgroups;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_BarList.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_BarList.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.FinishBN = components.Get<Button>(0);
				cell0.Subgroups = components.Get<UISubGroup>(1);
				cell = cell0;
			}
			return cell;
		}
	}

	protected Cell GetSubCellView(UISubGroup tableView, BaseTableCell tableCell,string SubGroupname)
	{
		object cell = null;
		if (mCachedSubViews.TryGetValue(tableCell.transform, out cell))
			return (Cell)cell;
		if (SubGroupname == TV_Subgroups.NAMESTR )		{
			cell = TV_Subgroups.Get(tableCell);
		}
		mCachedViews[tableCell.transform] = cell;
		return (Cell)cell;
	}
	protected class TV_Subgroups
	{
		public static string CELLSTR_ = "";
		public static string NAMESTR = "Subgroups";
		public class Cell0 : Cell
		{
			public Image Fin;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Fin = components.Get<Image>(0);
				cell = cell0;
			}
			return cell;
		}
	}
}
