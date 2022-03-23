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
            value = value % 1f;
            value += speed / handler.spline.GetNorm2Derivative(value).magnitude;
            var p =handler.spline.GetNorm2Curve(value);
            velocity = (p - tracer.transform.position).magnitude * 10000;
            tracer.transform.position = p;
        }
    }
}
