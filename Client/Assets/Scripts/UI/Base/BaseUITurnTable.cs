//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUITurnTable : UIView
{
	protected UIFixTableView m_SlotList;
	protected Button m_CloseBN;
	protected Button m_BuyBN;
	protected UITableView m_OerderList;

	string _xprefabPath = "UITurnTable";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_SlotList = components.Get<UIFixTableView>(0);
		this.m_CloseBN = components.Get<Button>(1);
		this.m_BuyBN = components.Get<Button>(2);
		this.m_OerderList = components.Get<UITableView>(3);
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
		if (tableView == m_SlotList)		{
			cell = TV_SlotList.Get(tableCell);
		}
		 else if (tableView == m_OerderList)		{
			cell = TV_OerderList.Get(tableCell);
		}
		mCachedViews[tableCell.transform] = cell;
		return (Cell)cell;
	}
	protected class TV_SlotList
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public Image Img;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_SlotList.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_SlotList.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Img = components.Get<Image>(0);
				cell = cell0;
			}
			return cell;
		}
	}
	protected class TV_OerderList
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public Image Img;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_OerderList.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_OerderList.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Img = components.Get<Image>(0);
				cell = cell0;
			}
			return cell;
		}
	}
}
