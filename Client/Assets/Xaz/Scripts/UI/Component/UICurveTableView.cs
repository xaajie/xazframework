//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//  弧形的TableView
//  @author xiejie
//----------------------------------------------------
using System.Collections;
using UnityEngine;
namespace Xaz
{
    public class UICurveTableView : UITableView
    {
        ArrayList localPosY = new ArrayList();

        protected override void Start()
        {
            base.Start();

            onCellInit += CalculateCellOffset;  //cell初始化时
        }

        public override void Clear(bool keepPosition)
        {
            base.Clear(keepPosition);
            localPosY.Clear();
        }
#if UNITY_EDITOR
        public void EditorCurveShow()
        {
            if (!Application.isPlaying)
            {
                CalculateOffset();
            }
        }
        public void EditorCalculateCellOffset(UITableView table, UITableViewCell cell, object data)
        {
            CalculateCellOffset(table, cell, data);
        }
#endif

        protected override void OnPositionChanged(float val)
        {
            base.OnPositionChanged(val);
            CalculateOffset();
        }

        void CalculateCellOffset(UITableView table, UITableViewCell cell, object data)
        {
            //初始化基础位置数组
            if (cell.cellIndex >= localPosY.Count)
            {
                localPosY.Add(cell.transform.localPosition.y);
            }

            CalculateCellOffset(cell);
        }

        private void CalculateOffset()
        {
            if (m_Direction == Direction.Vertical)
            {
                if (m_ColRects.Length < 1)
                {
                    return;
                }

                for (int i = 0; i < m_ActiveCells.Count; i++)
                {
                    UITableViewCell cell = m_ActiveCells[i];
                    CalculateCellOffset(cell);
                }
            }
            else
            {
                //TODO:横向的弧形
            }
        }

        //核心算法，计算cell位置
        private void CalculateCellOffset(UITableViewCell cell)
        {
            // 新首先计算屏幕中显示的圆弧所占整圆的比例，计算出角度，之后计算当前位置占矩形长边（竖直方向）一半的比例，计算出角度，(r*cos(angle) , r*sin(angle))
            // 即为位置，之后转换为原坐标系数值即可

            //显示范围确定
            float w = m_ViewBounds.extents.x * 2f - m_ColRects[0].y / 2f;
            float h = m_ViewBounds.extents.y * 2f;
            float r = (4 * w * w + h * h) / (8 * w);
            float sp = m_StoredMaxScrollPosition >= 0f ? m_StoredMaxScrollPosition / 2 : 0f;
            float realScrollPosition = sp + m_ContentBounds.center.y;
            //由于更改Y轴位置后会出现坐标不可控的问题，故每次计算前先将cell的Y值回到原值再计算（tableView中localPosition为相对固定值）
            float ty = (float)localPosY[cell.cellIndex];    //Y轴原值

            float curYPos = ty + realScrollPosition;
            float percent = Mathf.Abs(curYPos + m_ViewBounds.extents.y) / (m_ViewBounds.extents.y);
            // 扇形角度
            // 通过角度计算，保证元素间与圆心形成的夹角固定，从而保证弧长固定）
            float sinSectorAngle = h / (2f * r);
            float sectorAngle = Mathf.Asin(sinSectorAngle);
            float curAngle = sectorAngle * percent;
            // 当前位置
            float posX = r * Mathf.Cos(curAngle) - (r - w);
            float posY = r * Mathf.Sin(curAngle) - (h / 2f) - realScrollPosition;
            // 下半区
            if (-curYPos > h / 2)
            {
                posY = -(h / 2f) - r * Mathf.Sin(curAngle) - realScrollPosition;
            }

            cell.transform.localPosition = new Vector3(posX, posY);
        }

    }
}
