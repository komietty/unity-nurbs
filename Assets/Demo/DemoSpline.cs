using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.NURBS.Demo {
    [RequireComponent(typeof(SplineHandler))]
    public class DemoSpline : MonoBehaviour {
        protected GameObject tracer;
        protected SplineHandler handler;

        void Start() {
            handler = GetComponent<SplineHandler>();
            tracer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            tracer.transform.SetParent(this.transform);
        }

        void Update() {
            tracer.transform.position = handler.spline.GetCurve((Time.time / 3) % 1);
        }
    }
}
