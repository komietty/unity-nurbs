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
        protected float normalizedT(float t, int count) => Mathf.Clamp((t * (count + 1) + data.order - 1) / (count + data.order), 0, 1 - 1e-5f);
        protected MeshRenderer rndr;
        protected MeshFilter   fltr;
        protected NativeArray<Vector3> vtcs;

        void Start() {
            surface = new Surface(data.cps, data.order, data.count.x, data.count.y);
            for (int y = 0; y < data.count.y; y++)
                for (int x = 0; x < data.count.x; x++) {
                    var i = data.Convert(x, y);
                    surface.UpdateCP(new Vector2Int(x, y), new CP(transform.position + data.cps[i].pos, data.cps[i].weight));
                }
            CreateMesh();
        }

        void OnDestroy() {
            surface.Dispose();
            vtcs.Dispose();
        }

        public void Reset() {
            if (surface != null) surface.Dispose();
            surface = new Surface(data.cps, data.order, data.count.x, data.count.y);

        }

        public Vector3 GetCurve(float t1, float t2) {
            return surface.GetCurve(normalizedT(t1, data.count.x), normalizedT(t2, data.count.y));
        }

        void CreateMesh() {
            mesh = new Mesh();
            fltr = gameObject.AddComponent<MeshFilter>();
            rndr = gameObject.AddComponent<MeshRenderer>();
            vtcs = new NativeArray<Vector3>((division.x + 1) * (division.y + 1), Allocator.Persistent);
            var idcs = new List<int>();
            var lx = division.x + 1;
            var ly = division.y + 1;
            var dx = 1f / division.x;
            var dy = 1f / division.y;
            for (int iy = 0; iy < ly; iy++)
            for (int ix = 0; ix < lx; ix++) {
                int i = ix + iy * lx;
                vtcs[i] = surface.GetCurve(ix * dx, iy * dy);
                if(iy < division.y && ix < division.x) {
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
            public Vector2 invdiv;
            public int order;

            public void Execute(int id) {
                var l = division.x + 1;
                var x = (id % l) * invdiv.x;
                var y = (id / l) * invdiv.y;
                vtcs[id] = NURBSSurface.GetCurve(cps, x, y, order, cpslen.x, cpslen.y);
            }
        }


        public void UpdateMesh() {
            var job = new UpdateMeshJob {
                vtcs     = vtcs,
                cps      = surface.CPs,
                division = division,
                invdiv = new Vector2(1f / division.x, 1f / division.y),
                cpslen = data.count,
                order    = data.order
            };
            job.Schedule(vtcs.Length, 0).Complete();

            mesh.SetVertices(vtcs);
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

        }
    }
}
