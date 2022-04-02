using UnityEngine;

namespace kmty.NURBS {
    public class SplineHandler : MonoBehaviour {
        [SerializeField] protected SplineCpsData data;
        public bool showSegments;
        public bool show1stDerivative;
        public bool show2ndDerivative;
        public SplineCpsData Data { get { return data; } set { data = value; } }
        public Spline spline { get; protected set; }

        void Start() {
            spline = new Spline(data.cps.ToArray(), data.order, data.type);
            for (int i = 0; i < data.cps.Count; i++)
                spline.SetCP(i, new CP(transform.TransformPoint(data.cps[i].pos), data.cps[i].weight));
        }
    }
}
