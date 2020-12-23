using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.NURBS.Demo {
    [RequireComponent(typeof(SurfaceHandler))]
    public class DemoSurface : MonoBehaviour {
        protected GameObject tracer;
        protected SurfaceHandler handler;

        void Start() {
            handler = GetComponent<SurfaceHandler>();
        }
    }
}
