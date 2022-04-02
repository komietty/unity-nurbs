using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace kmty.NURBS.Test {

    public class NURBS {
        [Test]
        public void SplinePasses() { }

        float CircleKnotVector(int j){
            var p1 = Mathf.PI;
            var ph = Mathf.PI * 0.5f;
            var p1h = Mathf.PI * 1.5f;
            var p2 = Mathf.PI * 2;
            var a = new float[] { 0, 0, 0, ph, ph, p1, p1, p1h, p1h, p2, p2, p2 };
            return a[j];
        }
    }
}
