using UnityEngine;
using kmty.NURBS;

namespace kmty.NURBS.Demo {
    public class Demo_Surface : MonoBehaviour {
        public int order;
        public float amp;
        public Vector2Int num;
        [Range(0.05f, 0.2f)] public float divX;
        [Range(0.02f, 0.2f)] public float divY;
        public Material mat;
        public GameObject tracer;
        public GameObject cpPrefab;
        public float tracerNoiseFac;
        Surface surface;
        CP_Go[,] gos;
        CP[,] cps;

        void Start() {
            cps = new CP[num.x, num.y];
            gos = new CP_Go[num.x, num.y];

            for (int y = 0; y < num.y; y++)
                for (int x = 0; x < num.x; x++) {
                    var p = new Vector3(x, y, Random.value * amp);
                    var g = Instantiate(cpPrefab).GetComponent<CP_Go>();
                    g.transform.position = p;
                    gos[x, y] = g;
                    cps[x, y] = new CP(p, 1f);
                }
            surface = new Surface(cps, order);

        }

        void Update() {
            var t = Time.time * 0.5f;
            tracer.transform.position = surface.GetCurve(Mathf.PerlinNoise(t, tracerNoiseFac), Mathf.PerlinNoise(tracerNoiseFac, t));

            for (int y = 0; y < num.y; y++)
                for (int x = 0; x < num.x; x++) 
                    surface.UpdateCP(new Vector2Int(x, y), new CP(gos[x, y].transform.position, gos[x, y].weight));
        }

        void OnRenderObject() {
            mat.SetPass(0);
            GL.PushMatrix();
            for (float x = 0; x <= 1f + divX; x += divX) {
                GL.Begin(GL.LINE_STRIP);
                for (float y = 0; y < 1f; y += divY) GL.Vertex(surface.GetCurve(x, y));
                GL.End();
            }
            for (float y = 0; y <= 1f + divX; y += divX) {
                GL.Begin(GL.LINE_STRIP);
                for (float x = 0; x < 1f; x += divY) GL.Vertex(surface.GetCurve(x, y));
                GL.End();
            }

            mat.SetPass(1);
            for (int x = 0; x < num.x; x++) {
                GL.Begin(GL.LINE_STRIP);
                for (int y = 0; y < num.y; y++) GL.Vertex(cps[x, y].pos);
                GL.End();
            }
            for (int y = 0; y < num.y; y++) {
                GL.Begin(GL.LINE_STRIP);
                for (int x = 0; x < num.x; x++) GL.Vertex(cps[x, y].pos);
                GL.End();
            }


            GL.PopMatrix();
        }
    }
}
