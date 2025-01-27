using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

public class UISafeAreaCtrl : MonoBehaviour
{
    void Start()
    {
        UpdateView();


    }

    void UpdateView()
    {
#if USE_WX && !UNITY_EDITOR
        var sys = WX.GetSystemInfoSync();
        var safeArea = sys.safeArea;
        RectTransform rectTransform = GetComponent<RectTransform>();
        //顶部区域安全差异比例
        float py = (float)safeArea.top / (float)sys.windowHeight;
        //底部区域安全差异比例
        float by = ((float)sys.windowHeight - (float)safeArea.bottom) / (float)sys.windowHeight;
        //得到当前canvasScaler
        var canvas = GameMain.GetCanvasObj();
        var cs = canvas.GetComponent<CanvasScaler>();
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, rectTransform.offsetMax.y - (cs.referenceResolution.y * py));
        //rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, (rectTransform.offsetMin.y + cs.referenceResolution.y * (by)));
#endif
    }
}
