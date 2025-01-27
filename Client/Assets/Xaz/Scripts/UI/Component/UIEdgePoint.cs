//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
//----------------------------------------------
//  地图上的ui追踪标专用
//  遥杆也用到了，限制圆点显示范围【ID1006272】
//  @author xiejie
//----------------------------------------------
namespace Xaz
{
    using UnityEngine;

    public class UIEdgePoint : MonoBehaviour
    {
        public RectTransform realPoint;
        public RectTransform pointChagneContent; // 追踪的UI元素
        public RectTransform boundsRect;         // 指定的显示范围框
        //是否是圆形范围
        public bool isCircle = false;

        private Vector2 initSize;
        private Vector3 initLocalPos = Vector3.zero;

        public float lerpSpeed = 15f;
        public bool isTween = true;
        void Start()
        {
            // 记录UI元素A的初始位置和尺寸
            initLocalPos = pointChagneContent.localPosition;
            initSize = pointChagneContent.rect.size;
        }
        void Update()
        {
            if (pointChagneContent != null && boundsRect != null)
            {
                // 获取UI元素A的世界坐标
                Vector3 worldPos = realPoint.position;

                // 将世界坐标转换为指定的显示范围框的本地坐标
                Vector3 localPos = boundsRect.InverseTransformPoint(worldPos);

                if (IsInsideBounds(localPos, boundsRect.rect, initSize))
                {
                    // 如果在范围内，逐渐回到原始位置
                    if (isTween)
                    {
                        pointChagneContent.localPosition = Vector3.Lerp(pointChagneContent.localPosition, initLocalPos, Time.deltaTime * lerpSpeed);
                    }
                    else
                    {
                        pointChagneContent.localPosition = initLocalPos;
                    }
                }
                else
                {
                    Vector3 clampedPos;
                    if (isCircle)
                    {
                        clampedPos = ClampToWorldCircle(localPos, boundsRect.rect.center, boundsRect.rect.width / 2f, initSize);
                    }
                    else
                    {
                        clampedPos = ClampToBounds(localPos, boundsRect.rect, initSize);
                    }
                    // 将处理后的本地坐标转换回世界坐标
                    if (isTween)
                    {
                        pointChagneContent.position = Vector3.Lerp(pointChagneContent.position, boundsRect.TransformPoint(clampedPos), Time.deltaTime * lerpSpeed);
                    }
                    else
                    {
                        pointChagneContent.position = boundsRect.TransformPoint(clampedPos);
                    }
                }
            }
        }

        // 检查世界坐标是否在方形范围框内
        bool IsInsideBounds(Vector3 position, Rect bounds, Vector2 elementSize)
        {
            if (isCircle)
            {
                return Vector2.Distance(new Vector2(position.x, position.y), bounds.center) <= (bounds.width / 2f - elementSize.x/2f);
            }
            else
            {
                return bounds.Contains(new Vector2(position.x, position.y));
            }
        }

        // 将坐标限制在方形范围框内，考虑元素的尺寸
        Vector3 ClampToBounds(Vector3 position, Rect bounds, Vector2 elementSize)
        {
            float clampedX = Mathf.Clamp(position.x, bounds.xMin + elementSize.x / 2f, bounds.xMax - elementSize.x / 2f);
            float clampedY = Mathf.Clamp(position.y, bounds.yMin + elementSize.y / 2f, bounds.yMax - elementSize.y / 2f);
            return new Vector3(clampedX, clampedY, position.z);
        }
        // 将坐标限制在圆形范围内
        Vector3 ClampToWorldCircle(Vector3 position, Vector3 center, float radius, Vector2 elementSize)
        {
            Vector3 offset = position - center;
            float distance = offset.magnitude;
            float real_radius = radius - elementSize.x / 2f;
            if (distance > real_radius)
            {
                // 如果超出圆形范围，将坐标限制在圆形边缘
                Vector3 direction = offset.normalized;
                return center + direction * real_radius;
            }

            return position;
        }
    }
}
