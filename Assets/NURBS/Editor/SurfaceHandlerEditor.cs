using System.Linq;
using UnityEngine;
using UnityEditor;

namespace kmty.NURBS {
    [CustomEditor(typeof(SurfaceHandler))]
    public class SurfaceHandlerEditor : Editor {
        protected int selectedId = -1;
        protected int order;
        protected bool xloop;
        protected bool yloop;
        protected SurfaceHandler handler => (SurfaceHandler)target;
        protected Vector3 hpos => handler.transform.position;
        protected SurfaceCpsData data => handler.Data;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUILayout.Space(1);
            if (GUILayout.Button("Bake Mesh")) {
                var path = $"{handler.BakePath}/{handler.BakeName}.asset";
                CreateOrUpdate(Weld(handler.mesh), path);
            }
        }

        void OnSceneGUI() {
            var handler = (SurfaceHandler)target;
            var data = handler.Data;
            var hpos = handler.transform.position;
            if (handler.segments.Count == 0) handler.UpdateSegments(data, hpos);
            var cps = handler.Data.cps;
            if (handler.Data.order != order || handler.Data.xloop != xloop || handler.Data.yloop != yloop) {
                if (Application.isPlaying) handler.Init();
                order = data.order;
                xloop = data.xloop;
                yloop = data.yloop;
            };

            if (Application.isPlaying) {
                for (int i = 0; i < cps.Count; i++) {
                    var cp = cps[i];
                    handler.surf.SetCP(data.Convert(i), new CP(hpos + cp.pos, cp.weight));
                }
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
                    if (Application.isPlaying) handler.UpdateMesh();
                    handler.UpdateSegments(data, hpos);
                    EditorUtility.SetDirty(handler.Data);
                }
            }
            Handles.color = Color.gray;
            Handles.DrawLines(handler.segments.ToArray());
        }

        void CreateOrUpdate(Object altAsset, string assetPath) {
            var oldAsset = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
            if (oldAsset == null) {
                AssetDatabase.CreateAsset(altAsset, assetPath);
            } else {
                EditorUtility.CopySerializedIfDifferent(altAsset, oldAsset);
                AssetDatabase.SaveAssets();
            }
        }

        Mesh Weld(Mesh original) {
            var ogl_vrts = original.vertices;
            var ogl_idcs = original.triangles;
            var alt_mesh = new Mesh();
            var alt_vrts = ogl_vrts.Distinct().ToArray();
            var alt_idcs = new int[ogl_idcs.Length];
            var vrt_rplc = new int[ogl_vrts.Length];
            for (var i = 0; i < ogl_vrts.Length; i++) {
                var o = -1;
                for (var j = 0; j < alt_vrts.Length; j++) {
                    if (alt_vrts[j] == ogl_vrts[i]) { o = j; break; }
                }
                vrt_rplc[i] = o;
            }

            for (var i = 0; i < alt_idcs.Length; i++) {
                alt_idcs[i] = vrt_rplc[ogl_idcs[i]];
            }
            alt_mesh.SetVertices(alt_vrts);
            alt_mesh.SetTriangles(alt_idcs, 0);
            alt_mesh.RecalculateBounds();
            alt_mesh.RecalculateNormals();
            alt_mesh.RecalculateTangents();
            return alt_mesh;
        }
    }
}
