using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace kmty.NURBS.Test {

    public class NURBS {
        [Test]
        public void SplinePasses() {
            var cps = new CP[]{
                new CP(new Vector3(1, 0, 0), 1),
                new CP(new Vector3(1, 1, 0), 1),
                new CP(new Vector3(0, 1, 0), 1),
                };
            var spline = new Spline(cps, 2, true, KnotType.OpenUniform);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NURBSWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
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
