//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
namespace Xaz
{
    //[LuaCallCSharp]
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIRect : MaskableGraphic
	{
		protected UIRect()
		{
			useLegacyMeshGeneration = false;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}
	}
}
