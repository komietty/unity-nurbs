using UnityEngine;

namespace kmty.NURBS {
    public class Surface {
        CP[,] cps;
        int order;

        public Surface(CP[,] originalCps, int order) {
            int olx = originalCps.GetLength(0);
            int oly = originalCps.GetLength(1);
            int nlx = olx + 2 * order;
            int nly = oly + 2 * order;
            this.order = order;
            cps = new CP[nlx, nly];

            // prepare extented array 2d
            for (int y = 0; y < order; y++)
                for (int x = 0; x < order; x++) {
                    var ix = nlx - 1 - x;
                    var iy = nly - 1 - y;
                    cps[x, y] = originalCps[0, 0];
                    cps[ix, y] = originalCps[olx - 1, 0];
                    cps[x, iy] = originalCps[0, oly - 1];
                    cps[ix, iy] = originalCps[olx - 1, oly - 1];
                }
            for (int y = 0; y < oly; y++)
                for (int x = 0; x < order; x++) {
                    var yo = y + order;
                    var ix = nlx - 1 - x;
                    cps[x, yo] = originalCps[0, y];
                    cps[ix, yo] = originalCps[olx - 1, y];
                }
            for (int x = 0; x < olx; x++)
                for (int y = 0; y < order; y++) {
                    var xo = x + order;
                    var iy = nly - 1 - y;
                    cps[xo, y] = originalCps[x, 0];
                    cps[xo, iy] = originalCps[x, oly - 1];
                }
            for (int y = 0; y < oly; y++)
                for (int x = 0; x < olx; x++) {
                    cps[x + order, y + order] = originalCps[x, y];
                }
        }

        public Vector3 GetCurve(float tx, float ty) {
            var frac = Vector3.zero;
            var deno = 0f;
            for (int y = 0; y < cps.GetLength(1); y++) {
                for (int x = 0; x < cps.GetLength(0); x++) {
                    var bf = BasisFunc(x, order, tx, true) * BasisFunc(y, order, ty, false);
                    var cp = cps[x, y];
                    frac += cp.pos * bf * cp.weight;
                    deno += bf * cp.weight;
                }
            }
            return frac / deno;
        }

        public void UpdateCP(Vector2Int i, CP cp) {
            var cl = new Vector2Int(cps.GetLength(0), cps.GetLength(1));
            var ol = cl - order * 2 * Vector2.one;
            // corner condition
            if (i.x == 0 && i.y == 0)
                for (int x = 0; x <= order; x++) for (int y = 0; y <= order; y++) cps[x, y] = cp;
            else if (i.x == 0 && i.y == ol.y - 1)
                for (int x = 0; x <= order; x++) for (int y = 0; y <= order; y++) cps[x, cl.y - y - 1] = cp;
            else if (i.x == ol.x - 1 && i.y == 0)
                for (int x = 0; x <= order; x++) for (int y = 0; y <= order; y++) cps[cl.x - x - 1, y] = cp;
            else if (i.x == ol.x - 1 && i.y == ol.y - 1)
                for (int x = 0; x <= order; x++) for (int y = 0; y <= order; y++) cps[cl.x - x - 1, cl.y - y - 1] = cp;
            // edge condition
            else if (i.x == 0) for (int x = 0; x <= order; x++) cps[x, i.y + order] = cp;
            else if (i.y == 0) for (int y = 0; y <= order; y++) cps[i.x + order, y] = cp;
            else if (i.x == ol.x - 1) for (int j = 0; j <= order; j++) cps[cl.x - j - 1, i.y + order] = cp;
            else if (i.y == ol.y - 1) for (int j = 0; j <= order; j++) cps[i.x + order, cl.y - j - 1] = cp;
            // other point condition
            else cps[i.x + order, i.y + order] = cp;
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

        float KnotVector(int j, bool xDir) {
            var m = cps.GetLength(xDir ? 0 : 1) + order + 1;
            if (j < order + 1) return 0;
            if (j > m - (order + 1)) return 1;
            return Mathf.Max(j - order, 0f) / (m - 2 * (order + 1));
        }
    }
}
