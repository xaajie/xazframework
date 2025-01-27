//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
// UI粒子特效层级设置脚本
// 参考脚本及原理：https://www.freesion.com/article/6511199227/
// jietodo 对比性能
//new:https://blog.csdn.net/linxinfa/article/details/116406591

//使用注意事项
//1.必须有canvas容器节点下，
//2.scalingmode 得设置成Hierarchy
//3 shader限定，（mask里的shader限定为UI/Additive）
//----------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(CanvasRenderer))]
public class UIParticle : MaskableGraphic
{
    static readonly int s_IdMainTex = Shader.PropertyToID("_MainTex");
    static readonly List<Vector3> s_Vertices = new List<Vector3>();
    [Tooltip("The ParticleSystem rendered by CanvasRenderer")]
    [SerializeField] ParticleSystem m_ParticleSystem;
    [Tooltip("The UIParticle to render trail effect")]
    [SerializeField] UIParticle m_TrailParticle;
    [HideInInspector] [SerializeField] bool m_IsTrail = false;
    private Camera uiCamera = null;
    protected override void Start()
    {
        base.Start();
        uiCamera = CameraMgr.Instance.GetUICam();
    }

    public override Texture mainTexture
    {
        get
        {
            Texture tex = null;
            if (!m_IsTrail)
            {
                var textureSheet = m_ParticleSystem.textureSheetAnimation;
                if (textureSheet.enabled && textureSheet.mode == ParticleSystemAnimationMode.Sprites && 0 < textureSheet.spriteCount)
                {
                    tex = textureSheet.GetSprite(0).texture;
                }
            }
            if (!tex && _renderer)
            {
                var mat = m_IsTrail
                    ? _renderer.trailMaterial
                    : Application.isPlaying
                        ? _renderer.material
                        : _renderer.sharedMaterial;
                if (mat && mat.HasProperty(s_IdMainTex))
                {
                    tex = mat.mainTexture;
                }
            }
            return tex ?? s_WhiteTexture;
        }
    }

    public override Material GetModifiedMaterial(Material baseMaterial)
    {
        return base.GetModifiedMaterial(_renderer ? _renderer.sharedMaterial : baseMaterial);
    }

    protected override void OnEnable()
    {
        m_ParticleSystem = m_ParticleSystem ? m_ParticleSystem : GetComponent<ParticleSystem>();
        _renderer = m_ParticleSystem ? m_ParticleSystem.GetComponent<ParticleSystemRenderer>() : null;

        _mesh = new Mesh();
        _mesh.MarkDynamic();
        CheckTrail();
        base.OnEnable();
        raycastTarget = false;

        Canvas.willRenderCanvases += UpdateMesh;
    }

    protected override void OnDisable()
    {
        Canvas.willRenderCanvases -= UpdateMesh;
        DestroyImmediate(_mesh);
        _mesh = null;
        CheckTrail();
        base.OnDisable();
    }

    protected override void UpdateGeometry()
    {
    }

    Mesh _mesh;
    ParticleSystemRenderer _renderer;

    void UpdateMesh()
    {
        try
        {
            CheckTrail();

            if (m_ParticleSystem)
            {
                if (Application.isPlaying)
                {
                    _renderer.enabled = false;
                    
                }

                bool useTransform = false;
                Matrix4x4 matrix = default(Matrix4x4);
                switch (m_ParticleSystem.main.simulationSpace)
                {
                    case ParticleSystemSimulationSpace.Local:
                        matrix =
                        Matrix4x4.Rotate(m_ParticleSystem.transform.rotation).inverse
                         * Matrix4x4.Scale(m_ParticleSystem.transform.lossyScale).inverse;
                        useTransform = true;
                        break;
                    case ParticleSystemSimulationSpace.World:
                        matrix = m_ParticleSystem.transform.worldToLocalMatrix;
                        break;
                    case ParticleSystemSimulationSpace.Custom:
                        break;
                }

                _mesh.Clear();
                if (0 < m_ParticleSystem.particleCount)
                {
                    ParticleSystemBakeMeshOptions options = useTransform ? ParticleSystemBakeMeshOptions.BakeRotationAndScale : ParticleSystemBakeMeshOptions.Default;
                    if (m_IsTrail)
                    {
                        _renderer.BakeTrailsMesh(_mesh, uiCamera, options);
                    }
                    else
                    {
                        _renderer.BakeMesh(_mesh, uiCamera, options);
                    }

                    _mesh.GetVertices(s_Vertices);
                    var count = s_Vertices.Count;
                    for (int i = 0; i < count; i++)
                    {
                        s_Vertices[i] = matrix.MultiplyPoint3x4(s_Vertices[i]);
                    }
                    _mesh.SetVertices(s_Vertices);
                    s_Vertices.Clear();
                }

                canvasRenderer.SetMesh(_mesh);
                canvasRenderer.SetTexture(mainTexture);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    void CheckTrail()
    {
        if (isActiveAndEnabled && !m_IsTrail && m_ParticleSystem && m_ParticleSystem.trails.enabled)
        {
            if (!m_TrailParticle)
            {
                m_TrailParticle = new GameObject("[UIParticle] Trail").AddComponent<UIParticle>();
                var trans = m_TrailParticle.transform;
                trans.SetParent(transform);
                trans.localPosition = Vector3.zero;
                trans.localRotation = Quaternion.identity;
                trans.localScale = Vector3.one;

                m_TrailParticle._renderer = GetComponent<ParticleSystemRenderer>();
                m_TrailParticle.m_ParticleSystem = GetComponent<ParticleSystem>();
                m_TrailParticle.m_IsTrail = true;
            }
            m_TrailParticle.enabled = true;
        }
        else if (m_TrailParticle)
        {
            m_TrailParticle.enabled = false;
        }
    }
}
