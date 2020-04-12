using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.NURBS.Demo {
    [RequireComponent(typeof(SurfaceHandler))]
    public class DemoSurface : MonoBehaviour {
        protected GameObject tracer;
        protected SurfaceHandler handler;
        [SerializeField, Range(0, 0.9999f)] protected float tx;
        [SerializeField, Range(0, 0.9999f)] protected float ty;

        void Start() {
            handler = GetComponent<SurfaceHandler>();
            tracer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            tracer.transform.SetParent(this.transform);
        }

        void Update() {
            var _tx = handler.normalizedT(tx, handler.Data.count.x);
            var _ty = handler.normalizedT(ty, handler.Data.count.y);
            tracer.transform.position = handler.surface.GetCurve(_tx, _ty);
        }
    }
}
