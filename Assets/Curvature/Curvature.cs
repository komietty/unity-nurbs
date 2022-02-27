using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

public class Curvature : MonoBehaviour {
    [SerializeField, Range(1, 10)] protected float colorScale;
    GraphicsBuffer indexBuff;
    GraphicsBuffer tableBuff;
    GraphicsBuffer colorBuff;
    GraphicsBuffer vertexBuff;
    Material mat;

    struct frto {
        public int fr;
        public int to1;
        public int to2;
        public frto(int fr, int to1, int to2) {
            this.fr = fr;
            this.to1 = to1;
            this.to2 = to2;
        }
    }

    struct bgln {
        public int bg;
        public int ln;
        public bgln(int bg, int ln) { this.bg = bg; this.ln = ln; }
    }

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
        var mesh = Weld(filt.sharedMesh);
        var vrts = mesh.vertices.Select(v => v * 10000).ToArray();
        var tris = mesh.GetIndices(0);
        var frtos = new frto[tris.Length];
        var bglns = new bgln[vrts.Length];
        filt.mesh = mesh;
        mat = GetComponent<MeshRenderer>().sharedMaterial;

        Debug.Log(vrts.Length);
        Debug.Log(tris.Length);
        //foreach (var v in vrts) { Debug.Log(v); }

        for (var i = 0; i < tris.Length; i += 3) {
            var i0 = tris[i];
            var i1 = tris[i + 1];
            var i2 = tris[i + 2];
            frtos[i + 0] = new frto(i0, i1, i2);
            frtos[i + 1] = new frto(i1, i2, i0);
            frtos[i + 2] = new frto(i2, i0, i1);
        }

        frtos = frtos.OrderBy(ft => ft.fr).ToArray();

        var fr = 0;
        var bg = 0;
        for(var i = 0; i < frtos.Length; i++){
            var curr = frtos[i];
            if (curr.fr == fr + 1){
                bglns[fr] = new bgln(bg, i - bg);
                bg = i;
                fr++;
                if(curr.fr == vrts.Length - 1){
                    bglns[curr.fr] = new bgln(bg, frtos.Length - bg);
                }
            }
        }

        //test
        //foreach(var f in frtos){ Debug.Log("fr: " + f.fr + ", to1: " + f.to1 + ", to2: " + f.to2); }
        //foreach(var bl in bglns){ Debug.Log("bg: " + bl.bg + ", ln: " + bl.ln); }
        //Assert.IsTrue(bglns.Length == vrts.Length);
        var cols = new float[vrts.Length];
        for (var vid = 0; vid < vrts.Length; vid++) {
            var vrt = vrts[vid];
            var bgn = bglns[vid].bg;
            var len = bglns[vid].ln;
            var sum = 0f;
            for(var i = 0; i < len; i++){
                var frid = frtos[i + bgn].fr;
                var t1id = frtos[i + bgn].to1;
                var t2id = frtos[i + bgn].to2;
                var p0 = vrts[frid];
                var p1 = vrts[t1id];
                var p2 = vrts[t2id];
                var l1 = (p1 - p0).magnitude;
                var l2 = (p2 - p0).magnitude;
                if(l1 > 0 && l2 > 0){
                    var d1 = (p1 - p0) / l1;
                    var d2 = (p2 - p0) / l2;
                    var rad = Mathf.Acos(Vector3.Dot(d1, d2));
                    sum += rad;
                }
            }
            var k = (1f - sum / (Mathf.PI * 2)) * 12;
            cols[vid] = k;
            //Debug.Log( Mathf.FloorToInt(k * 10000) * 0.0001f);
        }

        //test

        indexBuff = new GraphicsBuffer(GraphicsBuffer.Target.Structured, frtos.Length, sizeof(int) * 3);
        tableBuff = new GraphicsBuffer(GraphicsBuffer.Target.Structured, bglns.Length, sizeof(int) * 2);
        indexBuff.SetData(frtos);
        tableBuff.SetData(bglns);

        vertexBuff = new GraphicsBuffer(GraphicsBuffer.Target.Structured, vrts.Length, sizeof(float) * 3);
        vertexBuff.SetData(vrts);

        colorBuff = new GraphicsBuffer(GraphicsBuffer.Target.Structured, cols.Length, sizeof(float));
        colorBuff.SetData(cols);

        mat.SetBuffer("indexBuff", indexBuff);
        mat.SetBuffer("tableBuff", tableBuff);
        mat.SetBuffer("colorBuff", colorBuff);
        mat.SetBuffer("vertexBuff", vertexBuff);
    }

    void OnDestroy(){
        indexBuff.Dispose();
        tableBuff.Dispose();
        colorBuff.Dispose();
        vertexBuff.Dispose();
    }

    void OnRenderObject() {
        mat.SetFloat("_ColorScale", colorScale);
     }
}
