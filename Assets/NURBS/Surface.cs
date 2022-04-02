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

        public Surface(CP[] cps, int order, int lx, int ly, SplineType xtype, SplineType ytype) {
            this.order = order;
            this.xloop = xtype == SplineType.Loop;
            this.yloop = ytype == SplineType.Loop;
            this.xknotType = xtype == SplineType.Clamped ? KnotType.OpenUniform : KnotType.Uniform;
            this.yknotType = ytype == SplineType.Clamped ? KnotType.OpenUniform : KnotType.Uniform;

            if (this.xloop && this.yloop) {
                var arr = new CP[(lx + order) * ly];
                for (int y = 0; y < ly; y++) {
                    for (int x = 0; x < lx + order; x++) {
                        var i1 = x % lx + y * lx;
                        var i2 = x + y * (lx + order);
                        arr[i2] = cps[i1];
                    }
                }
                var lst = arr.ToList();
                for (int i = 0; i < order; i++) {
                    var row = new CP[lx + order];
                    System.Array.Copy(arr, i * (lx + order), row, 0, lx + order);
                    lst.AddRange(row);
                }
                SetData(lst.ToArray(), lx + order, ly + order);
            } else if (this.yloop) {
                var lst = cps.ToList();
                for (int i = 0; i < order; i++) {
                    var row = new CP[lx];
                    System.Array.Copy(cps, i * lx, row, 0, lx);
                    lst.AddRange(row);
                }
                SetData(lst.ToArray(), lx, ly + order);
            } else if (this.xloop) {
                var arr = new CP[(lx + order) * ly];
                for (int y = 0; y < ly; y++) {
                    for (int x = 0; x < lx + order; x++) {
                        var i1 = x % lx + y * lx;
                        var i2 = x + y * (lx + order);
                        arr[i2] = cps[i1];
                    }
                }
                SetData(arr, lx + order, ly);
            } else {
                SetData(cps, lx, ly);
            }
        }

        public bool GetCurve(float tx, float ty, out Vector3 v){
            var fx = tx >= min.x && tx <= max.x;
            var fy = ty >= min.y && ty <= max.y;
            v = NURBSSurface.GetCurve(cps, tx, ty, order, lx, ly, xknotType, yknotType);
            return fx && fy;

        }
        public void SetCP(Vector2Int i, CP cp) {
            cps[idx(i.x, i.y)] = cp;
            if (xloop && i.x < order) cps[idx(lx - order + i.x, i.y)] = cp;
            if (yloop && i.y < order) cps[idx(i.x, ly - order + i.y)] = cp;
        }

        public bool IsAccessbile => cps.IsCreated;
        public void Dispose() => cps.Dispose();
        int idx(int x, int y) => x + y * lx;
        void SetData(CP[] cps, int lx, int ly) {
            this.lx = lx;
            this.ly = ly;
            this.cps = new NativeArray<CP>(cps, Allocator.Persistent);
        }
    }

    public static class NURBSSurface {
        public static Vector3 GetCurve(NativeArray<CP> cps, float tx, float ty, int o, int lx, int ly, KnotType kx, KnotType ky) {
            var f = Vector3.zero;
            var d = 1e-9f;
            for (int y = 0; y < ly; y++) {
                for (int x = 0; x < lx; x++) {
                    var b = BF(x, o, o, tx, lx, kx) * BF(y, o, o, ty, ly, ky);
                    var c = cps[x + y * lx];
                    f += c.pos * b * c.weight;
                    d += b * c.weight;
                }
            }
            return f / d;
        }

        static float KV(int j, int o, int l, KnotType t) => Shared.KnotVector(j, o, l, t);
        static float BF(int j, int k, int o, float t, int l, KnotType kt) {
            if (k == 0) { return (t >= KV(j, o, l, kt) && t < KV(j + 1, o, l, kt)) ? 1 : 0; }
            else {
                var d1 = KV(j + k, o, l, kt) - KV(j, o, l, kt);
                var d2 = KV(j + k + 1, o, l, kt) - KV(j + 1, o, l, kt);
                var c1 = d1 != 0 ? (t - KV(j, o, l, kt)) / d1 : 0;
                var c2 = d2 != 0 ? (KV(j + k + 1, o, l, kt) - t) / d2 : 0;
                return c1 * BF(j, k - 1, o, t, l, kt) + c2 * BF(j + 1, k - 1, o, t, l, kt);
            }
        }
    }
}
