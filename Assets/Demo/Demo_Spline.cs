using UnityEngine;
using System.Linq;
using kmty.NURBS;

namespace kmty.NURBS.Demo {
    public class Demo_Spline : MonoBehaviour {
        public int order;
        public int num;
        public float amp;
        public Material mat;
        public GameObject tracer;
        public GameObject cpPrefab;
        Spline spline;
        CP_Go[] gos;
        CP[] cps;

        void Start() {
            cps = Enumerable.Range(0, num - 1) .Select(i => new CP(new Vector3(i, Random.value * amp, Random.value * amp), 1)).ToArray();
            spline = new Spline(cps, order);
            gos = new CP_Go[cps.Length];
            for (int i = 0; i < cps.Length; i++) {
                var g = Instantiate(cpPrefab).GetComponent<CP_Go>();
                g.transform.position = cps[i].pos;
                gos[i] = g;
            }
        }

        void Update() {
            for (int i = 0; i < cps.Length; i++)
                spline.UpdateCP(i, new CP(gos[i].transform.position, gos[i].weight));
            tracer.transform.position = spline.GetCurve((Time.time / 3) % 1);
        }

        void OnRenderObject() {
            GL.PushMatrix();

            mat.SetPass(1);
            GL.Begin(GL.LINE_STRIP);
            for (int i = 0; i < cps.Length; i++) GL.Vertex(cps[i].pos);
            GL.End();

            mat.SetPass(0);
            GL.Begin(GL.LINE_STRIP);
            for (float t = 0; t < 1; t += 0.005f) GL.Vertex(spline.GetCurve(t));
            GL.End();

            GL.PopMatrix();
        }
    }
}
