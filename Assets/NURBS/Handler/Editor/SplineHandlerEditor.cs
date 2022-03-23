using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace kmty.NURBS {
    [CustomEditor(typeof(SplineHandler))]
    public class SplineHandlerEditor : Editor {
        Spline _spline;
        int selectedId = -1;
        int length;

        void OnEnable() {
            var handler = (SplineHandler)target;
            Init(handler.Data);
        }

        //public override void OnInspectorGUI() {
        //    base.OnInspectorGUI();
        //    EditorGUILayout.Space(1);
        //    EditorGUILayout.Toggle("Show Segments", showSegments);
        //    EditorGUILayout.Toggle("Show 1st Derivative", show1stDerivative);
        //    EditorGUILayout.Toggle("Show 2nd Derivative", show2ndDerivative);
        //}

        void OnSceneGUI() {
            var handler = (SplineHandler)target;
            var cps  = handler.Data.cps;
            if (cps.Count != length) Init(handler.Data);

            for (int i = 0; i < length; i++) {
                var cp = handler.Data.cps[i];
                _spline.SetCP(i, new CP(handler.transform.position + cp.pos, cp.weight));
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
            var hdl = (SplineHandler)target;
            var trs = hdl.transform;
            var cps = data.cps.Select(cp => new CP(trs.TransformPoint(cp.pos), cp.weight)).ToArray();
            length = cps.Length;
            _spline = new Spline(cps, data.order, data.loop, data.knotType);
        }

        void Draw() {
            var seg = 0.005f;
            var handler = (SplineHandler)target;
            if (_spline == null) return;
            if (handler.showSegments) {
                Handles.color = Color.grey;
                for (int i = 0; i < _spline.cps.Length - 1; i++)
                    Handles.DrawLine(_spline.cps[i].pos, _spline.cps[i + 1].pos);
            }
            for (float t = 0; t < 1 - seg; t += seg) {
                var va = _spline.GetNorm2Curve(t);
                var vb = _spline.GetNorm2Curve(t + seg);
                var d1 = _spline.GetNorm2Derivative(t);
                var d2 = _spline.GetNorm2SecondDerivative(t);
                Handles.color = Color.cyan;
                if (handler.show1stDerivative) Handles.DrawLine(va, va + d1 * 0.04f);
                Handles.color = Color.yellow;
                if (handler.show2ndDerivative) Handles.DrawLine(va, va + d2 * 0.001f);
                Handles.color = Color.cyan;
                Handles.DrawLine(va, vb);
            }
        }
    }
}
