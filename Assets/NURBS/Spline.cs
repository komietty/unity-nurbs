using UnityEngine;

// SOURCE:
// https://web.mit.edu/hyperbook/Patrikalakis-Maekawa-Cho/node16.html

namespace kmty.NURBS {
    public class Spline {
        public CP[] cps { get; protected set; }
        public KnotType type { get; protected set; }
        bool loop;
        int order;
        float min => knots[order];
        float max => knots[cps.Length];
        float[] knots;
        public float shift(float t) => min + (max - min) * t;

        public Spline(CP[] cps, int order, bool loop, KnotType type) {
            this.loop = loop;
            this.order = order;
            this.type = type;
            if (loop) {
                this.cps = new CP[cps.Length + order];
                System.Array.Copy(cps, this.cps, cps.Length);
                for (int i = 0; i < order; i++) this.cps[this.cps.Length - order + i] = cps[i];
            } else {
                this.cps = cps;
            }

            int knotNum = this.cps.Length + order + 1;
            knots = new float[knotNum];
            for (int i = 0; i < knotNum; i++) {
                knots[i] = SplineCommon.KnotVector(i, order, knotNum, type);
            }
        }

        public void SetCP(int i, CP cp) {
            cps[i] = cp;
            if (loop && i < order) cps[cps.Length - order + i] = cp;
        }

        public bool GetCurve(float t, out Vector3 v) {
            var frac = Vector3.zero;
            var deno = 1e-10f;
            for (int i = 0; i < cps.Length; i++) {
                var bf = BasisFunc(i, order, t);
                var cp = cps[i];
                frac += cp.pos * bf * cp.weight;
                deno += bf * cp.weight;
            }
            v = frac / deno;
            return t >= min && t <= max;
        }

        float BasisFancFirstDerivative(int i, int o, float t) {
            var i1 = BasisFunc(i + 1, o - 1, t) / (knots[i + o] - knots[i + 1]);
            var i2 = BasisFunc(i, o - 1, t)     / (knots[i + o - 1] - knots[i]);
            return (o - 1) * (-i1 + i2);
        }

        float BasisFancSecondDerivative(int i, int o, float t) {
            var i1 = BasisFancFirstDerivative(i + 1, o - 1, t) / (knots[i + o] - knots[i + 1]);
            var i2 = BasisFancFirstDerivative(i, o - 1, t)     / (knots[i + o - 1] - knots[i]);
            return (o - 1) * (-i1 + i2);
        }

        public Vector3 GetFirstDerivative(float t) {
            var v = Vector3.zero;
            for (int i = 0; i < cps.Length; i++) 
                v += BasisFancFirstDerivative(i, order, t) * cps[i].pos;
            return v;
        }

        public Vector3 GetSecondDerivative(float t) {
            var v = Vector3.zero;
            for (int i = 0; i < cps.Length; i++) 
                v += BasisFancSecondDerivative(i, order, t) * cps[i].pos;
            return v;
        }

        float BasisFunc(int j, int k, float t) {
            if (k == 0) { return (t >= knots[j] && t < knots[j + 1]) ? 1f : 0f; }
            else {
                var d1 = knots[j + k] - knots[j];
                var d2 = knots[j + k + 1] - knots[j + 1];
                var c1 = d1 != 0 ? (t - knots[j]) / d1 : 0;
                var c2 = d2 != 0 ? (knots[j + k + 1] - t) / d2 : 0;
                return c1 * BasisFunc(j, k - 1, t) + c2 * BasisFunc(j + 1, k - 1, t);
            }
        }

        // IN PROGRESS:
        // To calcurate derivatives for openknot vector, hodograph seems to be needed.
        // https://pages.mtu.edu/~shene/COURSES/cs3621/NOTES/spline/B-spline/bspline-derv.html

        public Spline hodograph {get; set;}
        float[] knots_droped;

        public static Spline GenHodograph(Spline s) {
            var cps = new CP[s.cps.Length - 1];
            if(s.type == KnotType.OpenUniform) {
                for (var i = 0; i < cps.Length; i++) {
                    var q = Vector3.zero;
                    if (i == 0) {
                        q = (s.cps[1].pos - s.cps[0].pos) / (s.knots[s.order + 1]);
                    } else if (i == cps.Length - 1) {
                        q = (s.cps[i + 1].pos - s.cps[i].pos) / (1 - s.knots[s.knots.Length - s.order - 1]);
                    } else {
                        q = (s.cps[i + 1].pos - s.cps[i].pos) / (s.knots[i + s.order + 1] - s.knots[i + 1]);
                    }
                    cps[i] = new CP(s.order * q, 1);
                }

            } else {
                for (var i = 0; i < cps.Length; i++) {
                    var q = (s.cps[i + 1].pos - s.cps[i].pos) / (s.knots[i + s.order + 1] - s.knots[i + 1]);
                    cps[i] = new CP(s.order * q, 1);
                }
            }
            return new Spline(cps, s.order - 1, s.loop, s.type);
        }
    }
}
