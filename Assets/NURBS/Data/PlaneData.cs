using UnityEngine;

namespace kmty.NURBS {
    [CreateAssetMenu(menuName = "ControlPoints/Plane", fileName = "PlaneData")]
    public class PlaneData : SurfaceCpsData {
        [SerializeField] protected SplineType xtype;
        [SerializeField] protected SplineType ytype;
        
        public override bool GetXLoop() { return xtype == SplineType.Loop; }
        public override bool GetYLoop() { return ytype == SplineType.Loop; }
        public override KnotType GetXKnot() { return xtype == SplineType.Clamped ? KnotType.OpenUniform : KnotType.Uniform; }
        public override KnotType GetYKnot() { return ytype == SplineType.Clamped ? KnotType.OpenUniform : KnotType.Uniform; }
        public override SplineType GetXtype() { return xtype; }
        public override SplineType GetYtype() { return ytype; }
    }
}