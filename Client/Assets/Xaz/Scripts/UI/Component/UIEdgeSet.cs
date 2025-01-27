//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
//----------------------------------------------
//  内容超框让边缘对齐脚本
//  @author xiejie
//  本体是左上锚点
//----------------------------------------------
namespace Xaz
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;


    public class UIEdgeSet : MonoBehaviour
    {
        private const int Bottom = 3;
        private const int Right = 2;
        private const int BottomRight = 5;
        private Vector2 initPivot;
        /// <summary>
        /// 关联实际宽高的目标
        /// </summary>
        public RectTransform relativeContent;

        private RectTransform target;
        void Awake()
        {
            target = gameObject.GetComponent<RectTransform>();
            initPivot = target.pivot;
        }

        private IEnumerator RefreshEdgeShow(float space, Transform parentView)
        {
            yield return XazHelper.waitFrame;
            if (relativeContent != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(relativeContent);
                yield return XazHelper.waitFrame;
                target.sizeDelta = relativeContent.rect.size;
            }
            else
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(target);
            }
            int type = -1;
            Vector3 pos = target.anchoredPosition3D;
            RectTransform fullview = parentView.transform.GetComponent<RectTransform>();
            float wid = fullview.rect.width / 2;
            float hid = fullview.rect.height / 2;
            if ((pos.y < (-hid + target.sizeDelta.y)) && (pos.x > (wid - target.sizeDelta.x)))
            {
                type = BottomRight;
            }
            else if (pos.y < (-hid + target.sizeDelta.y))
            {
                type = Bottom;
            }
            else if (pos.x > (wid - target.sizeDelta.x))
            {
                type = Right;
            }
            Transform cacheParm = target.transform.parent;
            target.transform.SetParent(parentView);
            //jietodo，其他情况遇到再说吧
            if (type > 0)
            {
                SetUIEdge(pos, type, space);
            }
            target.transform.SetParent(cacheParm);
        }

        public void RefreshEdge(float space, Transform parentView)
        {
            if (this.gameObject.activeSelf)
            {
                StartCoroutine(RefreshEdgeShow(space, parentView));
            }
        }
        /// <summary>
        /// 设置ui对象边距适应
        /// </summary>
        /// <param name="target">对象</param>
        /// <param name="type">1靠左 2靠右 3Bottom 4 Top</param>
        /// <param name="num">距离</param>
        private void SetUIEdge(Vector3 pos, int type, float num)
        {
           if (type == Right)
            {
                target.pivot = new Vector2(1, initPivot.y);
                target.anchorMin = new Vector2(1, 0.5f);
                target.anchorMax = new Vector2(1, 0.5f);
                target.anchoredPosition3D = new Vector3(num, pos.y, 0);
            }
            else if (type == Bottom)
            {
                target.pivot = new Vector2(initPivot.x, 0);
                target.anchorMin = new Vector2(0.5f, 0);
                target.anchorMax = new Vector2(0.5f, 0);
                target.anchoredPosition3D = new Vector3(pos.x, num, 0);
            }
            else if (type == BottomRight)
            {
                target.anchorMin = new Vector2(1f, 0);
                target.anchorMax = new Vector2(1f, 0);
                target.pivot = new Vector2(1f, 0);
                target.anchoredPosition3D = new Vector3(num, num, 0);
            }
        }
    }
}