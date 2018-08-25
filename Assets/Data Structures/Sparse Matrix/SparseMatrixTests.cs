using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class SparseMatrixTests {

    [Test]
    public void SparseMatrixTestsSimplePasses() {
        SparseMatrix<int> mtx = new SparseMatrix<int>();
        List<Point> pts = new List<Point>();
        for (int i = 0; i < 255; i++) {
            pts.Add(new Point(Random.Range(0, 99), Random.Range(5, 88)));
        }

        foreach (var pt in pts) {
            mtx.Set(pt, pts.IndexOf(pt));
        }

        foreach (var pt in pts) {
            Debug.Assert(mtx.Has(pt));
            int got = mtx.Get(pt);
            Debug.Assert(pts[got] == pt);
            Debug.Assert(mtx[pt.x, pt.y] == got);
        }

        foreach (var pt in pts) {
            mtx.Clear(pt);
            Debug.Assert(!mtx.Has(pt));
        }
    }
}
