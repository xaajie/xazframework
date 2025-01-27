//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
//----------------------------------------------
//  根据内容边界超出翻转,并修改承托线
//  @author xiejie
//----------------------------------------------
namespace Xaz
{
    using UnityEngine;
    using UnityEngine.UI;
    public class UIEdgeTurnSet : MonoBehaviour
    {
        private RectTransform tipbox;
        private RectTransform clicktarget;
        private RectTransform boundsRect;
        public UIState line;

        static string TOPLEFT = "topleft";
        static string BOTTOMLEFT = "bottomleft";
        static string BOTTOMRIGHT = "bottomright";
        static string TOPRIGHT = "topright";
        static string EMPTY = "empty";
        //线在两边占据的宽度之和
        public float lineWidth = 20;
        void Start()
        {
            //tipbox = this.GetComponent<RectTransform>();
        }

        void Update()
        {
            //一定时间内重算，让layout执行完全
            if (Time.time < countTimeEnd && canRefresh)
            {
                Refresh();
            }
        }

        private float targetWidth;
        private float tipboxWidth;
        private float tipboxHeight;
        private float countTimeEnd;
        Vector3 targetPositionInBounds;
        private bool canRefresh = false;
        public void StartAdjustTip(GameObject clicktargetf, RectTransform boundsRectf)
        {
            canRefresh = false;
            countTimeEnd = Time.time + 3;
            clicktarget = clicktargetf.GetComponent<RectTransform>();
            //LayoutRebuilder.ForceRebuildLayoutImmediate(boundsRectf);
            boundsRect = boundsRectf;
            tipbox = this.GetComponent<RectTransform>();
            // 这里得是显示对象(地图上的元素会被slider控制放大过啊。。。。。。。lossyScale失效)
            Transform oldparent = clicktargetf.transform.parent;
            clicktargetf.transform.SetParent(boundsRectf.transform);
            targetWidth = clicktarget.rect.width * clicktarget.localScale.x;
            clicktargetf.transform.SetParent(oldparent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(tipbox);
            Refresh();
            canRefresh = true;
        }

        public void Refresh()
        {
            if (clicktarget != null && tipbox != null)
            {
                targetPositionInBounds = boundsRect.InverseTransformPoint(clicktarget.position);
                Vector3 targetTipboxPosition;
                tipboxWidth = tipbox.rect.width + lineWidth;
                tipboxHeight = tipbox.rect.height + lineWidth;
                // 如果tipbox在clicktarget的右侧会超出boundsRect的范围
                if ((targetPositionInBounds.x + targetWidth / 2 + tipboxWidth) > boundsRect.rect.width / 2)
                {
                    //// 如果tipbox显示不完整超出boundsRect上界限
                    if ((targetPositionInBounds.y + tipboxHeight) > boundsRect.rect.height / 2)
                    {
                        // 在clicktarget的左下方显示tipbox
                        targetTipboxPosition = targetPositionInBounds + new Vector3(-targetWidth / 2 - tipboxWidth / 2, -tipboxHeight / 2, 0);
                        SetLineState(TOPRIGHT);
                    }
                    else
                    {
                        targetTipboxPosition = targetPositionInBounds + new Vector3(-targetWidth / 2 - tipboxWidth / 2, tipboxHeight / 2, 0);
                        SetLineState(BOTTOMRIGHT);
                    }
                }
                else
                {
                    if ((targetPositionInBounds.y + tipboxHeight) > boundsRect.rect.height / 2)
                    {
                        // 在clicktarget的左下方显示tipbox
                        targetTipboxPosition = targetPositionInBounds + new Vector3(targetWidth / 2 + tipboxWidth / 2, -tipboxHeight / 2, 0);
                        SetLineState(TOPLEFT);
                    }
                    else
                    {
                        targetTipboxPosition = targetPositionInBounds + new Vector3(targetWidth / 2 + tipboxWidth / 2, tipboxHeight / 2, 0);
                        SetLineState(BOTTOMLEFT);
                    }
                }

                // 设置tipbox的位置，确保当tips框过长的情况不超出boundsRect的范围，这种情况线需要隐藏
                //Debug.Log("11111111-----"+Mathf.Abs(targetTipboxPosition.y));
                //Debug.Log( tipboxHeight);
                if ((-targetTipboxPosition.y + tipboxHeight/2) < boundsRect.rect.height/2)
                {
                    tipbox.position = boundsRect.TransformPoint(targetTipboxPosition);
                }
                else
                {
                    targetTipboxPosition = ClampToBounds(targetTipboxPosition, boundsRect.rect, tipboxWidth, tipboxHeight);
                    tipbox.position = boundsRect.TransformPoint(targetTipboxPosition);
                    SetLineState(EMPTY);
                }
            }
        }
         
        void SetLineState(string vt)
        {
            if (line)
            {
                line.SetState(vt);
            }
        }

        // 将坐标限制在方形范围框内，考虑元素的尺寸
        Vector3 ClampToBounds(Vector3 position, Rect bounds, float tipboxWidth, float tipboxHeight)
        {
            float clampedX = Mathf.Clamp(position.x, bounds.xMin + tipboxWidth / 2f, bounds.xMax - tipboxWidth / 2f);
            float clampedY = Mathf.Clamp(position.y, bounds.yMin + tipboxHeight / 2f, bounds.yMax - tipboxHeight / 2f);
            return new Vector3(clampedX, clampedY, position.z);
        }

    }
}
