using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Common
{
    public class CutoutMask : Image
    {
        public override Material materialForRendering
        {
            get
            {
                // var mat = base.materialForRendering;
                // mat.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return base.materialForRendering;
            }
        }
    }
}