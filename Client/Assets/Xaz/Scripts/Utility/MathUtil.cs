//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 数学工具类
/// 用来存放各种公用的工具方法的量
/// </summary>

public class MathUtil
{
    //权重，所有情况都必须列入
    public static int GetRandomByWeight(List<int> weightList)
    {
        int sumWe = 0;
        for (int i = 0; i < weightList.Count; i++)
        {
            sumWe = sumWe + weightList[i];
        }
        int rand = Random.Range(0, sumWe);
        for (int i = 0; i < weightList.Count; i++)
        {
            if (rand <= weightList[i])
            {
                return i;
            }
            else
            {
                rand = rand - weightList[i];
            }
        }
        return 0;
    }

    public static int GetRandomByRatio(List<int> weightList)
    {
        int rand = Random.Range(0, 100);
        for (int i = 0; i < weightList.Count; i++)
        {
            if (rand <= weightList[i])
            {
                return i;
            }
            else
            {
                rand = rand - weightList[i];
            }
        }
        return -1;
    }

    //public static Vector3 GetClosestPointOnLine(Vector3 a, Vector3 b, float distance)
    //{
    //    Vector3 pos = Vector3.zero;
    //    Vector3 vector = (b - a).normalized;
    //    pos = vector * distance;
    //    pos = b + pos;
    //    return pos;
    //}
    public static Vector3 GetRandomPointInCircle(Vector3 center, float radius)
    {
        // 生成两个-1到1之间的随机数
        float randomX = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f, 1f);

        // 将这两个随机数作为单位向量，然后缩放到半径长度
        Vector3 randomDirection = new Vector3(randomX, 0, randomZ).normalized * radius;

        // 将方向向量加到圆心坐标上，得到最终的随机点坐标
        Vector3 randomPosition = center + randomDirection;

        // 如果需要，可以将生成的点限制在XZ平面上
        randomPosition.y = center.y;

        return randomPosition;
    }

    // 计算点到线段的最近点
    public static Vector2 ClosestPointOnLineSegment(Vector2 point, Vector2 linea, Vector2 lineb)
    {
        Vector2 ap = point - linea;
        Vector2 ab = lineb - linea;
        float t = Vector2.Dot(ap, ab) / Vector2.Dot(ab, ab);
        // 在线段上的投影点
        Vector2 projection = linea + (t < 0f ? Vector2.zero : t > 1f ? ab : t * ab);
        return projection;
    }
    /// <summary>
    /// 判断一个点是否在多边形内
    public static bool PolyContainsPoint(Vector2[] polyPoints, Vector2 p)
    {
        int j = polyPoints.Length - 1;
        bool inside = false;

        for (int i = 0; i < polyPoints.Length; i++)
        {
            if (((polyPoints[i].y < p.y && p.y <= polyPoints[j].y) || (polyPoints[j].y < p.y && p.y <= polyPoints[i].y)) &&
                (polyPoints[i].x + (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) * (polyPoints[j].x - polyPoints[i].x)) > p.x)
                inside = !inside;
            j = i;
        }
        return inside;
    }
    /// <summary>
    /// 两个多边形是否相交
    /// </summary>
    public static bool PolyContainsPoints(Vector2[] polyPoints, Vector2[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (PolyContainsPoint(polyPoints, points[i]))
                return true;
        }
        for (int i = 0; i < polyPoints.Length; i++)
        {
            if (PolyContainsPoint(points, polyPoints[i]))
                return true;
        }
        return false;
    }
}
