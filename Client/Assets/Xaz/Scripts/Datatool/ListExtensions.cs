using System;
using System.Collections.Generic;

public static class ListExtensions
{
    private static Random rng = new Random();

    /// <summary>
    /// 以随机顺序对列表中的元素进行排序。
    /// </summary>
    /// <typeparam name="T">列表中元素的类型。</typeparam>
    /// <param name="list">要乱序的列表。</param>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static bool ContainsItem<T>(this List<T> list, T item)
    {
        return list.Contains(item);
    }
    /// <summary>
    /// 复制列表中的元素到一个新的列表。
    /// </summary>
    /// <typeparam name="T">列表中元素的类型。</typeparam>
    /// <param name="list">要复制的列表。</param>
    /// <returns>新的列表，包含原列表中的所有元素。</returns>
    public static List<T> Copy<T>(this IList<T> list)
    {
        List<T> newList = new List<T>(list.Count);
        foreach (T item in list)
        {
            newList.Add(item);
        }
        return newList;
    }
}
