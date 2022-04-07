using Unity.Collections;
using UnityEngine;
using System.Linq;

namespace kmty.NURBS {
    public class Surface: System.IDisposable {
        NativeArray<CP> cps;
        public NativeArray<CP> CPs => cps;
        public Vector2 min => new Vector2(Shared.KnotVector(order, order, lx, xknotType), Shared.KnotVector(order, order, ly, yknotType));
        public Vector2 max => new Vector2(Shared.KnotVector(lx,    order, lx, xknotType), Shared.KnotVector(ly,    order, ly, yknotType));
        public int order, lx, ly;
        public bool xloop { get; protected set; }
        public bool yloop { get; protected set; }
        public KnotType xknotType { get; protected set; }
        public KnotType yknotType { get; protected set; }
        public void Dispose() => cps.Dispose();
        float shiftx(float t) => min.x + (max.x - min.x) * t;
        float shifty(float t) => min.y + (max.y - min.y) * t;
        int idx(int x, int y) => x + y * this.lx;

        public Surface(CP[] cps, int order, int lx, int ly, SplineType xtype, SplineType ytype) {
            this.order = order;
            this.xloop = xtype == SplineType.Loop;
            this.yloop = ytype == SplineType.Loop;
            this.xknotType = xtype == SplineType.Clamped ? KnotType.OpenUniform : KnotType.Uniform;
            this.yknotType = ytype == SplineType.Clamped ? KnotType.OpenUniform : KnotType.Uniform;
            if (xloop && yloop) {
                var xlooped = GenXLoop(cps, lx, ly);
                var ylooped = GenYLoop(xlooped, lx, ly);
                SetData(ylooped, lx + order, ly + order);
            } else if (yloop) { SetData(GenYLoop(cps, lx, ly), lx, ly + order);
            } else if (xloop) { SetData(GenXLoop(cps, lx, ly), lx + order, ly);
            } else            { SetData(cps, lx, ly); }
        }

        public bool GetCurve(float normTx, float normTy, out Vector3 v){
            var tx = shiftx(normTx);
            var ty = shifty(normTy);
            var fx = tx >= min.x && tx <= max.x;
            var fy = ty >= min.y && ty <= max.y;
            v = SurfaceUtil.GetCurve(cps, tx, ty, order, lx, ly, xknotType, yknotType);
            return fx && fy;

        }
        public void SetCP(Vector2Int i, CP cp) {
            var fx = xloop && i.x < order;
            var fy = yloop && i.y < order;
            cps[idx(i.x, i.y)] = cp;
            if (fx) cps[idx(lx - order + i.x, i.y)] = cp;
            if (fy) cps[idx(i.x, ly - order + i.y)] = cp;
            if (fx && fy) cps[idx(lx - order + i.x, ly - order + i.y)] = cp;
        }

        CP[] GenXLoop(CP[] cps, int lx, int ly) {
            var arr = new CP[(lx + order) * ly];
            for (int y = 0; y < ly; y++) 
            for (int x = 0; x < lx + order; x++) {
                var i1 = x % lx + y * lx;
                var i2 = x + y * (lx + order);
                arr[i2] = cps[i1];
            }
            return arr;
        }

        CP[] GenYLoop(CP[] cps, int lx, int ly) {
            var lst = cps.ToList();
            for (int i = 0; i < order; i++) {
                var row = new CP[lx + order];
                System.Array.Copy(cps, i * (lx + order), row, 0, lx + order);
                lst.AddRange(row);
            }
            return lst.ToArray();
        }

        void SetData(CP[] cps, int lx, int ly) {
            this.lx = lx;
            this.ly = ly;
            this.cps = new NativeArray<CP>(cps, Allocator.Persistent);
        }
    }

    public static class SurfaceUtil {
        public static Vector3 GetCurve(NativeArray<CP> cps, float tx, float ty, int o, int lx, int ly, KnotType kx, KnotType ky) {
            var f = Vector3.zero;
            var d = 1e-9f;
            for (int y = 0; y < ly; y++) {
                for (int x = 0; x < lx; x++) {
                    var b = BasisFunc(x, o, o, tx, lx, kx) * BasisFunc(y, o, o, ty, ly, ky);
                    var c = cps[x + y * lx];
                    f += c.pos * b * c.weight;
                    d += b * c.weight;
                }
            }
            return f / d;
        }

        static float BasisFunc(int j, int k, int o, float t, int l, KnotType kt) {
            if (k == 0) {
                return (t >= Shared.KnotVector(j, o, l, kt) && t < Shared.KnotVector(j + 1, o, l, kt)) ? 1 : 0;
            }
            else {
                var d1 = Shared.KnotVector(j + k, o, l, kt) - Shared.KnotVector(j, o, l, kt);
                var d2 = Shared.KnotVector(j + k + 1, o, l, kt) - Shared.KnotVector(j + 1, o, l, kt);
                var c1 = d1 != 0 ? (t - Shared.KnotVector(j, o, l, kt)) / d1 : 0;
                var c2 = d2 != 0 ? (Shared.KnotVector(j + k + 1, o, l, kt) - t) / d2 : 0;
                return c1 * BasisFunc(j, k - 1, o, t, l, kt) + c2 * BasisFunc(j + 1, k - 1, o, t, l, kt);
            }
        }
    }
}
