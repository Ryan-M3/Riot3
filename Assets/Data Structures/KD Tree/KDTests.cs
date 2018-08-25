using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;


public class KDTests {
    KDTree<int> tree;
    List<Point> pts;

    protected float[] ToArray(Point pt) {
        return new float[2] { pt.x, pt.y };
    }

    protected bool AboutTheSame(float[] a, float[] b) {
        bool same = true;
        for (int i = 0; i < a.Length; i++) {
            same = same && Mathf.RoundToInt(a[i]) == Mathf.RoundToInt(b[i]);
        }
        return same;
    }

    [SetUp]
    public void SetUp() {
        // based on the data and results from https://www.youtube.com/watch?v=W94M9D_yXKk
        tree = new KDTree<int>(2);
        pts = new List<Point>() {
            new Point(23, 18),
            new Point(32, 14),
            new Point(16, 22),
            new Point(12,  3),
            new Point( 9, 19),
            new Point(21, 28),
            new Point(25, 11),
            new Point(47, 44),
            new Point(43, 29),
            new Point(29,  4)
        };

        for (int i = 0; i < 10; i++) {
            tree.Add(ToArray(pts[i]), i + 1);
        }
    }

    [Test]
    public void TestAdd() {
        Debug.Assert(AboutTheSame(ToArray(pts[0]), tree.root.key));
        Debug.Assert(AboutTheSame(ToArray(pts[1]), tree.root.R.key));
        Debug.Assert(AboutTheSame(ToArray(pts[2]), tree.root.L.key));
        Debug.Assert(AboutTheSame(ToArray(pts[3]), tree.root.L.L.key));
        Debug.Assert(AboutTheSame(ToArray(pts[4]), tree.root.L.L.L.key));
        Debug.Assert(AboutTheSame(ToArray(pts[5]), tree.root.L.R.key));
        Debug.Assert(AboutTheSame(ToArray(pts[6]), tree.root.R.L.key));
        Debug.Assert(AboutTheSame(ToArray(pts[7]), tree.root.R.R.key));
        Debug.Assert(AboutTheSame(ToArray(pts[8]), tree.root.R.R.L.key));
        Debug.Assert(AboutTheSame(ToArray(pts[9]), tree.root.R.L.R.key));
    }

    [Test]
    public void TestGet() {
        for (int i = 0; i < 10; i++) {
            Debug.Assert(tree.Get(ToArray(pts[i])) == i + 1);
        }
    }

    [Test]
    public void TestGetChildren() {
        List<float[]> kids = tree
                            .ToList()
                            .Select(node => node.key)
                            .ToList();
        foreach (float[] kid in kids) {
            Debug.Assert(pts.Contains(new Point(kid[0], kid[1])));
        }
    }

    [Test]
    public void TestGetInArea() {
        /* Remember that Rects are measured from the lower left
         * point, rather than the center for whatever reason.
         *
         * Test values were created by plotting the points in
         * excel columns and rows, selecting/highlighting rect-
         * angular regions and manually checking what points
         * were in there.
         */
        List<Point> inArea = tree.root
                            .NodesInArea(new float[2] { 18, 21 }, 4f)
                            .Select(node => new Point(node.key[0], node.key[1]))
                            .ToList();
        Debug.Assert(inArea.Count == 1);
        Debug.Assert(inArea.Contains(pts[2]));

        inArea = tree.root
                .NodesInArea(new float[2] { 18, 21 }, 6.5f)
                .Select(node => new Point(node.key[0], node.key[1]))
                .ToList();
        Debug.Assert(inArea.Count == 2);
        Debug.Assert(inArea.Contains(pts[0]));
        Debug.Assert(inArea.Contains(pts[2]));

        inArea = tree.root
                .NodesInArea(new float[2] { 18, 21 }, 9f)
                .Select(node => new Point(node.key[0], node.key[1]))
                .ToList();
        Debug.Assert(inArea.Count == 3);
        Debug.Assert(inArea.Contains(pts[0]));
        Debug.Assert(inArea.Contains(pts[2]));
        Debug.Assert(inArea.Contains(pts[2]));
    }

    [Test]
    public void TestRemove() {
        tree.Del(ToArray(pts[5]));
        List<KDNode<int>> kids = tree.ToList();
        Debug.Assert(kids.Count == 9);
    }

    [Test]
    public void TestRemoveRoot() {
        float[] rootPt = tree.root.key;
        tree.Del(rootPt);
        foreach (Point pt in pts) {
            if (!KDNode<int>.SameKey(ToArray(pt), rootPt)) {
                Debug.Assert(tree.Get(ToArray(pt)) == pts.IndexOf(pt) + 1);
            }
        }
    }
}
