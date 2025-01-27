//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIParticleLayout : MonoBehaviour 
{
    void Start()
    {
        float width = (float)Screen.width;
        float height = (float)Screen.height;
        float designWidth = 1136;//开发时分辨率宽
        float designHeight = 640;//开发时分辨率高
        float s1 = designWidth / designHeight;
        float s2 = width / height;
        float scale = (s2/s1);
        foreach(ParticleSystem particle in GetComponentsInChildren<ParticleSystem>())
        {
            float x =   particle.transform.localScale.x *scale;
            particle.transform.localScale = new Vector3(x,particle.transform.localScale.y,particle.transform.localScale.z);
         }
    }
}



