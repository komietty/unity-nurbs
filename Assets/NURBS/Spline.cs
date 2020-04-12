using UnityEngine;

namespace kmty.NURBS {
    public class Spline {
        CP[] cps;
        int order;

        public Spline(CP[] originalCps, int order) {
            var ol = originalCps.Length;
            var nl = ol + 2 * order;
            cps = new CP[nl];
            System.Array.Copy(originalCps, 0, cps, order, ol);
            for (int i = 0; i < order; i++) {
                cps[i] = originalCps[0];
                cps[nl - i - 1] = originalCps[ol - 1];
            }
            this.order = order;
        }

        public Vector3 GetCurve(float t) {
            var frac = Vector3.zero;
            var deno = 0f;
            for (int i = 0; i < cps.Length; i++) {
                var bf = BasisFunc(i, order, t);
                var cp = cps[i];
                frac += cp.pos * bf * cp.weight;
                deno += bf * cp.weight;
            }
            return frac / deno;
        }

        public void UpdateCP(int i, CP cp) {
            var cl = cps.Length;
            var ol = cl - order * 2;
            if (i == 0) for (int j = 0; j <= order; j++) cps[j] = cp;
            else if (i == ol - 1) for (int j = 0; j <= order; j++) cps[cl - j - 1] = cp;
            else cps[i + order] = cp;
        }

        float BasisFunc(int j, int k, float t) {
            if (k == 0) {
                return (t >= KnotVector(j) && t < KnotVector(j + 1)) ? 1 : 0;
            }
            else {
                var d1 = KnotVector(j + k) - KnotVector(j);
                var d2 = KnotVector(j + k + 1) - KnotVector(j + 1);
                var c1 = d1 != 0 ? (t - KnotVector(j)) / d1 : 0;
                var c2 = d2 != 0 ? (KnotVector(j + k + 1) - t) / d2 : 0;
                return c1 * BasisFunc(j, k - 1, t) + c2 * BasisFunc(j + 1, k - 1, t);
            }
        }

        float KnotVector(int j) {
            var l = cps.Length;
            if (j < order) return 0;
            if (j >= l - order) return 1;
            return Mathf.Max(j - order, 0f) / (l - 2 * order);
        }
    }
}
