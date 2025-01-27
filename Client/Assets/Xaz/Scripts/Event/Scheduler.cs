//------------------------------------------------------------
// Xaz Framework
// 定时器
// Feedback: qq515688254
//----------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xaz
{

    static public class Scheduler
    {
        private class Task
        {
            static private Task m_Current;

            public int id;
            public Action action;
            public int repeat;
            public float interval;
            public float time;
            public bool ignoreTimeScale;
            public bool destroyed;
            public int position;

            public void Run()
            {
                if (repeat > 0 && --repeat == 0)
                {
                    m_TaskIdMap.Remove(id);
                    m_TaskActionMap.Remove(action);
                    destroyed = true;
                }

                m_Current = this;
                action();
                m_Current = null;
            }

            public bool running
            {
                get
                {
                    return m_Current == this;
                }
            }

            public static bool operator <(Task l, Task r)
            {
                return l.time < r.time || (l.time == r.time && l.id < r.id);
            }
            public static bool operator >(Task l, Task r)
            {
                return l.time > r.time || (l.time == r.time && l.id > r.id);
            }
        }

        static private int m_UniqueId = 0;
        static private readonly ObjectPool<Task> m_TaskPool;

        static private List<Task> m_TaskList = new List<Task>();
        static private List<Task> m_TaskUnscaledList = new List<Task>();
        static private Queue<Task> m_TaskUpdateList = new Queue<Task>();

        static private Dictionary<int, Task> m_TaskIdMap = new Dictionary<int, Task>();
        static private Dictionary<Action, Task> m_TaskActionMap = new Dictionary<Action, Task>();

        static Scheduler()
        {
            m_TaskPool = new ClassPool<Task>((t) =>
            {
                t.id = ++m_UniqueId;
                t.position = -1;
                t.destroyed = false;
            }, (t) =>
            {
                t.action = null;
                t.destroyed = true;
            });

            XazHelper.onInternalReload += () =>
            {
                RemoveAll();
            };

            XazHelper.CreateModule<SchedulerBehaviour>();
        }

        #region static variables and functions
        static public int Update(Action action)
        {
            return Schedule(action, 0f, 0, 0f, false);
        }
        static public int Update(Action action, int repeat)
        {
            return Schedule(action, 0f, repeat, 0f, false);
        }

        static public int Timeout(Action action)
        {
            return Schedule(action, 0f, 1, 0f, false);
        }
        static public int Timeout(Action action, float delay)
        {
            return Schedule(action, 0f, 1, delay, false);
        }
        static public int Timeout(Action action, float delay, bool ignoreTimeScale)
        {
            return Schedule(action, 0f, 1, delay, ignoreTimeScale);
        }

        static public int Interval(Action action, float interval)
        {
            return Schedule(action, interval, 0, 0f, false);
        }
        static public int Interval(Action action, float interval, bool ignoreTimeScale)
        {
            return Schedule(action, interval, 0, 0f, ignoreTimeScale);
        }
        static public int Interval(Action action, float interval, int repeat)
        {
            return Schedule(action, interval, repeat, 0f, false);
        }
        static public int Interval(Action action, float interval, int repeat, bool ignoreTimeScale)
        {
            return Schedule(action, interval, repeat, 0f, ignoreTimeScale);
        }
        static public int Interval(Action action, float interval, int repeat, float delay)
        {
            return Schedule(action, interval, repeat, delay, false);
        }
        static public int Interval(Action action, float interval, int repeat, float delay, bool ignoreTimeScale)
        {
            return Schedule(action, interval, repeat, delay, ignoreTimeScale);
        }

        static private int Schedule(Action action, float interval, int repeat, float delay, bool ignoreTimeScale)
        {
            Task task;
            if (m_TaskActionMap.TryGetValue(action, out task))
            {
                return task.id;
            }
            task = m_TaskPool.Get();
            task.action = action;
            task.interval = interval;
            task.repeat = repeat < 0 ? 0 : repeat;
            if (interval == 0 && delay == 0)
            {
                m_TaskUpdateList.Enqueue(task);
            }
            else
            {
                task.time = (ignoreTimeScale ? Time.unscaledTime : Time.time) + interval + delay;
                task.ignoreTimeScale = ignoreTimeScale;
                InsertTask(ignoreTimeScale ? m_TaskUnscaledList : m_TaskList, task);
            }
            m_TaskIdMap.Add(task.id, task);
            m_TaskActionMap.Add(task.action, task);
            return task.id;
        }

        static public void RemoveAll()
        {
            using (var it = m_TaskIdMap.GetEnumerator())
            {
                while (it.MoveNext())
                {
                    Task t = it.Current.Value;
                    if (t.running)
                    {
                        t.destroyed = true;
                    }
                    else
                    {
                        m_TaskPool.Release(t);
                    }
                }
            }
            if (m_TaskList.Count > 0)
            {
                var t = m_TaskList[0];
                m_TaskList.Clear();
                if (t.running)
                {
                    m_TaskList.Add(t);
                }
            }
            if (m_TaskUnscaledList.Count > 0)
            {
                var t = m_TaskUnscaledList[0];
                m_TaskUnscaledList.Clear();
                if (t.running)
                {
                    m_TaskUnscaledList.Add(t);
                }
            }
            m_TaskUpdateList.Clear();
            m_TaskIdMap.Clear();
            m_TaskActionMap.Clear();
        }

        static public bool Remove(ref int taskId)
        {
            Task task;
            if (m_TaskIdMap.TryGetValue(taskId, out task))
            {
                Remove(task);
                taskId = 0;
                return true;
            }
            return false;
        }

        static public bool Remove(Action action)
        {
            Task task;
            if (m_TaskActionMap.TryGetValue(action, out task))
            {
                Remove(task);
                return true;
            }
            return false;
        }

        static private void Remove(Task task)
        {
            m_TaskIdMap.Remove(task.id);
            m_TaskActionMap.Remove(task.action);
            task.action = null;
            task.destroyed = true;
            if (!task.running && task.position >= 0)
            {
                var list = task.ignoreTimeScale ? m_TaskUnscaledList : m_TaskList;
                RemoveTaskAt(list, task.position);
                m_TaskPool.Release(task);
            }
        }
        #endregion

        static private void InsertTask(List<Task> list, Task task)
        {
            list.Add(task);

            int c = list.Count - 1;
            while (c > 0)
            {
                int p = (c - 1) / 2;
                var parent = list[p];
                if (parent < task)
                {
                    break;
                }
                else
                {
                    list[c] = parent;
                    parent.position = c;
                    c = p;
                }
            }
            list[c] = task;
            task.position = c;
        }

        static private void RemoveTaskAt(List<Task> list, int index)
        {
            int n = list.Count - 1;
            if (n >= index)
            {
                list[index] = list[n];
                list.RemoveAt(n);
                ResortTaskAt(list, index);
            }
        }
        static private void ResortTaskAt(List<Task> list, int index)
        {
            int end = list.Count - 1;
            if (end < index)
                return;

            int c = index;
            int p = 2 * c + 1;
            var task = list[c];

            while (p <= end)
            {
                var child = list[p];
                if (p < end && child > list[p + 1])
                    child = list[++p];

                if (task < child)
                {
                    break;
                }
                else
                {
                    list[c] = child;
                    child.position = c;
                    c = p;
                    p = 2 * c + 1;
                }
            }
            list[c] = task;
            task.position = c;
        }


        public class Proxy
        {
            private static readonly ObjectPool<Proxy> m_ProxyPool = new ObjectPool<Proxy>(() => new Proxy(), null, (p) => p.RemoveAll());

            public static Proxy Get()
            {
                return m_ProxyPool.Get();
            }
            public static void Release(Proxy proxy)
            {
                m_ProxyPool.Release(proxy);
            }

            private readonly HashSet<int> m_TaskIds;

            private Proxy()
            {
                m_TaskIds = new HashSet<int>();
            }

            #region Update, Timeout, Interval, Remove
            public int Update(Action action)
            {
                return Schedule(action, 0f, 0, 0f, false);
            }
            public int Update(Action action, int repeat)
            {
                return Schedule(action, 0f, repeat, 0f, false);
            }

            public int Timeout(Action action)
            {
                return Schedule(action, 0f, 1, 0f, false);
            }
            public int Timeout(Action action, float delay)
            {
                return Schedule(action, 0f, 1, delay, false);
            }
            public int Timeout(Action action, float delay, bool ignoreTimeScale)
            {
                return Schedule(action, 0f, 1, delay, ignoreTimeScale);
            }

            public int Interval(Action action, float interval)
            {
                return Schedule(action, interval, 0, 0f, false);
            }
            public int Interval(Action action, float interval, bool ignoreTimeScale)
            {
                return Schedule(action, interval, 0, 0f, ignoreTimeScale);
            }
            public int Interval(Action action, float interval, int repeat)
            {
                return Schedule(action, interval, repeat, 0f, false);
            }
            public int Interval(Action action, float interval, int repeat, bool ignoreTimeScale)
            {
                return Schedule(action, interval, repeat, 0f, ignoreTimeScale);
            }
            public int Interval(Action action, float interval, int repeat, float delay)
            {
                return Schedule(action, interval, repeat, delay, false);
            }
            public int Interval(Action action, float interval, int repeat, float delay, bool ignoreTimeScale)
            {
                return Schedule(action, interval, repeat, delay, ignoreTimeScale);
            }

            private int Schedule(Action action, float interval, int repeat, float delay, bool ignoreTimeScale)
            {
                int taskId = Scheduler.Schedule(action, interval, repeat, delay, ignoreTimeScale);
                m_TaskIds.Add(taskId);
                return taskId;
            }

            public bool Remove(ref int taskId)
            {
                if (m_TaskIds.Remove(taskId))
                {
                    return Scheduler.Remove(ref taskId);
                }
                return false;
            }

            public bool Remove(Action action)
            {
                Task t = null;
                if (m_TaskActionMap.TryGetValue(action, out t))
                {
                    if (m_TaskIds.Remove(t.id))
                    {
                        Scheduler.Remove(t);
                    }
                    return true;
                }
                return false;
            }
            public void RemoveAll()
            {
                if (m_TaskIds.Count > 0)
                {
                    using (var it = m_TaskIds.GetEnumerator())
                    {
                        while (it.MoveNext())
                        {
                            int id = it.Current;
                            Scheduler.Remove(ref id);
                        }
                    }
                    m_TaskIds.Clear();
                }
            }
            #endregion
        }

        // MonoBehaviour
        class SchedulerBehaviour : MonoBehaviour
        {
            void Update()
            {
                int n = m_TaskUpdateList.Count;
                while (n-- > 0 && m_TaskUpdateList.Count > 0)
                {
                    Task t = m_TaskUpdateList.Dequeue();
                    if (!t.destroyed)
                    {
                        t.Run();
                        if (!t.destroyed)
                        {
                            m_TaskUpdateList.Enqueue(t);
                            continue;
                        }
                    }
                    m_TaskPool.Release(t);
                }

                ExecuteTaskList(m_TaskList, Time.time);
                ExecuteTaskList(m_TaskUnscaledList, Time.unscaledTime);
            }

            void ExecuteTaskList(List<Task> list, float time)
            {
                while (list.Count > 0)
                {
                    Task t = list[0];
                    if (!t.destroyed)
                    {
                        if (time < t.time)
                            break;
                        t.Run();
                        if (!t.destroyed)
                        {
                            if (t.interval == 0)
                            {
                                RemoveTaskAt(list, 0);
                                t.position = -1;
                                m_TaskUpdateList.Enqueue(t);
                            }
                            else
                            {
                                t.time = t.time + t.interval;
                                ResortTaskAt(list, 0);
                            }
                            continue;
                        }
                    }
                    RemoveTaskAt(list, 0);
                    m_TaskPool.Release(t);
                }
            }

        }
    }
}