using UnityEngine;
using UnityEditor;

namespace kmty.NURBS {
    [CustomEditor(typeof(PlaneData))]
    public class PlaneDataEditor : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var sf = (SurfaceCpsData)target;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate")) {
                for (int y = 0; y < sf.count.y; y++) {
                    for (int x = 0; x < sf.count.x; x++) {
                        var p = new Vector3(-x * sf.width.x, 0, y * sf.width.y);
                        var o = new Vector3(sf.size.x, sf.size.y) * 0.5f;
                        sf.cps[sf.Convert(x, y)] = new CP(p, 1);
                    }
                }
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}