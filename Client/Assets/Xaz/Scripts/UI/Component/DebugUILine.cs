//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// UI事件会在EventSystem在Update的Process触发。UGUI会遍历屏幕中所有RaycastTarget是true的UI，接着就会发射线，并且排序找到玩家最先触发的那个UI，在抛出事件给逻辑层去响应
/// </summary>
public class DebugUILine : MonoBehaviour {
	static Vector3[] fourCorners = new Vector3[4];

	void Start()
	{
	}

	void OnDrawGizmos()
	{
		foreach (MaskableGraphic g in GameObject.FindObjectsOfType<MaskableGraphic>())
		{
			if (g.raycastTarget)
			{
				RectTransform rectTransform = g.transform as RectTransform;
				rectTransform.GetWorldCorners(fourCorners);
				Gizmos.color = Color.blue;
				for (int i = 0; i < 4; i++)
					Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);

			}
		}
	}
}
#endif