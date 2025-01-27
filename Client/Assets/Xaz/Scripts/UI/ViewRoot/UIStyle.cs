//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Xaz
{
	
	[Serializable]
	public struct UIStyle
	{
		[SerializeField]
		internal bool popup;
		[SerializeField]
		internal bool topmost;
		[SerializeField]
		internal bool overlapped;
		[SerializeField]
		internal bool overrideColor;
		[SerializeField]
		internal Color maskColor;

		static public UIStyle Get(Color maskColor)
		{
			return new UIStyle() { overrideColor = true, maskColor = maskColor };
		}
		static public UIStyle Get(bool popup, bool topmost, bool overlapped)
		{
			return new UIStyle() { popup = popup, topmost = topmost, overlapped = overlapped };
		}
		static public UIStyle Get(bool popup, bool topmost, bool overlapped, Color maskColor)
		{
			return new UIStyle() { popup = popup, topmost = topmost, overlapped = overlapped, overrideColor = true, maskColor = maskColor };
		}
	}
}
