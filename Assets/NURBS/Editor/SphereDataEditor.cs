using UnityEngine;
using UnityEditor;

namespace kmty.NURBS {
    [CustomEditor(typeof(SphereData))]
    public class SphereDataEditor : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var sf = (SurfaceCpsData)target;
            var pi = Mathf.PI;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate")) {

                var y0 = 0;
                for (int x = 0; x < sf.count.x; x++) 
                    sf.cps[sf.Convert(x, y0)] = new CP(new Vector3(0, 0, (y0 + 1) * sf.width.y), 1);

                var y1 = sf.count.y - 1;
                for (int x = 0; x < sf.count.x; x++) 
                    sf.cps[sf.Convert(x, y1)] = new CP(new Vector3(0, 0, (y1 - 1) * sf.width.y), 1);
                
                for (int y = 1; y < sf.count.y - 1; y++) {
                    for (int x = 0; x < sf.count.x; x++) {
                        var px = Mathf.Cos(2 * pi * x / (float)sf.count.x);
                        var py = Mathf.Sin(2 * pi * x / (float)sf.count.x);
                        var pz = y * sf.width.y;
                        var p = new Vector3(px, py, pz);
                        sf.cps[sf.Convert(x, y)] = new CP(p, 1);
                    }
                }
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}