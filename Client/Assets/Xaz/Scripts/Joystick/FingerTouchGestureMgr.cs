using DigitalRubyShared;
using System.Collections.Generic;
using UnityEngine;

public class FingerTouchGestureMgr : MonoBehaviour
{

    /// <summary>
    /// 长按手势
    /// </summary>
    protected LongPressGestureRecognizer longPressGestureRecognizer;
    /// <summary>
    /// 平移手势
    /// </summary>
    protected PanGestureRecognizer panGestureRecognizer;

    /// <summary>
    /// tap手势
    /// </summary>
    protected TapGestureRecognizer tapGestureRecognizer;

    /// <summary>
    /// 旋转手势
    /// </summary>
    protected RotateGestureRecognizer rotateGestureRecognizer;

    /// <summary>
    /// 缩放手势
    /// </summary>
    protected ScaleGestureRecognizer scaleGestureRecognizer;

    protected static Dictionary<int, GestureRecognizerStateUpdatedDelegate> s_panListenerDic = new Dictionary<int, GestureRecognizerStateUpdatedDelegate>();
    protected static Dictionary<int, GestureRecognizerStateUpdatedDelegate> s_tapListenerDic = new Dictionary<int, GestureRecognizerStateUpdatedDelegate>();
    protected static Dictionary<int, GestureRecognizerStateUpdatedDelegate> s_rotateListenerDic = new Dictionary<int, GestureRecognizerStateUpdatedDelegate>();
    protected static Dictionary<int, GestureRecognizerStateUpdatedDelegate> s_scaleListenerDic = new Dictionary<int, GestureRecognizerStateUpdatedDelegate>();
    protected static Dictionary<int, GestureRecognizerStateUpdatedDelegate> s_dragBeginListenerDic = new Dictionary<int, GestureRecognizerStateUpdatedDelegate>();
    protected static Dictionary<int, GestureRecognizerStateUpdatedDelegate> s_longPressListenerDic = new Dictionary<int, GestureRecognizerStateUpdatedDelegate>();
    protected static Dictionary<int, GestureRecognizerStateUpdatedDelegate> s_dragListenerDic = new Dictionary<int, GestureRecognizerStateUpdatedDelegate>();
    protected static Dictionary<int, GestureRecognizerStateUpdatedDelegate> s_dragEndListenerDic = new Dictionary<int, GestureRecognizerStateUpdatedDelegate>();
    protected static int s_guidCounter = 1;

    // Use this for initialization
    void Start()
    {
        AddTouchGestureListener();
    }

    // Update is called once per frame
    //void Update()
    //{

    //}



    #region 手势管理
    /// <summary>
    /// 添加手势监听  
    /// </summary>
    public void AddTouchGestureListener()
    {
        //长按手势
        longPressGestureRecognizer = new LongPressGestureRecognizer();
        longPressGestureRecognizer.MinimumDurationSeconds = 0.1f;
        longPressGestureRecognizer.StateUpdated += OnLongPressGestureUpdated;
        //平移滑动手势
        panGestureRecognizer = new PanGestureRecognizer();
        panGestureRecognizer.StateUpdated += OnPanGestureUpdated;
        //点击手势
        tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.StateUpdated += OnTapGestureUpdated;
        // we want the tap to fail with the pan gesture tap and pick up a new tap with another finger
        tapGestureRecognizer.ClearTrackedTouchesOnEndOrFail = true;
        tapGestureRecognizer.AllowSimultaneousExecution(panGestureRecognizer);
        //旋转手势
        rotateGestureRecognizer = new RotateGestureRecognizer();
        rotateGestureRecognizer.StateUpdated += OnRotateGestureUpdated;
        //缩放手势
        scaleGestureRecognizer = new ScaleGestureRecognizer();
        scaleGestureRecognizer.StateUpdated += OnScaleGestureUpdated;
        scaleGestureRecognizer.AllowSimultaneousExecution(rotateGestureRecognizer);
        FingersScript.Instance.AddGesture(longPressGestureRecognizer);
        FingersScript.Instance.AddGesture(panGestureRecognizer);
        FingersScript.Instance.AddGesture(tapGestureRecognizer);
        FingersScript.Instance.AddGesture(rotateGestureRecognizer);
        FingersScript.Instance.AddGesture(scaleGestureRecognizer);
    }

    /// <summary>
    /// 移除手势监听
    /// </summary>
    public void RemoveTouchGestureListener()
    {
        if (FingersScript.HasInstance)
        {
            FingersScript.Instance.RemoveGesture(longPressGestureRecognizer);
            FingersScript.Instance.RemoveGesture(panGestureRecognizer);
            FingersScript.Instance.RemoveGesture(tapGestureRecognizer);
            FingersScript.Instance.RemoveGesture(rotateGestureRecognizer);
            FingersScript.Instance.RemoveGesture(scaleGestureRecognizer);
        }
    }

