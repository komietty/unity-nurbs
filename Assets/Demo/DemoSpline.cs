using UnityEngine;

namespace kmty.NURBS.Demo {
    [RequireComponent(typeof(SplineHandler))]
    public class DemoSpline : MonoBehaviour {
        protected GameObject tracer;
        protected SplineHandler handler;
        [SerializeField] protected float value;
        [SerializeField] protected float velocity;
        [SerializeField, Range(0, 1)] protected float speed;

        void Start() {
            handler = GetComponent<SplineHandler>();
            tracer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            tracer.transform.localScale *= 0.1f;
            tracer.transform.SetParent(this.transform);
        }

        void Update() {
            var s = handler.spline;
            value = value % 1f;
            if(s.knotType == KnotType.Uniform){
                s.GetFirstDerivative(value, out Vector3 d);
                value += speed / d.magnitude;
            } else {
                // TODO: when it could get derivative for clamped curve, normalize the speed;
                value += speed * 0.1f;
            }
            s.GetCurve(value, out Vector3 p);
            velocity = (p - tracer.transform.position).magnitude * 10000;
            tracer.transform.position = p;
        }
    }
}
