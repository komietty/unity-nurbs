using UnityEngine;
using UnityEditor;

namespace kmty.NURBS {
    [CustomEditor(typeof(CylinderData))]
    public class CylinderDataEditor : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var sf = (SurfaceCpsData)target;
            var pi = Mathf.PI;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate")) {
                for (int y = 0; y < sf.count.y; y++) {
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