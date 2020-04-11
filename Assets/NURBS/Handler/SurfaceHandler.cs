using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.NURBS {
    public class SurfaceHandler : MonoBehaviour {
        [SerializeField] protected SurfaceCpsData data;
        public SurfaceCpsData Data { get { return data; } set { data = value; } }
        public Surface surface { get; protected set; }

        void Start() {
            surface = new Surface(data.GetCps(), data.order);
            for (int y = 0; y < data.divide.y; y++)
                for (int x = 0; x < data.divide.x; x++) {
                    var i = x + y * data.divide.x;
                    surface.UpdateCP(new Vector2Int(x, y), new CP(transform.position + data.cps[i].pos, data.cps[i].weight));
                }
        }
    }
}
