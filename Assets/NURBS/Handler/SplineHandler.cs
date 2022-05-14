using UnityEngine;
using System.Collections.Generic;

namespace kmty.NURBS {
    public class SplineHandler : MonoBehaviour {
        [SerializeField] protected SplineCpsData data;
        public bool showSegments;
        public bool show1stDerivative;
        public bool show2ndDerivative;
        public SplineCpsData Data { get { return data; } set { data = value; } }
        public Spline spline { get; protected set; }
        public Mesh mesh { get; private set; }

        void Start() {
            spline = new Spline(data.cps.ToArray(), data.order, data.type);
            for (int i = 0; i < data.cps.Count; i++)
                spline.SetCP(i, new CP(transform.TransformPoint(data.cps[i].pos), data.cps[i].weight));
        }

        void CreateMesh() {
            mesh = new Mesh();
            var seg = 0.05f;
            var len = Mathf.FloorToInt(1 / seg);
            var ps = new Vector3[len];
            var fs = new Vector3[len];
            var ss = new Vector3[len];
            var vs = new Vector3[len];
            var ns = new Vector3[len];
            for (int i = 0; i < len; i++) {
                spline.GetCurve(i * seg, out ps[i]); 
                spline.GetFirstDerivative(i * seg, out fs[i]); 
                spline.GetSecondDerivative(i * seg, out ss[i]); 
            }
        }
    }
}
