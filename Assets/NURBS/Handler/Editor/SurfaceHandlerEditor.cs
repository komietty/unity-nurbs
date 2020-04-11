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
        public Surface surface { get; set; }

        void OnEnable() {
            var handler = (SurfaceHandler)target;
            Init(handler.Data);
        }

        void OnSceneGUI() {
            var handler = (SurfaceHandler)target;
            var cps = handler.Data.cps;

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
            surface = new Surface(cps, data.order);
        }

        void Draw(SurfaceCpsData data) {
            if (surface == null) return;
            var cps = data.GetCps();

            // draw grid
            List<Vector3> segments = new List<Vector3>();
            for (int x = 0; x < data.divide.x; x++) {
                for (int y = 0; y < data.divide.y - 1; y++) {
                    segments.Add(cps[x, y].pos);
                    segments.Add(cps[x, y + 1].pos);
                }
            }
            for (int y = 0; y < data.divide.y; y++) {
                for (int x = 0; x < data.divide.x - 1; x++) {
                    segments.Add(cps[x, y].pos);
                    segments.Add(cps[x + 1, y].pos);
                }
            }
            Handles.color = Color.blue;
            Handles.DrawLines(segments.ToArray());

            var seg = 0.05f;
            var frc = (float)EditorApplication.timeSinceStartup % 1f;
            segments.Clear();
            for (float y = 0; y < 1f; y += seg) {
                segments.Add(surface.GetCurve(0.5f, y));
                segments.Add(surface.GetCurve(0.5f, y + seg));
            }
            for (float x = 0; x < 1f; x += seg) {
                segments.Add(surface.GetCurve(x, frc));
                segments.Add(surface.GetCurve(x + seg, frc));
            }
            Handles.color = Color.red;
            Handles.DrawLines(segments.ToArray());
        }
    }
}
