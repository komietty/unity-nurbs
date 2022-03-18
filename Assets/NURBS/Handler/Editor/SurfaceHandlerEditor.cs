using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace kmty.NURBS {
    [CustomEditor(typeof(SurfaceHandler))]
    public class SurfaceHandlerEditor : Editor {
        protected int selectedId = -1;
        protected int order;
        protected List<Vector3> segments = new List<Vector3>();
        //protected SurfaceHandler handler => (SurfaceHandler)target;
        //protected Vector3 hpos => handler.transform.position;
        //protected SurfaceCpsData data => handler.Data;

        private Surface _surface;
        private int lx;
        private int ly;

        void OnEnable() {
            var handler = (SurfaceHandler)target;
            _surface?.Dispose();
            Init(handler.Data);
        }
        void onDisable() {
            _surface?.Dispose();
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUILayout.Space(1);
            //if (GUILayout.Button("Bake Mesh")) {
            //    if (!Directory.Exists(handler.BakePath)) Directory.CreateDirectory(handler.BakePath);
            //    var path = handler.BakePath + "/" + handler.BakeName + ".asset";
            //    CreateOrUpdate(handler.mesh, path);
            //}
        }

        void OnSceneGUI() {
            //if (!Application.isPlaying) return;
            var handler = (SurfaceHandler)target;
            var data = handler.Data;
            var hpos = handler.transform.position;
            if (segments.Count == 0) UpdateSegments(data, hpos);
            var cps = handler.Data.cps;
            if (handler.Data.order != order) {
                handler.Reset();
                order = data.order;
            };

            for (int i = 0; i < cps.Count; i++) {
                var cp = cps[i];
                //handler.surface.UpdateCP(data.Convert(i), new CP(hpos + cp.pos, cp.weight));
                //_surface.UpdateCP(data.Convert(i), new CP(hpos + cp.pos, cp.weight));
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
                    UpdateSegments(data, hpos);
                    EditorUtility.SetDirty(handler.Data);
                }
            }
            Handles.DrawLines(segments.ToArray());
            //Draw();
        }

        void Init(SurfaceCpsData data) {
            var cps = data.cps.Select(cp => new CP(cp.pos, cp.weight)).ToArray();
            lx = data.count.x;
            ly = data.count.y;
            _surface = new Surface(cps, data.order, lx, ly);
        }


        void UpdateSegments(SurfaceCpsData data, Vector3 hpos) {
            segments.Clear();
            for (int x = 0; x < data.count.x; x++) {
            for (int y = 0; y < data.count.y - 1; y++) {
                    segments.Add(hpos + data.cps[data.Convert(x, y)].pos);
                    segments.Add(hpos + data.cps[data.Convert(x, y + 1)].pos);
                }
                segments.Add(hpos + data.cps[data.Convert(x, data.count.y - 1)].pos);
                segments.Add(hpos + data.cps[data.Convert(x, 0)].pos);
            }
            for (int y = 0; y < data.count.y; y++) {
                for (int x = 0; x < data.count.x - 1; x++) {
                    segments.Add(hpos + data.cps[data.Convert(x, y)].pos);
                    segments.Add(hpos + data.cps[data.Convert(x + 1, y)].pos);
                }
                segments.Add(hpos + data.cps[data.Convert(data.count.x - 1, y)].pos);
                segments.Add(hpos + data.cps[data.Convert(0, y)].pos);
            }
        }

        void Draw() {

            if (_surface == null) return;
            var seg = 0.05f;
            for (float ty = 0; ty < 1 - seg; ty += seg) {
                for (float tx = 0; tx < 1 - seg; tx += seg) {
                    var f1 = _surface.GetCurve(tx, ty, out Vector3 v1);
                    var f2 = _surface.GetCurve(tx + seg, ty, out Vector3 v2);
                    Handles.color = Color.red;
                    if (f1 && f2) Handles.DrawLine(v1, v2);
                }
            }
            for (float tx = 0; tx < 1 - seg; tx += seg) {
                for (float ty = 0; ty < 1 - seg; ty += seg) {
                    var f1 = _surface.GetCurve(tx, ty, out Vector3 v1);
                    var f2 = _surface.GetCurve(tx, ty + seg, out Vector3 v2);
                    Handles.color = Color.red;
                    if (f1 && f2) Handles.DrawLine(v1, v2);
                }
            }
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
