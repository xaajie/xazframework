//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
namespace Xaz
{
    #if USE_LUA
    [SLua.CustomLuaClass]
    #endif
	[AddComponentMenu("UI/Effects/Gradient")]
    public class UIGradient :  BaseMeshEffect
    {
        public Color topColor = Color.white;
        public Color bottomColor = Color.black;

        public void Refresh()
        {
            Text t = GetComponent<Text>();
            if(t){
                t.FontTextureChanged();
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive())
                return;
            
            List<UIVertex> vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);
            
            ModifyVertices(vertexList);
            
            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
        }
    
        public void ModifyVertices(List<UIVertex> vertexList)
        {
            if (!IsActive())
            {
                return;
            }
            
            int count = vertexList.Count;
            if (count > 0)
            {
                float bottomY = vertexList[0].position.y;
                float topY = vertexList[0].position.y;
                
                for (int i = 1; i < count; i++)
                {
                    float y = vertexList [i] .position.y;
                    if (y > topY)
                    {
                        topY = y;
                    }
                    else if (y < bottomY)
                    {
                        bottomY = y;
                    }
                }
                
                float uiElementHeight = topY - bottomY;
                
                for (int i = 0; i < count; i++)
                {
                    UIVertex uiVertex = vertexList [i] ;
                    uiVertex.color = Color32.Lerp(bottomColor, topColor, (uiVertex.position.y - bottomY) / uiElementHeight);
                    vertexList [i] = uiVertex;
                }
            }
        }
        
        public override void ModifyMesh (Mesh mesh)
        {
            if (!IsActive ()) {
                return;
            }

            Vector3[] vertexList = mesh.vertices;
            int count = mesh.vertexCount;
            if (count > 0) {
                float bottomY = vertexList [0].y;
                float topY = vertexList [0].y;
                
                for (int i = 1; i < count; i++) {
                    float y = vertexList [i].y;
                    if (y > topY) {
                        topY = y;
                    } else if (y < bottomY) {
                        bottomY = y;
                    }
                }
                List<Color32> colors = new List<Color32> ();
                float uiElementHeight = topY - bottomY;
                for (int i = 0; i < count; i++) {
                    colors.Add (Color32.Lerp (bottomColor, topColor, (vertexList [i].y - bottomY) / uiElementHeight));
                }
                mesh.SetColors (colors);
            }
        }
    }
}