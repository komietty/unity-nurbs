using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace kmty.NURBS {
    [CustomEditor(typeof(SurfaceHandler))]
    public class SurfaceHandlerEditor : Editor {
        protected int selectedId = -1;
        protected int order;
        protected List<Vector3> segments = new List<Vector3>();
        protected SurfaceHandler handler => (SurfaceHandler)target;
        protected Vector3 hpos => handler.transform.position;
        protected SurfaceCpsData data => handler.Data;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUILayout.Space(1);
            if (GUILayout.Button("Bake Mesh")) {
                if (!Directory.Exists(handler.BakePath)) Directory.CreateDirectory(handler.BakePath);
                var path = handler.BakePath + "/" + handler.BakeName + ".asset";
                CreateOrUpdate(handler.mesh, path);

            }
        }

        void OnSceneGUI() {
            if (!Application.isPlaying) return;
            var cps = handler.Data.cps;
            if (handler.Data.order != order) {
                handler.Reset();
                order = data.order;
            };

            for (int i = 0; i < cps.Count; i++) {
                var cp = cps[i];
                handler.surface.UpdateCP(data.Convert(i), new CP(hpos + cp.pos, cp.weight));
            }

            for(var i = 0; i < cps.Count; i++) {
                var wp = handler.transform.TransformPoint(cps[i].pos);
                var sz = HandleUtility.GetHandleSize(wp) * 0.1f;
                if (Handles.Button(wp, Quaternion.identity, sz, sz, Handles.CubeHandleCap)) {
                    selectedId = i;
                    Repaint();
                }
            }

            if (selectedId > -1) {
                var cp = cps[selectedId];
                var wp = handler.transform.TransformPoint(cp.pos);
                EditorGUI.BeginChangeCheck();
                var pos = Handles.DoPositionHandle(wp, Quaternion.identity);
                if (EditorGUI.EndChangeCheck()) {
                    cp.pos = handler.transform.InverseTransformPoint(pos);
                    cps[selectedId] = cp;
                    handler.UpdateMesh();
                    EditorUtility.SetDirty(handler.Data);
                }
            }

            if (handler.surface != null) {
                DrawLines();
                //DrawCurve();
            };
        }

        void DrawLines() {
            segments.Clear();
            // draw grid
            for (int x = 0; x < data.count.x; x++)
            for (int y = 0; y < data.count.y - 1; y++) {
                segments.Add(hpos + data.cps[data.Convert(x, y)].pos);
                segments.Add(hpos + data.cps[data.Convert(x, y + 1)].pos);
            }
            for (int y = 0; y < data.count.y; y++) 
            for (int x = 0; x < data.count.x - 1; x++) {
                segments.Add(hpos + data.cps[data.Convert(x, y)].pos);
                segments.Add(hpos + data.cps[data.Convert(x + 1, y)].pos);
            }
            Handles.color = Color.blue;
            Handles.DrawLines(segments.ToArray());

        }

        void DrawCurve() { 
            segments.Clear();
            var s = 0.01f;
            var t = Mathf.Sin(Time.time) * 0.5f + 0.5f;
            for (float y = 0; y <= 1f; y += s) {
                segments.Add(handler.GetCurve(t, y));
                segments.Add(handler.GetCurve(t, y + s));
            }
            for (float x = 0; x < 1f; x += s) {
                segments.Add(handler.GetCurve(x, t));
                segments.Add(handler.GetCurve(x+ s, t));
            }
            Handles.color = Color.red;
            Handles.DrawLines(segments.ToArray());
        }

        void CreateOrUpdate(Object newAsset, string assetPath) {
            var oldAsset = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
            if (oldAsset == null) {
                AssetDatabase.CreateAsset(newAsset, assetPath);
            } else {
                EditorUtility.CopySerializedIfDifferent(newAsset, oldAsset);
                AssetDatabase.SaveAssets();
            }
        }

    }
}
