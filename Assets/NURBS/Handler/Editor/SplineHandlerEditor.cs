using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace kmty.NURBS {
    [CustomEditor(typeof(SplineHandler))]
    public class SplineHandlerEditor : Editor {
        protected int selectedId = -1;
        public Spline spline { get; set; }
        protected int length;

        void OnEnable() {
            var handler = (SplineHandler)target;
            Init(handler.Data);
        }

        void OnSceneGUI() {
            var handler = (SplineHandler)target;
            var cps  = handler.Data.cps;
            if (cps.Count != length) Init(handler.Data);

            for (int i = 0; i < length; i++) {
                var cp = handler.Data.cps[i];
                spline.UpdateCP(i, new CP(handler.transform.position + cp.pos, cp.weight));
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
                    EditorUtility.SetDirty(handler.Data);
                }
            }

            Draw();
        }

        void Init(SplineCpsData data) {
            var cps = data.cps.ToArray();
            length = cps.Length;
            spline = new Spline(cps, data.order);
        }

        void Draw() {
            if (spline == null) return;
            var seg = 0.005f;
            for (float t = 0; t < 1 - seg; t += seg)
                Handles.DrawLine(spline.GetCurve(t), spline.GetCurve(t + seg));
        }
    }
}
