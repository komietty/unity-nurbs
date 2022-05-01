using UnityEngine;
using UnityEditor;

namespace kmty.NURBS {
    [CustomEditor(typeof(TorusData))]
    public class TorusDataEditor : Editor {

        public override void OnInspectorGUI() {
            var surf = (SurfaceCpsData)target;
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate")) {
                for (int y = 0; y < surf.count.y; y++) {
                    for (int x = 0; x < surf.count.x; x++) {
                        var pi = Mathf.PI;
                        var cosu = Mathf.Cos(2 * pi * x / (float)surf.count.x);
                        var sinu = Mathf.Sin(2 * pi * x / (float)surf.count.x);
                        var cosv = Mathf.Cos(2 * pi * y / (float)surf.count.y);
                        var sinv = Mathf.Sin(2 * pi * y / (float)surf.count.y);
                        var p = new Vector3(
                            (2 + cosv) * cosu,
                            (2 + cosv) * sinu,
                            sinv
                            );
                        surf.cps[surf.Convert(x, y)] = new CP(p, 1);
                    }
                }
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}