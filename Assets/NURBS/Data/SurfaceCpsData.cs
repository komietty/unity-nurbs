using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.NURBS {
    [CreateAssetMenu(menuName = "kmty/Create ControlPointsData/Surface", fileName = "SurfaceCpsData")]
    public class SurfaceCpsData : ScriptableObject {
        public int order;
        public Vector2Int size;
        public Vector2Int divide;
        public List<CP> cps;
        public CP[,] GetCps() {
            CP[,] _cps = new CP[divide.x, divide.y];
            for (int y = 0; y < divide.y; y++) {
                for (int x = 0; x < divide.x; x++) {
                    _cps[x, y] = cps[x + y * divide.x];
                }
            }
            return _cps;
        }

        public int count => divide.x * divide.y;
        public Vector2 width => new Vector2(size.x / (float)divide.x, size.y / (float)divide.y);
        public int Convert(int x, int y) => x + y * divide.x;
        public int Convert(Vector2Int v) => v.x + v.y * divide.x;
        public Vector2Int Convert(int i) => new Vector2Int(i % divide.x, Mathf.FloorToInt(i / (float)divide.x));
    }
}
