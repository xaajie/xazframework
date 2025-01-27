//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIBook : UIGameView
{
	protected Button m_CloseBN;
	protected UITableView m_List;
	protected Text m_ProTxt;
	protected Button m_RightBN;
	protected Button m_LeftBN;
	protected Text m_PageNum;
	protected UITableView m_TabList;
	protected UIAnim m_Ani;

	string _xprefabPath = "UIBook";
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
		this.m_ProTxt = components.Get<Text>(2);
		this.m_RightBN = components.Get<Button>(3);
		this.m_LeftBN = components.Get<Button>(4);
		this.m_PageNum = components.Get<Text>(5);
		this.m_TabList = components.Get<UITableView>(6);
		this.m_Ani = components.Get<UIAnim>(7);
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
		 else if (tableView == m_TabList)		{
			cell = TV_TabList.Get(tableCell);
		}
		mCachedViews[tableCell.transform] = cell;
		return (Cell)cell;
	}
	protected class TV_List
	{
		public static string CELLSTR_small = "small";
		public class Cell0 : Cell
		{
			public RectTransform SelectEffect;
			public UIImage Icon;
			public Image Red;
			public Text Txt;
			public UIState Stt;
		}
		public static string CELLSTR_big = "big";
		public class Cell1 : Cell
		{
			public RectTransform SelectEffect;
			public UIImage Icon;
			public Image Red;
			public Text Txt;
			public UIState Stt;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_small) {
				//TV_List.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_List.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.SelectEffect = components.Get<RectTransform>(0);
				cell0.Icon = components.Get<UIImage>(1);
				cell0.Red = components.Get<Image>(2);
				cell0.Txt = components.Get<Text>(3);
				cell0.Stt = components.Get<UIState>(4);
				cell = cell0;
			} else if (tableCell.identifier == CELLSTR_big) {
				//TV_List.Cell1 cell = this.GetCellView(tableView, tableCell)  as TV_List.Cell1;
				var cell1 = new Cell1();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell1.SelectEffect = components.Get<RectTransform>(0);
				cell1.Icon = components.Get<UIImage>(1);
				cell1.Red = components.Get<Image>(2);
				cell1.Txt = components.Get<Text>(3);
				cell1.Stt = components.Get<UIState>(4);
				cell = cell1;
			}
			return cell;
		}
	}
	protected class TV_TabList
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public Button Bg;
			public Text Txt2;
			public Image Select;
			public Text Txt;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_TabList.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_TabList.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Bg = components.Get<Button>(0);
				cell0.Txt2 = components.Get<Text>(1);
				cell0.Select = components.Get<Image>(2);
				cell0.Txt = components.Get<Text>(3);
				cell = cell0;
			}
			return cell;
		}
	}
}
