using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.NURBS {
    [CreateAssetMenu(menuName = "kmty/Create ControlPointsData/Spline", fileName = "SplineCpsData")]
    public class SplineCpsData : ScriptableObject {
        public int order;
        public SplineType type;
        public List<CP> cps;
    }
}
