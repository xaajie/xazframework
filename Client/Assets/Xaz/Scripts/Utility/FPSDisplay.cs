//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
//----------------------------------------------------
// 帧率显示器
//  @author xiejie
//----------------------------------------------------
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public static FPSDisplay Instance = null;
    public bool open = true;
    private float deltaTime = 0.0f;

    private GUIStyle backgroundStyle;
    private GUIStyle textStyle;
    private GUIStyle buttonStyle;

    private Rect fpsRect;
    private Rect fpsRect2;
    private Rect buttonRect;
    string btntxt = "Reporter";
    //private Reporter reporterWindow;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (Instance != this)
            {
                Destroy(transform.gameObject);
            }
        }
        

        GameObject vt = GameObject.Find(btntxt);
        if (vt)
        {
            //reporterWindow = vt.GetComponent<Reporter>();
        }
    }
    void Update()
    {
        if (open)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        }
    }

    float msec;
    float fps;
    string text;
    Color vt = new Color(0.0f, 0.0f, 0.0f, 0.5f);
    void OnGUI()
    {
        if (open)
        {
            msec = deltaTime * 1000.0f;
            fps = 1.0f / deltaTime;
            text = string.Format("{0:0.}/{1}fps\n{2:0.0}ms", fps, Application.targetFrameRate, msec);
            GUI.color = vt;
            GUI.Box(fpsRect, "", backgroundStyle);

            GUI.color = Color.white;
            GUI.Label(fpsRect, text, textStyle);
            //if (GUI.Button(buttonRect, btntxt, buttonStyle))
            //{
            //    if (reporterWindow)
            //    {
            //        reporterWindow.gameObject.SetActive(true);
            //        reporterWindow.doShow();
            //    }
            //}

            backgroundStyle = new GUIStyle();
            backgroundStyle.normal.background = Texture2D.whiteTexture;
            backgroundStyle.alignment = TextAnchor.MiddleCenter;

            float w = Screen.width / 20;
            int fontsizet = Screen.height / 40;
            textStyle = new GUIStyle();
            textStyle.fontSize = fontsizet;
            textStyle.normal.textColor = Color.white;
            textStyle.fontStyle = FontStyle.Bold;
            textStyle.alignment = TextAnchor.MiddleCenter;

            buttonStyle = new GUIStyle("button");
            buttonStyle.fontSize = fontsizet;
            fpsRect = new Rect(10, 10, w, Screen.height / 15);
            buttonRect = new Rect(10, 10 + fpsRect.height, w, Screen.height / 25);
        }
    }
}