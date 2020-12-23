using UnityEngine;

namespace kmty.NURBS {
    public class Surface {
        CP[] cps;
        int order;
        int olx, oly;
        int nlx, nly;
        int nidx(int x, int y) => x + y * nlx;
        int oidx(int x, int y) => x + y * olx;

        public Surface(CP[] originalCps, int order, int olx, int oly) {
            this.olx = olx;
            this.oly = oly;
            this.order = order;
            nlx = olx + 2 * order;
            nly = oly + 2 * order;
            cps = new CP[nidx(nlx, nly)];

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

        public Vector3 GetCurve(float tx, float ty) {
            var frac = Vector3.zero;
            var deno = 0f;
            tx = Mathf.Min(tx, 1f - 1e-5f);
            ty = Mathf.Min(ty, 1f - 1e-5f);
            for (int y = 0; y < nly; y++) {
                for (int x = 0; x < nlx; x++) {
                    var bf = BasisFunc(x, order, tx, true) * BasisFunc(y, order, ty, false);
                    var cp = cps[nidx(x, y)];
                    frac += cp.pos * bf * cp.weight;
                    deno += bf * cp.weight;
                }
            }
            return frac / deno;
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

        float BasisFunc(int j, int k, float t, bool xDir) {
            if (k == 0) {
                return (t >= KnotVector(j, xDir) && t < KnotVector(j + 1, xDir)) ? 1 : 0;
            }
            else {
                var d1 = KnotVector(j + k, xDir) - KnotVector(j, xDir);
                var d2 = KnotVector(j + k + 1, xDir) - KnotVector(j + 1, xDir);
                var c1 = d1 != 0 ? (t - KnotVector(j, xDir)) / d1 : 0;
                var c2 = d2 != 0 ? (KnotVector(j + k + 1, xDir) - t) / d2 : 0;
                return c1 * BasisFunc(j, k - 1, t, xDir) + c2 * BasisFunc(j + 1, k - 1, t, xDir);
            }
        }

        /// <summary>
        /// open uniform knot vector
        /// </summary>
        float KnotVector(int j, bool xDir) {
            var l = xDir ? nlx : nly;
            if (j < order) return 0;
            if (j > l - order) return 1;
            return Mathf.Clamp01((j - order) / (float)(l - 2 * order));
        }
    }
}
