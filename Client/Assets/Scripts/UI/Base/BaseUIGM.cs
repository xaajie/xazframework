//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIGM : UIGameView
{
	protected InputField m_InputField;
	protected Button m_FindBN;
	protected UITableView m_List;
	protected Button m_CloseBN;
	protected Button m_AllBN;

	string _xprefabPath = "UIGM";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_InputField = components.Get<InputField>(0);
		this.m_FindBN = components.Get<Button>(1);
		this.m_List = components.Get<UITableView>(2);
		this.m_CloseBN = components.Get<Button>(3);
		this.m_AllBN = components.Get<Button>(4);
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
			public Button Img;
			public Text OrderTxt;
			public Text DescTxt;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_List.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_List.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Img = components.Get<Button>(0);
				cell0.OrderTxt = components.Get<Text>(1);
				cell0.DescTxt = components.Get<Text>(2);
				cell = cell0;
			}
			return cell;
		}
	}
}
