using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace kmty.NURBS {
    [CustomEditor(typeof(SurfaceCpsData))]
    public class SurfaceCpsDataEditor : Editor {

        public override void OnInspectorGUI() {
            var surf = (SurfaceCpsData)target;
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset")) {
                for (int y = 0; y < surf.count.y; y++) {
                    for (int x = 0; x < surf.count.x; x++) {
                        var p = new Vector3(x * surf.width.x, y * surf.width.y, Random.value);
                        var o = new Vector3(surf.size.x, surf.size.y) * 0.5f;
                        surf.cps[surf.Convert(x, y)] = new CP(p, 1);
                    }
                }
                EditorUtility.SetDirty(surf);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
