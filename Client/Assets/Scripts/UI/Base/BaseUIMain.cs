//------------------------------------------------------------
// Xaz Framework
// Auto Generate
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using TMPro;


public class BaseUIMain : UIGameView
{
	protected UIFixTableView m_Fixlist;
	protected GameJoystick m_Joystick;
	protected UIState m_UIState;
	protected UITableView m_BottomList;
	protected Text m_LvTxt;
	protected Slider m_Lvbar;
	protected PrefabPathLoad m_Bareff;
	protected Text m_ChallengeName;
	protected Button m_MapBN;
	protected UIImage m_ChallengeIcon;
	protected Button m_AchivementBN;
	protected Image m_AchivementRed;
	protected Text m_AchiveName;
	protected UITableView m_NoticeList;
	protected UITableView m_HappenList;
	protected UIAnim m_Ani;
	protected Slider m_Slider;

	string _xprefabPath = "UIMain";
	public override string prefabPath
	{
		get{ return _xprefabPath;}
	}

	sealed protected override void OnCreated()
	{
		base.OnCreated();
		var components = this.GetComponents(this.transform, true);
		this.m_Fixlist = components.Get<UIFixTableView>(0);
		this.m_Joystick = components.Get<GameJoystick>(1);
		this.m_UIState = components.Get<UIState>(2);
		this.m_BottomList = components.Get<UITableView>(3);
		this.m_LvTxt = components.Get<Text>(4);
		this.m_Lvbar = components.Get<Slider>(5);
		this.m_Bareff = components.Get<PrefabPathLoad>(6);
		this.m_ChallengeName = components.Get<Text>(7);
		this.m_MapBN = components.Get<Button>(8);
		this.m_ChallengeIcon = components.Get<UIImage>(9);
		this.m_AchivementBN = components.Get<Button>(10);
		this.m_AchivementRed = components.Get<Image>(11);
		this.m_AchiveName = components.Get<Text>(12);
		this.m_NoticeList = components.Get<UITableView>(13);
		this.m_HappenList = components.Get<UITableView>(14);
		this.m_Ani = components.Get<UIAnim>(15);
		this.m_Slider = components.Get<Slider>(16);
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
		if (tableView == m_Fixlist)		{
			cell = TV_Fixlist.Get(tableCell);
		}
		 else if (tableView == m_BottomList)		{
			cell = TV_BottomList.Get(tableCell);
		}
		 else if (tableView == m_NoticeList)		{
			cell = TV_NoticeList.Get(tableCell);
		}
		 else if (tableView == m_HappenList)		{
			cell = TV_HappenList.Get(tableCell);
		}
		mCachedViews[tableCell.transform] = cell;
		return (Cell)cell;
	}
	protected class TV_Fixlist
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public Image Adcd;
			public UIImage FaceIcon;
			public RectTransform Raw1;
			public Image Cd1;
			public UIImage Rawicon1;
			public Text Rawnum1;
			public RectTransform Raw2;
			public Image Cd2;
			public UIImage Rawicon2;
			public Text Rawnum2;
			public RectTransform Order;
			public UIImage Icon;
			public Text Iconnum;
			public UIState State;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_Fixlist.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_Fixlist.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Adcd = components.Get<Image>(0);
				cell0.FaceIcon = components.Get<UIImage>(1);
				cell0.Raw1 = components.Get<RectTransform>(2);
				cell0.Cd1 = components.Get<Image>(3);
				cell0.Rawicon1 = components.Get<UIImage>(4);
				cell0.Rawnum1 = components.Get<Text>(5);
				cell0.Raw2 = components.Get<RectTransform>(6);
				cell0.Cd2 = components.Get<Image>(7);
				cell0.Rawicon2 = components.Get<UIImage>(8);
				cell0.Rawnum2 = components.Get<Text>(9);
				cell0.Order = components.Get<RectTransform>(10);
				cell0.Icon = components.Get<UIImage>(11);
				cell0.Iconnum = components.Get<Text>(12);
				cell0.State = components.Get<UIState>(13);
				cell = cell0;
			}
			return cell;
		}
	}
	protected class TV_BottomList
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public Button Bn;
			public UIImage Icon;
			public Text Name;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_BottomList.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_BottomList.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Bn = components.Get<Button>(0);
				cell0.Icon = components.Get<UIImage>(1);
				cell0.Name = components.Get<Text>(2);
				cell = cell0;
			}
			return cell;
		}
	}
	protected class TV_NoticeList
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public RectTransform Content;
			public UIImage Icon;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_NoticeList.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_NoticeList.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Content = components.Get<RectTransform>(0);
				cell0.Icon = components.Get<UIImage>(1);
				cell = cell0;
			}
			return cell;
		}
	}
	protected class TV_HappenList
	{
		public static string CELLSTR_ = "";
		public class Cell0 : Cell
		{
			public RectTransform Content;
			public Button Ui__mian_frame03_bg;
			public UIImage Icon;
			public Text Time;
			public UIState UIState;
		}

		static public Cell Get(BaseTableCell tableCell)
		{
			Cell cell = null;
			if (tableCell.identifier == CELLSTR_) {
				//TV_HappenList.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_HappenList.Cell0;
				var cell0 = new Cell0();
				var components = tableCell.transform.GetComponent<UIComponentCollection>();
				cell0.Content = components.Get<RectTransform>(0);
				cell0.Ui__mian_frame03_bg = components.Get<Button>(1);
				cell0.Icon = components.Get<UIImage>(2);
				cell0.Time = components.Get<Text>(3);
				cell0.UIState = components.Get<UIState>(4);
				cell = cell0;
			}
			return cell;
		}
	}
}
