using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.NURBS {
    public class SurfaceHandler : MonoBehaviour {
        [SerializeField] protected SurfaceCpsData data;
        public SurfaceCpsData Data { get { return data; } set { data = value; } }
        public Surface surface { get; protected set; }
        [SerializeField, Range(0, 1)] public float checker;
        public float normalizedT(float t, int count) => Mathf.Clamp((t * (count + 1) + data.order - 1) / (count + data.order), 0, 1 - 1e-5f);

        void Start() {
            surface = new Surface(data.GetCps(), data.order);
            for (int y = 0; y < data.count.y; y++)
                for (int x = 0; x < data.count.x; x++) {
                    var i = data.Convert(x, y);
                    surface.UpdateCP(new Vector2Int(x, y), new CP(transform.position + data.cps[i].pos, data.cps[i].weight));
                }
        }
    }
}
