//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIBuilding : UIGameView
{
	protected Button m_CloseBN2;
	protected UITableView m_List;
	protected Text m_Title;
	protected Button m_CloseBN;
	protected UIAnim m_Ani;

	string _xprefabPath = "UIBuilding";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_CloseBN2 = components.Get<Button>(0);
		this.m_List = components.Get<UITableView>(1);
		this.m_Title = components.Get<Text>(2);
		this.m_CloseBN = components.Get<Button>(3);
		this.m_Ani = components.Get<UIAnim>(4);
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
cellv.Attrlist.onCellInit = OnSubCellInit;
cellv.Attrlist.onCellClick = OnSubCellClick;
		}
		mCachedViews[tableCell.transform] = cell;
		return (Cell)cell;
	}
	protected class TV_List
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public UIImage Iconbg;
			public UIImage Buildicon;
			public Text Nametxt;
			public Slider Slider;
			public Button FullBN;
			public Button LevelupBN;
			public Text BnTxt;
			public Image CanlvupArrow;
			public CategoryBox Cost;
			public Text BarTxt;
			public RectTransform Adcost;
			public UISubGroup Attrlist;
			public Text Lvtxt;
			public UIState Cellstate;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_List.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_List.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Iconbg = components.Get<UIImage>(0);
				cell0.Buildicon = components.Get<UIImage>(1);
				cell0.Nametxt = components.Get<Text>(2);
				cell0.Slider = components.Get<Slider>(3);
				cell0.FullBN = components.Get<Button>(4);
				cell0.LevelupBN = components.Get<Button>(5);
				cell0.BnTxt = components.Get<Text>(6);
				cell0.CanlvupArrow = components.Get<Image>(7);
				cell0.Cost = components.Get<CategoryBox>(8);
				cell0.BarTxt = components.Get<Text>(9);
				cell0.Adcost = components.Get<RectTransform>(10);
				cell0.Attrlist = components.Get<UISubGroup>(11);
				cell0.Lvtxt = components.Get<Text>(12);
				cell0.Cellstate = components.Get<UIState>(13);
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
		if (SubGroupname == TV_Attrlist.NAMESTR )		{
			cell = TV_Attrlist.Get(tableCell);
		}
		mCachedViews[tableCell.transform] = cell;
		return (Cell)cell;
	}
	protected class TV_Attrlist
	{
		public static string CELLSTR_ = "";
		public static string NAMESTR = "Attrlist";
		public class Cell0 : Cell
		{
			public UIImage AttrIcon;
			public Text AttrNum;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.AttrIcon = components.Get<UIImage>(0);
				cell0.AttrNum = components.Get<Text>(1);
				cell = cell0;
			}
			return cell;
		}
	}
}
