using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.NURBS {
    [CreateAssetMenu(menuName = "ControlPoints/Spline", fileName = "SplineCpsData")]
    public class SplineCpsData : ScriptableObject {
        public int order;
        public SplineType type;
        public List<CP> cps;
    }
}
