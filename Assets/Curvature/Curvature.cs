using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using kmty.gist;
using static UnityEngine.GraphicsBuffer;

public class Curvature : MonoBehaviour {
    [SerializeField, Range(1, 10)] protected float colorScale;
    [SerializeField] protected ComputeShader cs;
    [SerializeField] protected bool weld;
    protected int len = 0;
    protected Material mat;
    protected GraphicsBuffer tblBuff, colBuff, idxBuff, vrtBuff;

    Mesh Weld(Mesh original) {
        var ogl_vrts = original.vertices;
        var ogl_idcs = original.triangles;
        var alt_mesh = new Mesh();
        var alt_vrts = ogl_vrts.Distinct().ToArray();
        var alt_idcs = new int[ogl_idcs.Length];
        var vrt_rplc = new int[ogl_vrts.Length];
        for(var i = 0; i < ogl_vrts.Length; i++){
            var o = -1;
            for(var j = 0;  j < alt_vrts.Length; j++){
                if (alt_vrts[j] == ogl_vrts[i]) { o = j; break; }
            }
            vrt_rplc[i] = o;
        }

        for(var i = 0; i < alt_idcs.Length; i++){
            alt_idcs[i] = vrt_rplc[ogl_idcs[i]];
        }
        alt_mesh.SetVertices(alt_vrts);
        alt_mesh.SetTriangles(alt_idcs, 0);
        return alt_mesh;
    }

    void Start() {
        var filt = GetComponent<MeshFilter>();
        var mesh = filt.sharedMesh;
        if (weld) {
            mesh = Weld(filt.sharedMesh);
            filt.mesh = mesh;
        }
        var vrts = mesh.vertices;
        var tris = mesh.GetIndices(0);
        var frtos = new Vector3Int[tris.Length];
        var bglns = new Vector2Int[vrts.Length];
        mat = GetComponent<MeshRenderer>().sharedMaterial;
        len = vrts.Length;

        for (var i = 0; i < tris.Length; i += 3) {
            var i0 = tris[i];
            var i1 = tris[i + 1];
            var i2 = tris[i + 2];
            frtos[i + 0] = new Vector3Int(i0, i1, i2);
            frtos[i + 1] = new Vector3Int(i1, i2, i0);
            frtos[i + 2] = new Vector3Int(i2, i0, i1);
        }

        frtos = frtos.OrderBy(ft => ft.x).ToArray();

        int fr = 0;
        int bg = 0;
        for(var i = 0; i < frtos.Length; i++){
            var curr = frtos[i];
            if (curr.x == fr + 1){
                bglns[fr] = new Vector2Int(bg, i - bg);
                bg = i;
                fr++;
                if (curr.x == vrts.Length - 1) {
                    bglns[curr.x] = new Vector2Int(bg, frtos.Length - bg);
                }
            }
        }

        vrtBuff = new GraphicsBuffer(Target.Vertex, vrts.Length, sizeof(float) * 3);
        colBuff = new GraphicsBuffer(Target.Structured, vrts.Length, sizeof(float));
        idxBuff = new GraphicsBuffer(Target.Structured, frtos.Length, sizeof(int) * 3);
        tblBuff = new GraphicsBuffer(Target.Structured, bglns.Length, sizeof(int) * 2);
        idxBuff.SetData(frtos);
        tblBuff.SetData(bglns);
        vrtBuff.SetData(vrts);
    }

    void Update() {
        var k = cs.FindKernel("CalcCurvature");
        cs.SetBuffer(k, "_Vrts", vrtBuff);
        cs.SetBuffer(k, "_Idxs", idxBuff);
        cs.SetBuffer(k, "_Table", tblBuff);
        cs.SetBuffer(k, "_Curvatures", colBuff);
        ComputeShaderUtil.Dispatch1D(cs, k, len);
    }

    void OnRenderObject() {
        mat.SetFloat("_ColorScale", colorScale);
        mat.SetBuffer("_Curvature", colBuff);
    }

    void OnDestroy() {
        idxBuff.Dispose();
        tblBuff.Dispose();
        colBuff.Dispose();
        vrtBuff.Dispose();
    }
}
