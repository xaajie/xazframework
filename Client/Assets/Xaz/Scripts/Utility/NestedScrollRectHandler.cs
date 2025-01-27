using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NestedScrollRectHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScrollRect outerScrollRect;

    private bool isDragging;

    void Awake()
    {
       
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录开始拖拽时的状态，决定由谁处理拖拽
        isDragging = ShouldDragOuter(eventData);

        if (isDragging)
        {
            outerScrollRect.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 根据开始拖拽时的决定，继续处理拖拽
        if (isDragging)
        {
            outerScrollRect.OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 根据开始拖拽时的决定，结束拖拽
        if (isDragging)
        {
            outerScrollRect.OnEndDrag(eventData);
        }
    }

    private bool ShouldDragOuter(PointerEventData eventData)
    {
        // 判断应该由外层还是内层处理拖拽，可以根据需要调整逻辑
        return Mathf.Abs(eventData.delta.y) > Mathf.Abs(eventData.delta.x);
    }
}
