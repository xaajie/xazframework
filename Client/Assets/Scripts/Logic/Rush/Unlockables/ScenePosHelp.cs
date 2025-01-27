using System.Collections.Generic;
using UnityEngine;
public class ScenePosHelp : MonoBehaviour
{
    [SerializeField] public List<Vector3> points = new List<Vector3>();
    [SerializeField] public Vector3 spawnPoint;
    [SerializeField] public Vector3 exitPoint;
    [SerializeField] public Vector3 spawnPoint0;
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.color = Color.magenta;
            var center = transform.TransformPoint(points[i]);
            var size = new Vector3(2f, 0.2f, 2f);
            Gizmos.DrawWireCube(center, size);
            //Gizmos.DrawCube(center, size);
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.black;
            UnityEditor.Handles.Label(center + Vector3.up * 0.5f, "Point"+i, style);
        }

    }

    void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(spawnPoint, 0.7f);
        }
        if (spawnPoint0 != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(spawnPoint0, 0.7f);
        }
        if (exitPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(exitPoint, 0.7f);
        }
    }
#endif
}

