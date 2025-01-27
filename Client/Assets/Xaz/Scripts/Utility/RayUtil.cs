//----------------------------------------------------------------------------
//--射线检测的封装工具
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RayUtil
{

    private static Ray ray = new Ray(Vector3.zero, Vector3.down);
    private static float MAX_RAY_DIST = 400;
    //地形射线
    private static int mappathLayerMaskId = -1;
    private static float initheight = -200;
    //通过layername，返回值用于设置cullingMask
    public static int GetMask(string[] maskLayers)
    {
        int ret = 0;
        for (int i = 0; i < maskLayers.Length; i++)
        {
            ret |= 1 << LayerMask.NameToLayer(maskLayers[i]);
        }
        return ret;

    }

    //判断UI某个界面是否处于隐藏状态
    static public bool IsUIHide(Transform ui)
    {
        return (ui && ui.gameObject.layer == LayerMask.NameToLayer(XazConfig.LayerDefine.UIINVISIBLE));
    }

    /// <summary>
    /// 只设当前层的layer
    /// </summary>
    /// <param name="go"></param>
    /// <param name="layername"></param>
    public static void SetOneLayer(GameObject go, string layername)
    {
        go.gameObject.layer = LayerMask.NameToLayer(layername);
    }

    public static void SetLayer(GameObject go, string layername)
    {
        SetOneLayer(go, layername);
        XazHelper.SetLayer(go, LayerMask.NameToLayer(layername));
    }

    public static void SetUILayer(GameObject go, string layername, bool enableRay)
    {
        XazHelper.SetLayer(go, LayerMask.NameToLayer(layername));
        GraphicRaycaster gr = go.GetComponent<GraphicRaycaster>();
        if (gr)
        {
            gr.enabled = enableRay;
        }
    }

    public static void SetLayerById(GameObject go, int layerId)
    {
        XazHelper.SetLayer(go, layerId);
    }

    private static Vector3 post = Vector3.zero;
    private static int terrainId;

    public static void SetMapPathCheckLayer(string[] layername, string terrainLayer)
    {
        terrainId = LayerMask.NameToLayer(terrainLayer);
        mappathLayerMaskId = GetMask(layername);
    }

    public static float GetRayHitLayer(float x, float y, float z, int layerIds, out int layerN)
    {
        float height = initheight;
        layerN = terrainId;
        height = Mathf.Max(initheight, y, TerrainManager.TerrainSampleHeightByPos(x, z));
        // Debug.Log(height+ "layernameheight");
        if (layerIds > 0)
        {
            int layername = terrainId;
            post.Set(x, height + 30, z);
            ray.origin = post;
            RaycastHit hit;
            float nY = 0;
            // Debug.Log(height + "layernameheight" + post.ToString());
            if (Physics.Raycast(ray, out hit, MAX_RAY_DIST, layerIds))
            {
                layername = hit.transform.gameObject.layer;
                // Debug.Log("layername" + LayerMask.LayerToName(layername));
                nY = hit.point.y;
            }
            if (nY >= height)
            {
                height = nY;
                layerN = layername;
            }
            //Debug.Log(height + "layername" + nY+ LayerMask.LayerToName(layerN));
        }
        return height;
    }

    /// <summary>
    /// 获得可行走面的实际着地高度
    /// </summary>
    public static float GetMapPathHeight(float x, float y, float z, out int layerN)
    {
        return GetRayHitLayer(x, y, z, mappathLayerMaskId, out layerN);
    }

    public static float TerrainSampleHeightByPos(Terrain terrain, float x, float z)
    {
        return terrain.SampleHeight(new Vector3(x, 0, z));
    }

    private static Vector3 checkpos = Vector3.zero;
    public static List<Transform> GetRayHitTargetAll(float x, float z, float orginy, float distantce, int layerMask)
    {
        checkpos.Set(x, orginy, z);
        RaycastHit[] raycastHit = Physics.RaycastAll(checkpos, Vector3.down, distantce, layerMask);
        // RaycastHit[] raycastHit = Physics.RaycastAll(new Vector3(x, MAX_RAY_DIST, z), Vector3.down, MAX_RAY_DIST, layerMask);
        List<Transform> nlist = new List<Transform>();
        for (int i = 0; i < raycastHit.Length; i++)
        {
            nlist.Add(raycastHit[i].transform);
        }
        return nlist;
    }

    /// <summary>
    /// 鼠标点射线到指定layer距离范围内的所有物体
    /// </summary>
    public static RaycastHit[] RaycastFromScreenToWorldByMousePos(int layerMask, float rayCastDistance)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.RaycastAll(ray, rayCastDistance, layerMask);
    }
}