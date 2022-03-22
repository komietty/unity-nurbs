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
            tracer.transform.localScale *= 0.2f;
            tracer.transform.SetParent(this.transform);
        }

        void Update() {
            var t = handler.normalizedT((Time.time / 3) % 1);
            var min = handler.spline.min;
            var max = handler.spline.max;
            var f = handler.spline.GetCurve(min + (max - min) * t, out Vector3 v);
            if (f) tracer.transform.position = v;
        }
    }
}
