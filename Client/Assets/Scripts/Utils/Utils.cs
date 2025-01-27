//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Xaz;
/// <summary>
/// 工具类
/// 用来存放各种公用的工具方法的量
/// </summary>

public class Utils
{
    /// <summary>
    /// 获取多语言文本
    /// 来源1.表  2txt文件
    /// author xiejie
    /// </summary>
    /// <param name="key">文本key</param>
    /// <returns></returns>
    static public string GetLang(string key)
    {
        return Localization.Get(key);
    }

    public static bool IsEditor()
    {
#if UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }

    public static bool IsEidtorOsx()
    {
#if UNITY_EDITOR_OSX
           return true;
#else
        return false;
#endif
    }

    static public Transform FindChildByName(Transform root, string name)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name == name)
                return child;
            if (child.childCount > 0)
            {
                child = FindChildByName(child, name);
                if (child != null)
                    return child;
            }
        }
        return null;
    }

    static public void SetParentAndReset(Transform t, Transform parentTf)
    {
        if (t != null && parentTf != null)
        {
            t.SetParent(parentTf);
            ResetLocalTr(t);
        }
    }

    /// <summary>
    /// 设置父结点,并且保持自己的放大锁小的倍率
    /// </summary>
    /// <param name="t">T.</param>
    /// <param name="parentTf">Parent tf.</param>
    static public void SetParentAndKeepScale(Transform t, Transform parentTf)
    {
        if (t != null && parentTf != null)
        {
            t.SetParent(parentTf);
            t.localPosition = Vector3.zero;
            t.localEulerAngles = Vector3.zero;
        }
    }

    static public string GetPlatform()
    {
#if UNITY_STANDALONE
        return "Win";
#elif (UNITY_IOS || UNITY_IPHONE)
		return "IOS";
#elif UNITY_ANDROID
        return "Android";
#else
        return string.Empty;
#endif
    }


    static public GameObject AddChild(Component parent, GameObject prefab)
    {
        GameObject go;
        if (prefab != null)
        {
            go = GameObject.Instantiate(prefab);
            go.name = prefab.name;
        }
        else
        {
            go = new GameObject("GameObject");
        }
        if (parent != null)
        {
            go.transform.SetParent(parent.transform);
            if (parent.GetComponent<RectTransform>() != null)
            {
                if (!(go.transform is RectTransform))
                    go.AddComponent<RectTransform>();
            }
        }
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.Euler(Vector3.zero);
        go.transform.localScale = Vector3.one;
        return go;
    }

    /// <summary>
    /// 顺时针旋转
    /// </summary>
    /// <param name="v"></param>
    /// <param name="cos, sin">旋转角的cos，sin</param>
    /// <returns></returns>
    static public Vector2 Vec2Rotate(Vector2 v, float cos, float sin)
    {

        float x = v.x * cos + v.y * sin;
        float y = -v.x * sin + v.y * cos;

        Vector2 v1 = new Vector2(x, y);
        return v1;
    }


    static public float TerrainSampleHeightByPos(Terrain terrain, float x, float z)
    {
        return terrain.SampleHeight(new Vector3(x, 0, z));
    }

    /// <summary>
    /// 初始状态 lua端GVector3传入参数，会坐标异常，不建议使用，都改用TerrainSampleHeightByPos
    /// </summary>
    /// <returns></returns>
    static public float TerrainSampleHeight(Terrain terrain, Vector3 pos)
    {
        return terrain.SampleHeight(pos);
    }

    static public void SetLayerAll(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        Transform[] trans = root.GetComponentsInChildren<Transform>();
        foreach (Transform child in trans)
        {
            child.gameObject.layer = layer;
        }
    }

    /// <summary>
    /// 设置坐标
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    static public void SetPos(GameObject obj, float x, float y, float z)
    {
        obj.transform.position = new Vector3(x, y, z);
    }

    /// <summary>
    /// 设置EventSystem是否开启
    /// </summary>
    static public void EnableEventSystem(bool enabled)
    {
        GameObject evnetSystemGo = GameObject.Find("EventSystem");
        if (evnetSystemGo != null)
        {
            UnityEngine.EventSystems.EventSystem eventSystem = evnetSystemGo.GetComponent<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem != null)
                eventSystem.enabled = enabled;
        }
    }

    /// <summary>
    /// 设置Gameojbect激活或者不激活
    /// 封装的方法，提供给LUA用减少调用次数
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="isActive">If set to <c>true</c> is active.</param>
    public static void SetActive(GameObject obj, bool isActive)
    {
        if (obj != null)
        {
            if (isActive)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                }
            }
            else
            {
                if (obj.activeSelf)
                {
                    obj.SetActive(false);
                }
            }
        }
        else
        {
            // Debug.Log("Error Set GameObject active is null!");
        }
    }

    /// <summary>
    /// 设置Gameojbect激活或者不激活
    /// 封装的方法，提供给LUA用减少调用次数
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="isActive">If set to <c>true</c> is active.</param>
    public static void SetTransformActive(Transform tr, bool isActive)
    {
        if (tr != null)
        {
            GameObject obj = tr.gameObject;
            SetActive(obj, isActive);

        }
        else
        {
            // Debug.Log("Error Set Transform active is null!");
        }
    }

    /// <summary>
    /// 设置父物体
    /// </summary>
    /// <param name="child">子变换.</param>
    /// <param name="parent">父变换</param>
    /// <param name="isReset">是否用父对象的参数设置子变换</param>
    public static void SetParent(Transform child, Transform parent, bool isReset)
    {
        child.SetParent(parent);
        if (isReset)
        {
            ResetLocalTr(child);
        }
    }

    /// <summary>
    /// 重置变换的局部参数
    /// </summary>
    /// <param name="tr">变换.</param>
    public static void ResetLocalTr(Transform tr)
    {
        if (tr != null)
        {
            tr.localScale = Vector3.one;
            tr.localPosition = Vector3.zero;
            tr.localEulerAngles = Vector3.zero;
        }
    }

    /// <summary>
    /// 根据路径返回文件名
    /// </summary>
    /// <returns>文件名.</returns>
    /// <param name="path">路径</param>
    public static string GetFileName(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            //Debug.Log("Effor path is null !");
            return null;
        }
        path.Replace('\\', '/');
        int index = path.LastIndexOf("/");
        if (index >= path.Length)
        {
            index = path.Length;
        }
        return path.Substring(index + 1);
    }

    public static Vector3 MatrixMul(Matrix4x4 matrix, Vector3 v)
    {
        Vector3 result = matrix.MultiplyVector(v);
        result.y = 0;
        return result.normalized;
    }

    /// <summary>
    /// 旋转向量
    /// </summary>
    /// <returns>旋转后的向量 v3.</returns>
    /// <param name="v1">要旋转的向量.</param>
    /// <param name="angle">旋转的角度.</param>
    public static Vector3 GetRotateV3(Vector3 v, float angle)
    {
        angle = angle * Mathf.PI / 180;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        float x = v.x * cos + v.z * sin;
        float z = -v.x * sin + v.z * cos;
        Vector3 obj = new Vector3(x, 0, z);
        return obj;
    }


    //当前是否有输入键盘显示
    public static bool KeyBoardVisible()
    {
#if UNITY_IOS
			return TouchScreenKeyboard.visible;
#elif UNITY_ANDROID
        return TouchScreenKeyboard.visible;
#else
        return false;
#endif
    }

    /// <summary>
    /// 通过十六进制颜色初始化一个Color
    /// </summary>
    /// <returns>The color form hex.</returns>
    /// <param name="hex">Hex.</param>
    /// <param name="isAlpha">If set to <c>true</c> 是否是RGBA格式的默认为RGB格式.</param>
    public static Color GetColorFormHex(long hex, bool isAlpha = false)
    {
        Color color = Color.white;
        if (isAlpha)
        {
            float r = (hex & 0xff000000) >> 24;
            float g = (hex & 0xff0000) >> 16;
            float b = (hex & 0xff00) >> 8;
            float a = hex & 0xff;
            color = new Color(r, g, b, a);
        }
        else
        {
            float r = (hex & 0xff0000) >> 16;
            float g = (hex & 0xff00) >> 8;
            float b = hex & 0xff;
            color = new Color(r, g, b);
        }
        return color;
    }


    /// <summary>
    /// 设置手机震动
    /// </summary>
    public static void ShowPhoneVibrate()
    {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }



    /// <summary>
    /// 获取电量
    /// </summary>
    public static float GetBatteryLevel()
    {
        return SystemInfo.batteryLevel;
    }

    /// <summary>
    /// 返回所有子对象的变换
    /// </summary>
    /// <returns>变换数组.</returns>
    /// <param name="tr">父变换.</param>
    public static Transform[] GetTrChilds(Transform tr)
    {
        Transform[] obj = null;
        if (tr != null)
        {
            obj = new Transform[tr.childCount];
            for (int i = 0; i < tr.childCount; i++)
            {
                obj[i] = tr.GetChild(i);
            }
        }
        else
        {
            Debug.Log("Error : this Transform is null");
        }
        return obj;
    }

    /// <summary>
    /// 释放场景
    /// </summary>
    /// <returns><c>true</c>, if load scene name was uned, <c>false</c> otherwise.</returns>
    /// <param name="name">Name.</param>
    public static void UnLoadSceneName(string name)
    {
        SceneManager.UnloadSceneAsync(name);
    }

    public static float GetAnimClipLen(Animator anim, string clipName)
    {
        if (anim != null)
        {
            RuntimeAnimatorController ac = anim.runtimeAnimatorController;
            for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
            {
                if (ac.animationClips[i].name == clipName)        //If it has the same name as your clip
                {
                    return ac.animationClips[i].length;
                }
            }
        }
        return 0f;
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public static void PlayAnimation(Transform tr)
    {
        Animation anim = tr.GetComponent<Animation>();
        if (anim)
        {
            anim.Play();
        }
    }

    /// <summary>
    ///  修改材质颜色
    /// </summary>
    /// <param name="tr"></param>
    public static void SetMaterialColor(Transform tr, string shaderColorName, bool isRetainAlpha, Color c)
    {
        if (tr)
        {
            MeshRenderer material = tr.GetComponent<MeshRenderer>();
            if (material)
                material.material.SetColor(shaderColorName, isRetainAlpha ? new Color(c.r, c.g, c.b, material.material.GetColor(shaderColorName).a) : c);
        }
    }

    /// <summary>
    /// 修改材质
    /// </summary>
    /// <param name="tr"></param>
    public static void SetMaterial(Transform tr, string path_root, string materialName)
    {
        if (tr)
        {
            Material ma = ResMgr.LoadByPath(path_root + materialName, typeof(Material)) as Material;
            if (ma)
            {
                MeshRenderer material = tr.GetComponent<MeshRenderer>();
                if (material)
                    material.material = ma;
            }
        }
    }


    /// <summary>
    /// 返回陀螺仪的旋转值
    /// </summary>
    public static Vector2 GetGyroAttitude()
    {
        float x = 0f;
        float y = 0f;
        if (SystemInfo.supportsGyroscope)
        {
            x = Input.gyro.gravity.x;
            y = Input.gyro.gravity.y;
        }

        return new Vector2(x, y);
    }


    public static int GetMask(int[] maskLayers)
    {
        int ret = 0;
        for (int i = 0; i < maskLayers.Length; i++)
        {
            ret |= 1 << maskLayers[i];
        }
        //Debug.Log("===============ret=" + ret);
        return ret;

    }

    //设置光的cull Mask
    public static void SetLightMask(GameObject go, int[] maskLayers)
    {
        if (go != null)
        {
            Light light = go.GetComponent<Light>();
            if (light != null)
            {
                int mask = GetMask(maskLayers);
                light.cullingMask = mask;
            }
        }
    }

    public static SkinnedMeshRenderer GetSkinnedMeshRendererInChild(Transform root)
    {
        int childCount = root.childCount;
        Transform child = null;
        SkinnedMeshRenderer renderer = null;
        for (int i = 0; i < childCount; i++)
        {
            child = root.GetChild(i);
            renderer = child.GetComponent<SkinnedMeshRenderer>();
            if (renderer != null)
                break;
        }
        return renderer;
    }

    public static Transform GetChildByPartName(Transform root, string partName)
    {
        Transform child = null;
        if (root != null)
        {
            int childCount = root.childCount;
            for (int i = 0; i < childCount; i++)
            {
                child = root.GetChild(i);
                if (child.name.Contains(partName))
                    break;
                else
                    child = null;
            }
        }
        return child;
    }


    public static GameObject CreateColliderObject(string name, int layer, GameObject parent, Vector3 pos,
    Quaternion quat)
    {
        GameObject go = new GameObject(name);
        go.layer = layer;
        go.transform.SetParent(parent.transform);
        go.transform.position = pos;
        go.transform.rotation = quat;
        return go;
    }

    //增加碰撞组件方法
    public static void AddCapsuleCollider(GameObject go, bool isTrigger, float radius, float height)
    {
        CapsuleCollider capsuleCollider = go.AddComponent<CapsuleCollider>();
        capsuleCollider.isTrigger = isTrigger;
        capsuleCollider.radius = radius;
        capsuleCollider.height = height;
    }

    public static void AddBoxCollider(GameObject go, bool isTrigger, float x, float y, float z)
    {
        BoxCollider boxCollider = go.AddComponent<BoxCollider>();
        boxCollider.isTrigger = isTrigger;
        boxCollider.size = new Vector3(x, y, z);
    }

    /// <summary>
    /// 发射射线返回射到的物体
    /// </summary>
    /// <returns>The ray cast all.</returns>
    /// <param name="res">Res.</param>
    /// <param name="dir">Dir.</param>
    /// <param name="distance">Distance.</param>
    /// <param name="mask">Mask.</param>
    public static RaycastHit[] RayCastAll(Vector3 res, Vector3 dir, float distance, int mask)
    {
        return Physics.RaycastAll(res, dir, distance, mask);
    }

    /// <summary>
    /// 判断V3相等的小数点缩放级别
    /// </summary>
    public const int COMPAIR_SCALE = 1000;

    /// <summary>
    /// 对此两个V3是否相等
    /// </summary>
    /// <param name="v1">V1.</param>
    /// <param name="v2">V2.</param>
    public static bool CompairV3(Vector3 v1, Vector3 v2)
    {
        long[] tempV1 = GetLongFormV3(v1);
        long[] tempV2 = GetLongFormV3(v2);
        for (int i = 2; i >= 0; i--)
        {
            if (tempV1[i] != tempV2[i])
            {
                return false;
            }
        }
        return true;
    }

    static long[] GetLongFormV3(Vector3 v3)
    {
        long[] temp = new long[3];
        temp[0] = (long)(v3.x * COMPAIR_SCALE);
        temp[1] = (long)(v3.y * COMPAIR_SCALE);
        temp[2] = (long)(v3.z * COMPAIR_SCALE);
        return temp;
    }

    /// <summary>
    /// color 转换hex color转换成00FFF4FF格式xj
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        int a = Mathf.RoundToInt(color.a * 255.0f);
        string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
        return hex;
    }

    /// <summary>
    /// hex转换到color 00FFF4FF 转换成 Color
    /// </summary>addby xiejie
    /// <param name="hex"></param>
    /// <returns></returns>
    public static Color HexToColor(string hex)
    {
        if (hex.IndexOf("#") != -1)
        {
            hex = hex.Replace("#", "");
        }
        if (hex.Length < 9)
        {
            hex = hex + "FF";
        }
        byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        float r = br / 255f;
        float g = bg / 255f;
        float b = bb / 255f;
        float a = cc / 255f;
        return new Color(r, g, b, a);
    }

    public static bool CheckClickInTarget(GameObject target)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        List<RaycastResult> raylist = new List<RaycastResult>();
        eventData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(eventData, raylist);
        foreach (var item in raylist)
        {
            if (item.gameObject != null)
            {
                if (item.gameObject == target)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool CheckClickInTarget(GameObject target,out PointerEventData pos)
    {
        pos = null;
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        List<RaycastResult> raylist = new List<RaycastResult>();
        eventData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(eventData, raylist);
        foreach (var item in raylist)
        {
            if (item.gameObject != null)
            {
                if (item.gameObject == target)
                {
                    pos = eventData;
                    return true;
                }
            }
        }
        return false;
    }

    public static int GetDayNum(int num, int stime)
    {
        if (TimeUtil.IsSameDay(stime))
        {
            return num;
        }
        else
        {
            return 0;
        }
    }

    public static void SetSillDesc(UIRichText comp, string content)
    {
        comp.AddClickListener(UIRichText.SPEC_CATEGORY, (parm, go) =>
        {
            UserCategoryData vt = ModuleMgr.CategoryMgr.CreateFrom(parm);
            //ModuleMgr.TipMgr.ShowTip(vt.GetItemType(), vt.GetID(), vt, null, go);
        });
        comp.text = content;
    }
}
/// <summary>
/// 空类型的回调
/// </summary>
public delegate void VoidDelegate();
public delegate void IntDelegate(int index);
public delegate void BoolDelegate(bool isCan);
public delegate void ObjDelegate(GameObject obj, string path);
