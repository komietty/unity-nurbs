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
                for (var y = 0; y < surf.divide.y; y++) {
                    for (var x = 0; x < surf.divide.x; x++) {
                        var p = new Vector3(x * surf.width.x, y * surf.width.y, Random.value);
                        var o = new Vector3(surf.size.x, y * surf.size.y) * 0.5f;
                        surf.cps[surf.Convert(x, y)] = new CP(p, 1);
                    }
                }
                EditorUtility.SetDirty(surf);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
