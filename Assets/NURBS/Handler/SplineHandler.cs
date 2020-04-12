using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.NURBS {
    public class SplineHandler : MonoBehaviour {
        [SerializeField] protected SplineCpsData data;
        public SplineCpsData Data { get { return data; } set { data = value; } }
        public Spline spline { get; protected set; }
        public float normalizedT(float time) => (time * (1 + data.cps.Count) + (data.order - 1)) / (data.cps.Count + data.order);

        void Start() {
            spline = new Spline(data.cps.ToArray(), data.order);

            // convert to world coordinate
            for (int i = 0; i < data.cps.Count; i++)
                spline.UpdateCP(i, new CP(transform.position + data.cps[i].pos, data.cps[i].weight));
        }
    }
}
