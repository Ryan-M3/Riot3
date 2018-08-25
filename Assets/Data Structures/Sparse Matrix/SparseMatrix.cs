using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A sparse 2D matrix that stores some value
/// of type T at a given 2D coordinate.
/// </summary>
public class SparseMatrix<T> {
    protected Dictionary<Point, T> mtx = new Dictionary<Point, T>();

    public List<Point> GetPts() {
        List<Point> pts = new List<Point>();
        foreach (Point pt in mtx.Keys)
            pts.Add(pt);
        return pts;
    }

    public T Get(Point c) {
        return mtx[c];
    }

    public bool Has(Point c) {
        return mtx.ContainsKey(c);
    }

    public void Set(Point c, T value) {
        mtx[c] = value;
    }

    public void Clear(Point c) {
        mtx.Remove(c);
    }

    public T this[int i, int j] {
        get {
            return Get(new Point(i, j));
        }
        set {
            Set(new Point(i, j), value);
        }
    }

    public T this[float i, float j] {
        get {
            return Get(new Point(i, j));
        }
        set {
            Set(new Point(i, j), value);
        }
    }

    public T this[Point c] {
        get {
            return Get(c);
        }

        set {
            Set(c, value);
        }
    }
}
