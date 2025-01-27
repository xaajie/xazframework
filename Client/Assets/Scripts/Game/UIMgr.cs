//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xaz;
using static UIConfirm;
using static XazConfig;

public static class UIMgr
{
    private static UIViewRoot uiViewRoots;

    public static Dictionary<System.Type, List<System.Type>> queueViewConfig = new Dictionary<System.Type, List<System.Type>>()
    {
        { typeof(UILvup), new List<System.Type> {typeof(UILvup) } },
        { typeof(UIChallengeFin), new List<System.Type> {typeof(UILvup) } },
    };

    private static Queue<(System.Type uiType, UIContext context, System.Action<UIView> setDataAction)> cachequeue = new Queue<(System.Type, UIContext, System.Action<UIView>)>();
    static UIMgr()
    {
        uiViewRoot = GameObject.Find(XazConfig.viewRootNode).GetComponent<UIViewRoot>();
    }

    static public void Open<T>(Action<T> opencallback = null, UIContext context = null)
        where T : UIView, new()
    {
        {
            if (uiViewRoot)
            {
                System.Type currentUIType = typeof(T);
                if (queueViewConfig.TryGetValue(currentUIType, out List<System.Type> exclusiveUITypes))
                {
                    foreach (Type exclusiveUIType in exclusiveUITypes)
                    {
                        bool exclusiveUI = uiViewRoot.Exists(exclusiveUIType);
                        if (exclusiveUI)
                        {
                            cachequeue.Enqueuce((currentUIType, context, view => opencallback?.Invoke((T)view)));
                            return;
                        }
                    }
                }
                uiViewRoot.OpenAsync<T>(context, opencallback);

            }
        }
    }

    static public void Close(UIView view, bool destroy = false)
    {
        if (uiViewRoot)
        {
            uiViewRoot.Close(view, destroy);
            CheckCloseQueue();
        }
    }

    private static int checkqueueTimeId;
    static public void Close<T>(bool destroy = true)
           where T : UIView
    {
        {
            if (uiViewRoot)
            {
                uiViewRoot.Close<T>(destroy);
                CheckCloseQueues();
            }
        }
    }

    private static Coroutine _xt; //?????
    public static void WaitCloseUICallBack<T>(Action closecallback)
        where T : UIView
    {
        _xt = XazHelper.StartCoroutine(CheckView<T>(closecallback));
    }

    static IEnumerator CheckView<T>(Action callback)
         where T : UIView
    {
        yield return new WaitUntil(() =>
        {
            return Get<T>() == null;
        });
        callback();
    }
    private static void CheckCloseQueue()
    {
        Scheduler.Remove(ref checkqueueTimeId);
        checkqueueTimeId = Scheduler.Timeout(delegate ()
        {
            NextQueue();
            Scheduler.Remove(ref checkqueueTimeId);
        }, 0.6f);
    }

    private static bool IsMutalUINow(Type exclusiveUIType)
    {
        List<Type> list = queueViewConfig[exclusiveUIType];
        for (int i = 0; i < list.Count; i++)
        {
            if (uiViewRoot.Exists(list[i]))
            {
                return true;
            }
        }
        return false;
    }
    private static void NextQueue()
    {
        Queue<(System.Type, UIContext, System.Action<UIView>)> remainingRequests = new Queue<(System.Type, UIContext, System.Action<UIView>)>();
        int pendingCount = cachequeue.Count;
        bool isHasOpen = false;
        for (int i = 0; i < pendingCount; i++)
        {
            var (pendingUIType, context, setDataAction) = cachequeue.Dequeue();
            bool isAddQueue = true;
            if (!isHasOpen)
            {
                bool isMutal = IsMutalUINow(pendingUIType);
                if (!isMutal)
                {
                    System.Reflection.MethodInfo openMethod = typeof(UIMgr).GetMethod("Open", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    var genericMethod = openMethod.MakeGenericMethod(pendingUIType);
                    genericMethod.Invoke(null, new object[] { setDataAction, context });
                    isHasOpen = true;
                    isAddQueue = false;
                }
            }
            if (isAddQueue)
            {
                remainingRequests.Enqueue((pendingUIType, context, setDataAction));
            }
        }
        cachequeue = remainingRequests;
    }

    static public T Get<T>()
       where T : UIView
    {
        {
            if (uiViewRoot)
            {
                return uiViewRoot.Get<T>();
            }
        }
        return null;
    }
    static public bool IsFocused<T>()
       where T : UIView
    {
        {
            if (uiViewRoot)
            {
                return uiViewRoot.IsFocused<T>();
            }
        }
        return false;
    }

    static public void SetTop<T>()
        where T : UIView, new()
    {
        {
            if (uiViewRoot)
            {
                Close<T>();
                Open<T>();

            }
        }
    }

    static public void SetHideView<T>(bool hide)
    where T : UIView, new()
    {
        {
            if (uiViewRoot)
            {
                RayUtil.SetUILayer(uiViewRoot.Get<T>().gameObject, hide ? LayerDefine.UIINVISIBLE : LayerDefine.UILAYER, !hide);
            }
        }
    }

    //通用弹框
    //漂浮文本
    static public void ShowFlyTip(string det)
    {
        Open<UIFlyTip>(uiView => uiView.SetFlyInfo(det));
    }
    static public void ShowFlyTipKey(string det)
    {
        ShowFlyTip(Utils.GetLang(det));
    }
    //确认框
    static public void ShowConfirm(string desc, ChooseDelegate ok, ChooseDelegate cancel = null)
    {
        ShowConfirm(desc, ok, cancel, string.Empty, string.Empty);
    }

    static public void ShowConfirm(string desc, ChooseDelegate ok, ChooseDelegate cancel, string bluetxt, string redtxt)
    {
        Open<UIConfirm>(uiView => uiView.SetConfirmInfo(desc, ok, cancel));
    }

    static public void CloseAll()
    {
        uiViewRoot.CloseAll(true);
    }
}

