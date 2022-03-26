using UnityEngine;

namespace kmty.NURBS {
    public class Spline {
        public CP[] cps { get; private set; }
        bool loop;
        int order;
        int ctrlNum => cps.Length;
        int knotNum => cps.Length + order + 1;
        float min => knots[order];
        float max => knots[ctrlNum];
        public float shift(float t) => min + (max - min) * t;
        float[] knots;
        float[] knots_droped;
        KnotType knotType;

        public Spline(CP[] cps, int order, bool loop, KnotType knotType) {
            this.loop = loop;
            this.order = order;
            this.knotType = knotType;
            if (loop) {
                this.cps = new CP[cps.Length + order];
                System.Array.Copy(cps, this.cps, cps.Length);
                for (int i = 0; i < order; i++) this.cps[this.cps.Length - order + i] = cps[i];
            } else {
                this.cps = cps;
            }

            knots = new float[knotNum];
            for (int i = 0; i < knotNum; i++) {
                if (knotType == KnotType.Uniform) knots[i] = i / (float)(knotNum - 1);
                else {
                    //knots[i] = OpenUniformKnotVec(i);
                    if (i <= order) knots[i] = 0f;
                    if (i >= knotNum - 1 - order) knots[i] = 1f;
                    knots[i] = (float)i / (knotNum - order + 1);
                }
            }

            knots_droped = new float[knotNum - 2];
            for (int i = 0; i < knotNum - 2; i++) { knots_droped[i] = knots[i + 1]; }
        }

        public void SetCP(int i, CP cp) {
            cps[i] = cp;
            if (loop && i < order) cps[ctrlNum - order + i] = cp;
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
            return t >= knots[order] && t <= knots[ctrlNum]; 
        }

        float BasisFancFirstDerivative(int i, int o, float t) {
            if (knotType == KnotType.Uniform) {
                var i1 = BasisFunc(i + 1, o - 1, t) / (knots[i + o] - knots[i + 1]);
                var i2 = BasisFunc(i, o - 1, t) / (knots[i + o - 1] - knots[i]);
                return o * (-i1 + i2);
            } else {
                return 0;
            }
        }

        float BasisFancSecondDerivative(int i, int o, float t) {
            if (knotType == KnotType.Uniform) {
                var i1 = BasisFancFirstDerivative(i + 1, o - 1, t) / (knots[i + o] - knots[i + 1]);
                var i2 = BasisFancFirstDerivative(i, o - 1, t) / (knots[i + o - 1] - knots[i]);
                return o * (-i1 + i2);
            } else {
                return 0;
            }
        }

        public Vector3 GetFirstDerivative(float t) {
            var v = Vector3.zero;
            for (int i = 0; i < cps.Length; i++) 
                v += BasisFancFirstDerivative(i, order, t) * cps[i].pos;
            return v;
        }

        public Vector3 GetFirstDerivative_(float t) {
            if(hodograph == null) hodograph = GenHodograph(this); 
            hodograph.GetCurve(t, out Vector3 v);
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

        public Spline hodograph {get; set;}
        public static Spline GenHodograph(Spline s) {
            var cps = new CP[s.cps.Length - 1];
            for (var i = 0; i < cps.Length; i++) {
                var q = s.order 
                        / (s.knots[i + s.order + 1] - s.knots[i + 1])
                        * (s.cps[i + 1].pos - s.cps[i].pos);
                cps[i] = new CP(q, 1);
            }
            return new Spline(cps, s.order - 1, s.loop, s.knotType);
        }

        //public Vector3 GetCurveOffset(float t) {
        //    var frac = Vector3.zero;
        //    for (int i = 0; i < cps.Length; i++) {
        //        var bf = BasisFunc(i + 1, order, t);
        //        var cp = cps[i];
        //        frac += cp.pos * bf;
        //    }
        //    return frac;
        //}
        //public Vector3 GetDerivativeOffset(float t) {
        //    var frac = Vector3.zero;
        //    for (int i = 0; i < cps.Length - 1; i++) {
        //        var bf = BasisFunc(i + 2, order - 1, t);
        //        frac += bf * GetQ(i);
        //    }
        //    return frac;
        //}
    }
}
