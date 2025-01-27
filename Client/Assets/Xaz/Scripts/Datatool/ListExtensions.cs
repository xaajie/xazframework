using System;
using System.Collections.Generic;

public static class ListExtensions
{
    private static Random rng = new Random();

    /// <summary>
    /// �����˳����б��е�Ԫ�ؽ�������
    /// </summary>
    /// <typeparam name="T">�б���Ԫ�ص����͡�</typeparam>
    /// <param name="list">Ҫ������б�</param>
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
    /// �����б��е�Ԫ�ص�һ���µ��б�
    /// </summary>
    /// <typeparam name="T">�б���Ԫ�ص����͡�</typeparam>
    /// <param name="list">Ҫ���Ƶ��б�</param>
    /// <returns>�µ��б�����ԭ�б��е�����Ԫ�ء�</returns>
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
