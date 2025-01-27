//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUITest : UIGameView
{
	protected UIState m_UIState;
	protected Text m_Text;
	protected UITableView m_List;
	protected UIImage m_UIImage;
	protected UIButton m_UIButton;

	string _xprefabPath = "UITest";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_UIState = components.Get<UIState>(0);
		this.m_Text = components.Get<Text>(1);
		this.m_List = components.Get<UITableView>(2);
		this.m_UIImage = components.Get<UIImage>(3);
		this.m_UIButton = components.Get<UIButton>(4);
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
		public static string CELLSTR_2 = "2";
		public class Cell0 : Cell
		{
			public UIState St;
		}
		public static string CELLSTR_m = "m";
		public class Cell1 : Cell
		{
			public Text Text;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_2) {
				//TV_List.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_List.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.St = components.Get<UIState>(0);
				cell = cell0;
			} else if (tableCell.identifier == CELLSTR_m) {
				//TV_List.Cell1 cell = this.GetCellView(tableView, tableCell)  as TV_List.Cell1;
				var cell1 = new Cell1();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell1.Text = components.Get<Text>(0);
				cell = cell1;
			}
			return cell;
		}
	}
}
