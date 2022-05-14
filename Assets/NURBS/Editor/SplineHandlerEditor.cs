using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace kmty.NURBS {
    [CustomEditor(typeof(SplineHandler))]
    public class SplineHandlerEditor : Editor {
        protected Spline spline;
        protected int length;
        protected List<int> idcs = new List<int>();

        void OnEnable() {
            var hdl = (SplineHandler)target;
            Init(hdl.Data);
        }

        void OnSceneGUI() {
            var h = (SplineHandler)target;
            var cps = h.Data.cps;
            var e = Event.current.type;
            var q = Quaternion.identity;
            var selected = false;
            if (cps.Count != length) Init(h.Data);

            Handles.zTest = CompareFunction.Less;
            Handles.color = Color.white;

            for (int i = 0; i < length; i++) {
                var c = h.Data.cps[i];
                spline.SetCP(i, new CP(h.transform.position + c.pos, c.weight));
            }

            for(var i = 0; i < cps.Count; i++) {
                var w = h.transform.TransformPoint(cps[i].pos);
                var s = Mathf.Min(HandleUtility.GetHandleSize(w) * 0.1f, 0.03f);
                if (Handles.Button(w, q, s, s, Handles.SphereHandleCap)) {
                    idcs.Add(i);
                    selected = true;
                    Repaint();
                }
            }

            if (e == EventType.MouseUp && !selected) idcs.Clear();
            Handles.zTest = CompareFunction.Always;
            Handles.color = Color.HSVToRGB(30f / 360, 1, 1);

            if (idcs.Count > 0) {
                var sum = Vector3.zero;
                foreach(var i in idcs){
                    var w = h.transform.TransformPoint(cps[i].pos);
                    var s = Mathf.Min(HandleUtility.GetHandleSize(w) * 0.1f, 0.03f);
                    sum += w;
                    Handles.SphereHandleCap(0, w, q, s, Event.current.type);
                }
                EditorGUI.BeginChangeCheck();
                var d = sum / idcs.Count;
                var p = Handles.DoPositionHandle(d, q);
                if (EditorGUI.EndChangeCheck()) {
                    foreach (var i in idcs) {
                        var c = cps[i];
                        c.pos += h.transform.InverseTransformPoint(p - d);
                        cps[i] = c;
                    }
                    EditorUtility.SetDirty(h.Data);
                }
            }

            Draw(spline, h);
        }

        void Init(SplineCpsData data) {
            var hdl = (SplineHandler)target;
            var trs = hdl.transform;
            var cps = data.cps.Select(c => new CP(trs.TransformPoint(c.pos), c.weight)).ToArray();
            length = cps.Length;
            spline = new Spline(cps, data.order, data.type);
        }

        void Draw(Spline s, SplineHandler h) {
            var seg = 0.003f;
            if (h.showSegments) {
                Handles.color = Color.grey;
                for (int i = 0; i < s.cps.Length - 1; i++)
                    Handles.DrawLine(s.cps[i].pos, s.cps[i + 1].pos);
            }
            for (float t = 0; t < 1 - seg; t += seg) {
                s.GetCurve(t, out Vector3 va);
                s.GetCurve(t + seg, out Vector3 vb);
                s.GetFirstDerivative(t, out Vector3 d1);
                s.GetSecondDerivative(t, out Vector3 d2);
                var c1 = d1 * 0.1f + Vector3.one * 0.5f;
                var c2 = d2 * 0.01f + Vector3.one * 0.5f;
                if (h.show1stDerivative) {
                    Handles.color = new Vector4(c1.x, c1.y, c1.z, 1);
                    Handles.DrawLine(va, va + d1 * 0.05f);
                 }
                if (h.show2ndDerivative) {
                    Handles.color = new Vector4(c2.x, c2.y, c2.z, 1);
                    Handles.DrawLine(va, va + d2 * 0.001f);
                 }
                Handles.color = Color.cyan;
                Handles.DrawLine(va, vb);
            }
        }
    }
}
