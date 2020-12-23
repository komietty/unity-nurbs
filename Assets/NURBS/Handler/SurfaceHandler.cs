using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.NURBS {
    public class SurfaceHandler : MonoBehaviour {
        [SerializeField] protected SurfaceCpsData data;
        [SerializeField] protected Material mat;
        [SerializeField, Range(0, 1)] public float checker;
        public SurfaceCpsData Data { get { return data; } set { data = value; } }
        public Surface surface { get; protected set; }
        public float normalizedT(float t, int count) => Mathf.Clamp((t * (count + 1) + data.order - 1) / (count + data.order), 0, 1 - 1e-5f);
        public Mesh mesh { get; private set; }
        protected MeshRenderer rndr;
        protected MeshFilter   fltr;

        void Start() {
            surface = new Surface(data.cps.ToArray(), data.order, data.count.x, data.count.y);
            for (int y = 0; y < data.count.y; y++)
                for (int x = 0; x < data.count.x; x++) {
                    var i = data.Convert(x, y);
                    surface.UpdateCP(new Vector2Int(x, y), new CP(transform.position + data.cps[i].pos, data.cps[i].weight));
                }
            CreateMesh(100, 100);
        }

        public void CreateMesh(int divX, int divY) {
            mesh = new Mesh();
            fltr = gameObject.AddComponent<MeshFilter>();
            rndr = gameObject.AddComponent<MeshRenderer>();
            var vtcs = new Vector3[(divX + 1) * (divY + 1)];
            var idcs = new List<int>();
            var lx = divX + 1;
            var ly = divY + 1;
            var dx = 1f / divX;
            var dy = 1f / divY;
            for (int iy = 0; iy <= divY; iy++)
            for (int ix = 0; ix <= divX; ix++) {
                int i = ix + iy * lx;
                vtcs[i] = surface.GetCurve(ix * dx, iy * dy);
                if(iy < divY && ix < divX) {
                    idcs.Add(i);
                    idcs.Add(i + 1);
                    idcs.Add(i + lx);
                    idcs.Add(i + lx);
                    idcs.Add(i + 1);
                    idcs.Add(i + lx + 1);
                }
            }
            mesh.vertices  = vtcs;
            mesh.triangles = idcs.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            rndr.material = mat;
            fltr.mesh = mesh;
        }

        public void UpdateMesh(int i, Vector3 pos) {
        }
    }
}
