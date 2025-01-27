//------------------------------------------------------------
// Xaz Framework
// 多语言界面组件脚本
// Feedback: qq515688254
//------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Localize : MonoBehaviour
{
    private Text txtComp;
    public string key;
    private string value="";
    bool mStarted = false;

    void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        if (mStarted) OnLocalize();
    }

    void Start()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        mStarted = true;
        txtComp = GetComponent<Text>();
        OnLocalize();
    }


    void OnLocalize()
    {
        if (!string.IsNullOrEmpty(key) && value == "")
        {
            value = Localization.Get(key);
            txtComp.text = value;
        }
    }

    public void Refresh()
    {
        value = "";
        OnLocalize();
    }

}
