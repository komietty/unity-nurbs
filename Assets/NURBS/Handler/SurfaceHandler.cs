using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

namespace kmty.NURBS {
    public class SurfaceHandler : MonoBehaviour {
        [SerializeField] protected SurfaceCpsData data;
        [SerializeField] protected Material mat;
        [SerializeField] protected Vector2Int division;
        [SerializeField] protected string bakePath = "Assets/Bakedmesh";
        [SerializeField] protected string bakeName = "bakedMesh";
        public SurfaceCpsData Data { get => data; set { data = value; } }
        public Surface surface { get; protected set; }
        public Mesh mesh { get; private set; }
        public string BakePath => bakePath;
        public string BakeName => bakeName;
        protected NativeArray<Vector3> vtcs;
        public List<Vector3> segments { get; protected set; } = new List<Vector3>();

        void Start() {
            Init();
            for (int y = 0; y < data.count.y; y++)
            for (int x = 0; x < data.count.x; x++) {
                var i = data.Convert(x, y);
                surface.SetCP(new Vector2Int(x, y), new CP(transform.position + data.cps[i].pos, data.cps[i].weight));
            }
            CreateMesh();
            UpdateSegments(data, transform.position);
        }
        
        void OnDestroy() {
            surface.Dispose();
            vtcs.Dispose();
        }

        public void Init() {
            if (surface != null) surface.Dispose();
            surface = new Surface(data.cps.ToArray(), data.order, data.count.x, data.count.y, data.xtype, data.ytype);
        }

        void CreateMesh() {
            mesh = new Mesh();
            vtcs = new NativeArray<Vector3>((division.x + 1) * (division.y + 1), Allocator.Persistent);
            var fltr = gameObject.AddComponent<MeshFilter>();
            var rndr = gameObject.AddComponent<MeshRenderer>();
            var idcs = new List<int>();
            var lx = division.x + 1;
            var ly = division.y + 1;
            var dx = 1f / division.x;
            var dy = 1f / division.y;
            for (int iy = 0; iy < ly; iy++)
            for (int ix = 0; ix < lx; ix++) {
                int i = ix + iy * lx;
                float _x = surface.min.x + ix * dx * (surface.max.x - surface.min.x);
                float _y = surface.min.y + iy * dy * (surface.max.y - surface.min.y);
                var f = surface.GetCurve(_x, _y, out Vector3 v);
                if(!f)  Debug.LogWarning("surface range is somehow wrong");
                vtcs[i] = v;
                if (iy < division.y && ix < division.x) {
                    idcs.Add(i);
                    idcs.Add(i + 1);
                    idcs.Add(i + lx);
                    idcs.Add(i + lx);
                    idcs.Add(i + 1);
                    idcs.Add(i + lx + 1);
                }
            }
            mesh.SetVertices(vtcs);
            mesh.SetTriangles(idcs.ToArray(), 0);
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
            rndr.material = mat;
            fltr.mesh = mesh;
        }

        [BurstCompile]
        struct UpdateMeshJob : IJobParallelFor {
            [WriteOnly] public NativeArray<Vector3> vtcs;
            [ReadOnly]  public NativeArray<CP> cps;
            public Vector2Int division;
            public Vector2Int cpslen;
            public Vector2 min;
            public Vector2 max;
            public Vector2 invdiv;
            public KnotType xknot;
            public KnotType yknot;
            public int order;

            public void Execute(int id) {
                var l = division.x + 1;
                var ix = (id % l) * invdiv.x;
                var iy = (id / l) * invdiv.y;
                float _x = min.x + ix * (max.x - min.x);
                float _y = min.y + iy * (max.y - min.y);
                vtcs[id] = NURBSSurface.GetCurve(cps, _x, _y, order, cpslen.x, cpslen.y, xknot, yknot);
            }
        }

        public void UpdateMesh() {
            var job = new UpdateMeshJob {
                vtcs = vtcs,
                cps = surface.CPs,
                division = division,
                invdiv = new Vector2(1f / division.x, 1f / division.y),
                cpslen = new Vector2Int(surface.lx, surface.ly),
                min = surface.min,
                max = surface.max,
                order = data.order,
                xknot = data.xknot,
                yknot = data.yknot,
            };
            job.Schedule(vtcs.Length, 0).Complete();

            mesh.SetVertices(vtcs);
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

        }

        public void UpdateSegments(SurfaceCpsData data, Vector3 hpos) {
            segments.Clear();
            for (int x = 0; x < data.count.x; x++) {
                for (int y = 0; y < data.count.y - 1; y++) {
                    segments.Add(hpos + data.cps[data.Convert(x, y)].pos);
                    segments.Add(hpos + data.cps[data.Convert(x, y + 1)].pos);
                }
                if (data.yloop) {
                    segments.Add(hpos + data.cps[data.Convert(x, data.count.y - 1)].pos);
                    segments.Add(hpos + data.cps[data.Convert(x, 0)].pos);
                }
            }
            for (int y = 0; y < data.count.y; y++) {
                for (int x = 0; x < data.count.x - 1; x++) {
                    segments.Add(hpos + data.cps[data.Convert(x, y)].pos);
                    segments.Add(hpos + data.cps[data.Convert(x + 1, y)].pos);
                }
                if(data.xloop){
                    segments.Add(hpos + data.cps[data.Convert(data.count.x - 1, y)].pos);
                    segments.Add(hpos + data.cps[data.Convert(0, y)].pos);
                }
            }
        }
    }
}
