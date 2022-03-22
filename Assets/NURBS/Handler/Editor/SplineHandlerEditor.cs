using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace kmty.NURBS {
    [CustomEditor(typeof(SplineHandler))]
    public class SplineHandlerEditor : Editor {
        private Spline _spline;
        private int selectedId = -1;
        private int length;

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
            _spline = new Spline(cps, data.order, data.loop);
            _spline.CreateHodograph();
        }

        void Draw() {
            if (_spline == null) return;
            var seg = 0.003f;
            //for (int i = 0; i < _spline.cps.Length - 1; i++)
            //    Handles.DrawLine(_spline.cps[i].pos, _spline.cps[i + 1].pos);
            for (float t = _spline.min; t < _spline.max - seg; t += seg) {
                var f1 = _spline.GetCurve(t, out Vector3 v1);
                var f2 = _spline.GetCurve(t + seg, out Vector3 v2);
                var dr1 = _spline.GetDerivative(t);
                var dr2 = _spline.GetSecondDerivative(t);
                Handles.color = Color.cyan;
                Handles.DrawLine(v1, v1 + dr1 * 0.04f);
                Handles.color = Color.yellow;
                //Handles.DrawLine(v1, v1 + dr2 * 0.002f);
                Handles.color = Color.red;
                if (f1 && f2) Handles.DrawLine(v1, v2);
            }
        }
    }
}
