//------------------------------------------------------------
// Xaz Framework
// 事件系统
// Feedback: qq515688254
//------------------------------------------------------------
using System.Collections.Generic;

public class EventMgr
{
    public delegate void EventHandler();
    private static Dictionary<EventEnum, List<EventHandler>> EventM = new Dictionary<EventEnum, List<EventHandler>>() { };

    public static void AddEventListener(EventEnum name, EventHandler callBack)
    {
        List<EventHandler> list;
        if (EventM.TryGetValue(name, out list))
        {
            if (list.IndexOf(callBack) <= 0)
            {
                list.Add(callBack);
            }
        }
        else
        {
            list = new List<EventHandler>() { };
            list.Add(callBack);
            EventM.Add(name, list);
        }
    }

    public static void RemoveEventListener(EventEnum name, EventHandler callBack = null)
    {
        if (callBack == null)
        {
            EventM.Remove(name);
        }
        else
        {
            List<EventHandler> list;
            if (EventM.TryGetValue(name, out list))
            {
                list.Remove(callBack);
            }
        }
    }

    /// <summary>
    /// 派发事件
    /// author xiejie
    /// </summary>
    /// <param name="name"></param>
    /// <param name="go"></param>
    public static void DispatchEvent(EventEnum name)
    {
        List<EventHandler> list;
        if (EventM.TryGetValue(name, out list))
        {
            //InvalidOperationException: Collection was modified；
            //foreach (EventHandler handler in list)
            for (int i=0; i<list.Count;i++)
            {
                EventHandler handler = list[i];
                if (handler != null)
                {
                    handler();
                }
            }
        }
    }

    public static void Destroy()
    {
        EventM.Clear();
    }
}
