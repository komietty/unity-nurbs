using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace kmty.NURBS {
    [CustomEditor(typeof(SurfaceHandler))]
    public class SurfaceHandlerEditor : Editor {
        protected int selectedId = -1;
        protected int lenX;
        protected int lenY;
        protected int order;
        public Surface surface { get; set; }

        void OnEnable() {
            var handler = (SurfaceHandler)target;
            Init(handler.Data);
        }

        void OnSceneGUI() {
            var handler = (SurfaceHandler)target;
            var cps = handler.Data.cps;

            if (handler.Data.order != order) Init(handler.Data);

            for (int i = 0; i < cps.Count; i++) {
                var cp = handler.Data.cps[i];
                surface.UpdateCP(handler.Data.Convert(i), new CP(handler.transform.position + cp.pos, cp.weight));
            }

            for(var i = 0; i < cps.Count; i++) {
                var cp   = cps[i];
                var wp   = handler.transform.TransformPoint(cp.pos);
                var size = HandleUtility.GetHandleSize(wp) * 0.1f;
                if (Handles.Button(wp, Quaternion.identity, size, size, Handles.CubeHandleCap)) {
                    selectedId = i;
                    Repaint();
                }
            }

            if (selectedId > -1) {
                var cp   = cps[selectedId];
                var wp   = handler.transform.TransformPoint(cp.pos);
                EditorGUI.BeginChangeCheck();
                var pos = Handles.DoPositionHandle(wp, Quaternion.identity);
                if (EditorGUI.EndChangeCheck()) {
                    cp.pos = handler.transform.InverseTransformPoint(pos);
                    cps[selectedId] = cp;
                    EditorUtility.SetDirty(handler);
                }
            }

            Draw(handler.Data);
        }

        void Init(SurfaceCpsData data) {
            var cps = data.GetCps();
            lenX = cps.GetLength(0);
            lenY = cps.GetLength(1);
            order = data.order;
            surface = new Surface(cps, data.order);
        }

        void Draw(SurfaceCpsData data) {
            if (surface == null) return;
            var cps = data.GetCps();

            // draw grid
            List<Vector3> segments = new List<Vector3>();
            for (int x = 0; x < data.count.x; x++) {
                for (int y = 0; y < data.count.y - 1; y++) {
                    segments.Add(cps[x, y].pos);
                    segments.Add(cps[x, y + 1].pos);
                }
            }
            for (int y = 0; y < data.count.y; y++) {
                for (int x = 0; x < data.count.x - 1; x++) {
                    segments.Add(cps[x, y].pos);
                    segments.Add(cps[x + 1, y].pos);
                }
            }
            Handles.color = Color.blue;
            Handles.DrawLines(segments.ToArray());

            var seg = 0.05f;
            var handler = (SurfaceHandler)target;
            segments.Clear();
            for (float y = 0; y <= 1f; y += seg) {
                segments.Add(surface.GetCurve(handler.normalizedT(handler.checker, data.count.x), handler.normalizedT(y, data.count.y)));
                segments.Add(surface.GetCurve(handler.normalizedT(handler.checker, data.count.x), handler.normalizedT(y + seg, data.count.y)));
            }
            for (float x = 0; x < 1f; x += seg) {
                segments.Add(surface.GetCurve(handler.normalizedT(x, data.count.x), handler.normalizedT(handler.checker, data.count.y)));
                segments.Add(surface.GetCurve(handler.normalizedT(x+ seg, data.count.x), handler.normalizedT(handler.checker, data.count.y)));
            }
            Handles.color = Color.red;
            Handles.DrawLines(segments.ToArray());
        }
    }
}
