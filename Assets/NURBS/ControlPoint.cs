using UnityEngine;

namespace kmty.NURBS {

    public struct CP {
        public Vector3 pos;
        public float weight;
        public CP(Vector3 p, float w) { pos = p; weight = w; }
    }
}
