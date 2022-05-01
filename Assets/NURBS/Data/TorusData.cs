using UnityEngine;

namespace kmty.NURBS {
    [CreateAssetMenu(menuName = "ControlPoints/Torus", fileName = "TorusData")]
    public class TorusData : SurfaceCpsData {

        public override bool GetXLoop() { return true; }
        public override bool GetYLoop() { return true; }
        public override KnotType GetXKnot() { return KnotType.Uniform; }
        public override KnotType GetYKnot() { return KnotType.Uniform; }
        public override SplineType GetXtype() { return SplineType.Loop; }
        public override SplineType GetYtype() { return SplineType.Loop; }
    }
}