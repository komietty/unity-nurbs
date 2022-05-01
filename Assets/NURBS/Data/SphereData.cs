using UnityEngine;

namespace kmty.NURBS {
    [CreateAssetMenu(menuName = "ControlPoints/Sphere", fileName = "SphereData")]
    public class SphereData : SurfaceCpsData {

        public override bool GetXLoop() { return true; }
        public override bool GetYLoop() { return false; }
        public override KnotType GetXKnot() { return KnotType.Uniform; }
        public override KnotType GetYKnot() { return KnotType.OpenUniform; }
        public override SplineType GetXtype() { return SplineType.Loop; }
        public override SplineType GetYtype() { return SplineType.Clamped; }
    }
}
