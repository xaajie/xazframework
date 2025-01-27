using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Terrain管理 只负责提供一些当前terrain的一些方法支持
/// 不用动态创建和修改，这段进行修改
/// </summary>
public class TerrainManager
{
    //private static Terrain originalTerrain;
    private static Terrain terrain;
    private static GameObject obj;
    /// <summary>
    /// 初始化细节原型组
    /// </summary>
    public static void InitDetail(Terrain newTerrain)
    {
        if (newTerrain != null)
        {
            terrain = newTerrain;
            //originalTerrain = CreateTerrain(newTerrain);
        }
    }

    public static void ResetDetail()
    {
//        if (terrain != null)
//        {
//            DetailMapCopy(originalTerrain, terrain);
//            GameObject.DestroyImmediate(obj);
//            originalTerrain = null;
//            terrain = null;
//#if UNITY_EDITOR
//            //AssetDatabase.CreateAsset(terrainData, "Assets/Terrain_Modify.asset");
//            //AssetDatabase.SaveAssets();
//            //AssetDatabase.Refresh();
//#endif
//        }
    }

    public static float TerrainSampleHeightByPos(float x, float z)
    {
        if (terrain != null)
        {
            return terrain.SampleHeight(new Vector3(x, 0, z));
        }
        return 0;
    }

    static Terrain CreateTerrain(Terrain oldT)
    {
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = oldT.terrainData.heightmapResolution;
        terrainData.baseMapResolution = oldT.terrainData.baseMapResolution;
        terrainData.size = oldT.terrainData.size;
        terrainData.alphamapResolution = oldT.terrainData.alphamapResolution;
        terrainData.SetDetailResolution(oldT.terrainData.detailResolution, oldT.terrainData.detailResolutionPerPatch);

        terrainData.detailPrototypes = oldT.terrainData.detailPrototypes;
        terrainData.treePrototypes = oldT.terrainData.treePrototypes;
        terrainData.detailPrototypes = oldT.terrainData.detailPrototypes;
        terrainData.terrainLayers = new TerrainLayer[oldT.terrainData.terrainLayers.Length];
        obj = Terrain.CreateTerrainGameObject(terrainData);
        RayUtil.SetLayer(obj, "UIHideView");
        Terrain newTerrain = obj.GetComponent<Terrain>();
        terrainData.treeInstances = new TreeInstance[0];
        DetailMapCopy(oldT, newTerrain);
#if UNITY_EDITOR
        //AssetDatabase.CreateAsset(terrainData, "Assets/Terrain_Modify.asset");
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
#endif
        return newTerrain;
    }

    static void DetailMapCopy(Terrain t, Terrain to)
    {

        to.terrainData.treeInstances = new TreeInstance[0];
        TreeInstance[] bufIns = t.terrainData.treeInstances;
        for (int i = 0; i < bufIns.Length; i++)
        {
            to.AddTreeInstance(bufIns[i]);
        }
    }

    /// <summary>
    /// 移除地形上的树，没有做多地图的处理
    /// </summary>
    /// <param name="terrain">目标地形</param>
    /// <param name="center">中心点</param>
    /// <param name="radius">半径</param>
    /// <param name="index">树模板的索引</param>
    public static void RemoveTree(Vector3 center, float radius, int index = 0)
    {
        if (terrain == null)
        {
            Debug.Log("没有terrain数据");
        }
        center -= terrain.GetPosition();     // 转为相对位置
        Vector2 v2 = new Vector2(center.x, center.z);
        v2.x /= Terrain.activeTerrain.terrainData.size.x;
        v2.y /= Terrain.activeTerrain.terrainData.size.z;

        terrain.Invoke("RemoveTrees", v2, radius / Terrain.activeTerrain.terrainData.size.x, index);
    }
    //范围内的所有的tree
    public static void RemoveAllTree(float x, float z, float radius)
    {
        if (terrain == null)
        {
            Debug.Log("没有terrain数据");
        }
        Vector3 center = new Vector3(x, TerrainSampleHeightByPos(x, z), z);
        center -= terrain.GetPosition();     // 转为相对位置
        Vector2 v2 = new Vector2(center.x, center.z);
        v2.x /= Terrain.activeTerrain.terrainData.size.x;
        v2.y /= Terrain.activeTerrain.terrainData.size.z;

        for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
        {
            terrain.Invoke("RemoveTrees", v2, radius / Terrain.activeTerrain.terrainData.size.x, i);
        }
        //terrain.Invoke("RemoveTrees", v2, radius / Terrain.activeTerrain.terrainData.size.x, 0);
    }
}