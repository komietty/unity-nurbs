using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.NURBS {
    public class Surface: System.IDisposable {
        NativeArray<CP> cps;
        int order, olx, oly, nlx, nly;
        int nidx(int x, int y) => x + y * nlx;
        int oidx(int x, int y) => x + y * olx;
        public NativeArray<CP> CPs => cps;

        public Surface(List<CP> originalCps, int order, int olx, int oly) {
            this.olx = olx;
            this.oly = oly;
            this.order = order;
            nlx = olx + 2 * order;
            nly = oly + 2 * order;
            cps = new NativeArray<CP>(nlx * nly, Allocator.Persistent);

            // prepare extented array 2d
            for (int y = 0; y < order; y++)
                for (int x = 0; x < order; x++) {
                    var ix = nlx - 1 - x;
                    var iy = nly - 1 - y;
                    cps[nidx(x, y)]   = originalCps[oidx(0, 0)];
                    cps[nidx(ix, y)]  = originalCps[oidx(olx - 1, 0)];
                    cps[nidx(x, iy)]  = originalCps[oidx(0, oly - 1)];
                    cps[nidx(ix, iy)] = originalCps[oidx(olx - 1, oly - 1)];
                }
            for (int y = 0; y < oly; y++)
                for (int x = 0; x < order; x++) {
                    var yo = y + order;
                    var ix = nlx - 1 - x;
                    cps[nidx(x, yo)]  = originalCps[oidx(0, y)];
                    cps[nidx(ix, yo)] = originalCps[oidx(olx - 1, y)];
                }
            for (int x = 0; x < olx; x++)
                for (int y = 0; y < order; y++) {
                    var xo = x + order;
                    var iy = nly - 1 - y;
                    cps[nidx(xo, y)]  = originalCps[oidx(x, 0)];
                    cps[nidx(xo, iy)] = originalCps[oidx(x, oly - 1)];
                }
            for (int y = 0; y < oly; y++)
                for (int x = 0; x < olx; x++) {
                    cps[nidx(x + order, y + order)] = originalCps[oidx(x, y)];
                }
        }

        public void UpdateCP(Vector2Int i, CP cp) {
            var cl = new Vector2Int(nlx, nly);
            var ol = cl - order * 2 * Vector2.one;
            // corner condition
            if (i.x == 0 && i.y == 0)                    for (int x = 0; x <= order; x++) for (int y = 0; y <= order; y++) cps[nidx(x, y)] = cp;
            else if (i.x == 0 && i.y == ol.y - 1)        for (int x = 0; x <= order; x++) for (int y = 0; y <= order; y++) cps[nidx(x, cl.y - y - 1)] = cp;
            else if (i.x == ol.x - 1 && i.y == 0)        for (int x = 0; x <= order; x++) for (int y = 0; y <= order; y++) cps[nidx(cl.x - x - 1, y)] = cp;
            else if (i.x == ol.x - 1 && i.y == ol.y - 1) for (int x = 0; x <= order; x++) for (int y = 0; y <= order; y++) cps[nidx(cl.x - x - 1, cl.y - y - 1)] = cp;
            // edge condition
            else if (i.x == 0)        for (int x = 0; x <= order; x++) cps[nidx(x, i.y + order)] = cp;
            else if (i.y == 0)        for (int y = 0; y <= order; y++) cps[nidx(i.x + order, y)] = cp;
            else if (i.x == ol.x - 1) for (int j = 0; j <= order; j++) cps[nidx(cl.x - j - 1, i.y + order)] = cp;
            else if (i.y == ol.y - 1) for (int j = 0; j <= order; j++) cps[nidx(i.x + order, cl.y - j - 1)] = cp;
            // other point condition
            else cps[nidx(i.x + order, i.y + order)] = cp;
        }

        public Vector3 GetCurve(float tx, float ty) => NURBSSurface.GetCurve(cps, tx, ty, order, olx, oly);
        public bool IsAccessbile => cps.IsCreated;
        public void Dispose() => cps.Dispose();
    }

    public static class NURBSSurface {

        public static Vector3 GetCurve(NativeArray<CP> cps, float tx, float ty, int order, int olx, int oly) {
            var frac = Vector3.zero;
            var deno = 0f;
            var nlx = olx + 2 * order;
            var nly = oly + 2 * order;
            tx = Mathf.Min(tx, 1f - 1e-5f);
            ty = Mathf.Min(ty, 1f - 1e-5f);
            for (int y = 0; y < nly; y++) {
                for (int x = 0; x < nlx; x++) {
                    var bf = BasisFunc(x, order, order, tx, nlx) * BasisFunc(y, order, order, ty, nly);
                    var cp = cps[x + y * nlx];
                    frac += cp.pos * bf * cp.weight;
                    deno += bf * cp.weight;
                }
            }
            return frac / deno;
        }

        public static float BasisFunc(int j, int k, int order, float t, int l) {
            if (k == 0) {
                return (t >= KnotVector(j, order, l) && t < KnotVector(j + 1, order, l)) ? 1 : 0;
            }
            else {
                var d1 = KnotVector(j + k, order, l) - KnotVector(j, order, l);
                var d2 = KnotVector(j + k + 1, order, l) - KnotVector(j + 1, order, l);
                var c1 = d1 != 0 ? (t - KnotVector(j, order, l)) / d1 : 0;
                var c2 = d2 != 0 ? (KnotVector(j + k + 1, order, l) - t) / d2 : 0;
                return c1 * BasisFunc(j, k - 1, order, t, l) + c2 * BasisFunc(j + 1, k - 1, order, t, l);
            }
        }

        /// <summary>
        /// open uniform knot vector
        /// </summary>
        static float KnotVector(int j, int order, int len) {
            if (j < order) return 0;
            if (j > len - order) return 1;
            return Mathf.Clamp01((j - order) / (float)(len - 2 * order));
        }
    }
}
