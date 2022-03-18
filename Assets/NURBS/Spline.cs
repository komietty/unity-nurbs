using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://pages.mtu.edu/~shene/COURSES/cs3621/NOTES/spline/B-spline/bspline-curve-closed.html
//https://pages.mtu.edu/~shene/COURSES/cs3621/NOTES/spline/B-spline/bspline-derv.html
// TODO: discrete Curvature

namespace kmty.NURBS {
    public enum KnotType { Uniform, OpenUniform }

    public class Spline {
        public CP[] cps { get; private set; }
        public float min => KnotVec(order);
        public float max => KnotVec(ctrlNum);
        bool closed;
        int order;
        int ctrlNum => cps.Length;             // num of control pooints
        int knotNum => cps.Length + order + 1; // num of knot vectors
        KnotType type = KnotType.Uniform;
        Spline hodograph;

        public Spline(CP[] cps, int order, bool closed) {
            this.closed = closed;
            this.order = order;
            if (closed) {
                this.cps = new CP[cps.Length + order];
                System.Array.Copy(cps, this.cps, cps.Length);
                for (int i = 0; i < order; i++) {
                    this.cps[this.cps.Length - order + i] = cps[i];
                }
            } else {
                this.cps = cps;
            }
        }

        public void SetCP(int i, CP cp) {
            cps[i] = cp;
            if (closed && i < order) cps[ctrlNum - order + i] = cp;
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
            return t >= KnotVec(order) && t <= KnotVec(ctrlNum); 
        }

        Vector3 GetQ(int i) { return order / (KnotVec(i + order + 1) - KnotVec(i + 1)) * (cps[i + 1].pos - cps[i].pos);}
        public Vector3 GetDerivative(float t) {
            var frac = Vector3.zero;
            for (int i = 0; i < cps.Length - 1; i++) {
                var bf = BasisFunc(i + 1, order - 1, t);
                frac += bf * GetQ(i);
            }
            return frac;
        }

        public void CreateHodograph(){
            var _cps = new CP[ctrlNum - 1];
            for (var i = 0; i < _cps.Length; i++) {
                //_cps[i] = new CP(GetQ(i).normalized, 1);
                _cps[i] = new CP(GetQ(i), 1);
                //TODO: how about weight??
                //TODO: is it nessaray, nomalize for curvature??
            }
            hodograph = new Spline(_cps, order - 1, closed);
        }

        public Vector3 GetSecondDerivative(float t) => hodograph.GetDerivative(t);



        float BasisFunc(int j, int k, float t) {
            if (k == 0) {
                return (t >= KnotVec(j) && t < KnotVec(j + 1)) ? 1f : 0f;
            }
            else {
                var d1 = KnotVec(j + k) - KnotVec(j);
                var d2 = KnotVec(j + k + 1) - KnotVec(j + 1);
                var c1 = d1 != 0 ? (t - KnotVec(j)) / d1 : 0;
                var c2 = d2 != 0 ? (KnotVec(j + k + 1) - t) / d2 : 0;
                return c1 * BasisFunc(j, k - 1, t) + c2 * BasisFunc(j + 1, k - 1, t);
            }
        }

        float KnotVec(int j) {
            switch(type){
                case KnotType.OpenUniform: return OpenUniformKnotVec(j);
                default: return UniformKnotVec(j);
            }
        }

        float UniformKnotVec(int j) {
            var t0 = 0f;
            var t1 = 1f;
            return t0 + (t1 - t0) / (knotNum - 1) * j;
        }

        float OpenUniformKnotVec(int j) {
            if (j <= order)               return 0f;
            if (j >= knotNum - 1 - order) return 1f;
            return (float)j / (knotNum - order + 1);
        }
    }
}
