using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
public static class TerrainUtil
{
    public static T[,] Concat0<T>(this T[,] array_0, T[,] array_1)
    {
        if (array_0.GetLength(0) != array_1.GetLength(0))
        {
            Debug.LogError("两个数组第一维不一致");
            return null;
        }
        T[,] ret = new T[array_0.GetLength(0), array_0.GetLength(1) + array_1.GetLength(1)];
        for (int i = 0; i < array_0.GetLength(0); i++)
        {
            for (int j = 0; j < array_0.GetLength(1); j++)
            {
                ret[i, j] = array_0[i, j];
            }
        }
        for (int i = 0; i < array_1.GetLength(0); i++)
        {
            for (int j = 0; j < array_1.GetLength(1); j++)
            {
                ret[i, j + array_0.GetLength(1)] = array_1[i, j];
            }
        }
        return ret;
    }

    public static T[,] Concat1<T>(this T[,] array_0, T[,] array_1)
    {
        if (array_0.GetLength(1) != array_1.GetLength(1))
        {
            Debug.LogError("两个数组第二维不一致");
            return null;
        }
        T[,] ret = new T[array_0.GetLength(0) + array_1.GetLength(0), array_0.GetLength(1)];
        for (int i = 0; i < array_0.GetLength(0); i++)
        {
            for (int j = 0; j < array_0.GetLength(1); j++)
            {
                ret[i, j] = array_0[i, j];
            }
        }
        for (int i = 0; i < array_1.GetLength(0); i++)
        {
            for (int j = 0; j < array_1.GetLength(1); j++)
            {
                ret[i + array_0.GetLength(0), j] = array_1[i, j];
            }
        }
        return ret;
    }

    public static T[,] GetPart<T>(this T[,] array, int base_0, int base_1, int length_0, int length_1)
    {
        if (base_0 + length_0 > array.GetLength(0) || base_1 + length_1 > array.GetLength(1))
        {
            Debug.Log(base_0 + length_0 + ":" + array.GetLength(0));
            Debug.Log(base_1 + length_1 + ":" + array.GetLength(1));
            Debug.LogError("索引超出范围");
            return null;
        }
        T[,] ret = new T[length_0, length_1];
        for (int i = 0; i < length_0; i++)
        {
            for (int j = 0; j < length_1; j++)
            {
                ret[i, j] = array[i + base_0, j + base_1];
            }
        }
        return ret;
    }

    /// <summary>
    /// 通过反射和函数名调用非公有方法
    /// </summary>
    /// <param name="obj">目标对象</param>
    /// <param name="methodName">函数名</param>
    /// <param name="objs">参数数组</param>
    public static void Invoke(this object obj, string methodName, params object[] objs)
    {
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
        Type type = obj.GetType();
        MethodInfo m = type.GetMethod(methodName, flags);
        m.Invoke(obj, objs);
    }
}
