//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIAchivement : UIGameView
{
	protected UITableView m_List;
	protected Text m_Title;
	protected Button m_CloseBN;

	string _xprefabPath = "UIAchivement";
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
TV_List.Cell0 cellv = cell as TV_List.Cell0;
cellv.Awalist.onCellInit = OnSubCellInit;
cellv.Awalist.onCellClick = OnSubCellClick;
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
			public Slider Slider;
			public Text SliderTxt;
			public Button GetBN;
			public Button GoBN;
			public Text Desc;
			public Text Desc_1;
			public Text Title;
			public UISubGroup Awalist;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_List.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_List.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Icon = components.Get<UIImage>(0);
				cell0.Slider = components.Get<Slider>(1);
				cell0.SliderTxt = components.Get<Text>(2);
				cell0.GetBN = components.Get<Button>(3);
				cell0.GoBN = components.Get<Button>(4);
				cell0.Desc = components.Get<Text>(5);
				cell0.Desc_1 = components.Get<Text>(6);
				cell0.Title = components.Get<Text>(7);
				cell0.Awalist = components.Get<UISubGroup>(8);
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
		if (SubGroupname == TV_Awalist.NAMESTR )		{
			cell = TV_Awalist.Get(tableCell);
		}
		mCachedViews[tableCell.transform] = cell;
		return (Cell)cell;
	}
	protected class TV_Awalist
	{
		public static string CELLSTR_ = "";
		public static string NAMESTR = "Awalist";
		public class Cell0 : Cell
		{
			public CategoryBox Box;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Box = components.Get<CategoryBox>(0);
				cell = cell0;
			}
			return cell;
		}
	}
}