    public void OnLongPressGestureUpdated(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Began)
        {
            StartResListAction(s_dragBeginListenerDic, gesture);
        }
        else if (gesture.State == GestureRecognizerState.Executing)
        {
            StartResListAction(s_dragListenerDic, gesture);
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            StartResListAction(s_dragEndListenerDic, gesture);
        }
    }
    /// <summary>
    /// 平移滑动回调
    /// </summary>
    /// <param name="gesture">The gesture<see cref="GestureRecognizer"/>.</param>
    public void OnPanGestureUpdated(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Began)
        {
            StartResListAction(s_dragBeginListenerDic, gesture);
        }
        else if (gesture.State == GestureRecognizerState.Executing)
        {
            StartResListAction(s_panListenerDic, gesture);
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            StartResListAction(s_dragEndListenerDic, gesture);
        }
    }

    /// <summary>
    /// tap点击回调
    /// </summary>
    /// <param name="gesture">The gesture<see cref="GestureRecognizer"/>.</param>
    public void OnTapGestureUpdated(GestureRecognizer gesture)
    {
        if (gesture.State != GestureRecognizerState.Ended)
        {
            return;
        }
        StartResListAction(s_tapListenerDic, gesture);
    }

    /// <summary>
    /// 旋转手势回调
    /// </summary>
    /// <param name="gesture">The gesture<see cref="GestureRecognizer"/>.</param>
    public void OnRotateGestureUpdated(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            StartResListAction(s_rotateListenerDic, gesture);
        }
    }


    /// <summary>
    /// 缩放手势回调
    /// </summary>
    /// <param name="gesture">The gesture<see cref="GestureRecognizer"/>.</param>
    public void OnScaleGestureUpdated(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            StartResListAction(s_scaleListenerDic, gesture);
        }
    }

    List<GestureRecognizerStateUpdatedDelegate> callbackList = new List<GestureRecognizerStateUpdatedDelegate>();
    private void StartResListAction(Dictionary<int, GestureRecognizerStateUpdatedDelegate> dic, GestureRecognizer gesture)
    {
        callbackList.Clear();
        Dictionary<int, GestureRecognizerStateUpdatedDelegate>.Enumerator scaleCollection = dic.GetEnumerator();
        while (scaleCollection.MoveNext())
        {
            callbackList.Add(scaleCollection.Current.Value);
        }
        for (int i = 0; i < callbackList.Count; i++)
        {
            GestureRecognizerStateUpdatedDelegate callback = callbackList[i];
            callback(gesture);
        }
    }

    #endregion

    #region 手势添加监听器
    public static int AddPanGestureListener(GestureRecognizerStateUpdatedDelegate callback)
    {
        int guid = GetNextGUID();
        s_panListenerDic.Add(guid, callback);
        return guid;
    }
    public static void RemovePanGestureListener(int guid)
    {
        s_panListenerDic.Remove(guid);
    }

    public static int AddTapGestureListener(GestureRecognizerStateUpdatedDelegate callback)
    {
        int guid = GetNextGUID();
        s_tapListenerDic.Add(guid, callback);
        return guid;
    }
    public static void RemoveTapGestureListener(int guid)
    {
        s_tapListenerDic.Remove(guid);
    }

    public static int AddRotateGestureListener(GestureRecognizerStateUpdatedDelegate callback)
    {
        int guid = GetNextGUID();
        s_rotateListenerDic.Add(guid, callback);
        return guid;
    }
    public static void RemoveRotateGestureListener(int guid)
    {
        s_rotateListenerDic.Remove(guid);
    }


    public static int AddScaleGestureListener(GestureRecognizerStateUpdatedDelegate callback)
    {
        int guid = GetNextGUID();
        s_scaleListenerDic.Add(guid, callback);
        return guid;
    }
    public static void RemoveScaleGestureListener(int guid)
    {
        s_scaleListenerDic.Remove(guid);
    }

    public static int AddLongPressGestureListener(GestureRecognizerStateUpdatedDelegate callback)
    {
        int guid = GetNextGUID();
        s_longPressListenerDic.Add(guid, callback);
        return guid;
    }

    public static void RemoveLongPressGestureListener(int guid)
    {
        s_longPressListenerDic.Remove(guid);
    }

    public static int AddDragBeginGestureListener(GestureRecognizerStateUpdatedDelegate callback)
    {
        int guid = GetNextGUID();
        s_dragBeginListenerDic.Add(guid, callback);
        return guid;
    }

    public static void RemoveDragBeginGestureListener(int guid)
    {
        s_dragBeginListenerDic.Remove(guid);
    }

    public static int AddDragGestureListener(GestureRecognizerStateUpdatedDelegate callback)
    {
        int guid = GetNextGUID();
        s_dragListenerDic.Add(guid, callback);
        return guid;
    }

    public static void RemoveDragGestureListener(int guid)
    {
        s_dragListenerDic.Remove(guid);
    }

    public static int AddDragEndGestureListener(GestureRecognizerStateUpdatedDelegate callback)
    {
        int guid = GetNextGUID();
        s_dragEndListenerDic.Add(guid, callback);
        return guid;
    }

    public static void RemoveDragEndGestureListener(int guid)
    {
        s_dragEndListenerDic.Remove(guid);
    }

    private static int GetNextGUID() { return ++s_guidCounter; }
    #endregion
}
