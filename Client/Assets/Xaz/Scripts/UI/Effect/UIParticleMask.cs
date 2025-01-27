//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
[AddComponentMenu("UI/UIParticleMask")]
public class UIParticleMask :Mask 
{
    const string PATH = "Shader/UIMask/";
    protected override void Start()
    {
        base.Start();
        if(Application.isPlaying)
        {
            Image image = GetComponent<Image>();
            image.material = new Material(Resources.Load<Shader>( PATH + "UI-Mask"));
            foreach( ParticleSystemRenderer system in  transform.GetComponentsInChildren<ParticleSystemRenderer>(true))
            {
                SetRenderer(system);
            }

            foreach( TrailRenderer trail in  transform.GetComponentsInChildren<TrailRenderer>(true))
            {
                SetRenderer(trail);
            }
        }
    }


    void SetRenderer(Renderer renderer)
    {
        Shader shader =  Resources.Load<Shader>(PATH + renderer.sharedMaterial.shader.name);
        if(shader)
        {
            #if UNITY_EDITOR
            renderer.material.shader = shader;
            #else
            renderer.sharedMaterial.shader = shader;
            #endif
        }else
        {
            Debug.LogWarning(string.Format("{0}  的材质  {1} : 不支持的UI特效裁切 {2}",gameObject.name,renderer.sharedMaterial.name, renderer.sharedMaterial.shader.name));
        }
    }

}
