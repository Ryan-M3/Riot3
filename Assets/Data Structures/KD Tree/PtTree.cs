using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PtTree<T> : KDTree<T>, IBoardDataStruct<T> {
    public PtTree() : base(2) {
    }

    protected float[] ToArray(Point pt) {
        return new float[2] { pt.x, pt.y };
    }

    public void Add(Point pt, T value) {
        if (pt.x % 1 == 0.5f || pt.y % 1 == 0.5f)
            Debug.Log(value);
        Add(ToArray(pt), value);
    }

    public T Get(Point pt) {
        return Get(ToArray(pt));
    }

    public KDNode<T> Subtree(Point pt) {
        return Subtree(ToArray(pt));
    }

    public void Del(Point pt) {
        Del(ToArray(pt));
    }

    public bool Has(Point pt) {
        return root != null && root.HasKey(ToArray(pt));
    }

    public List<T> Find(Point center, float radius) {
        float[] asArray = new float[2] { center.x, center.y };
        return SearchArea(asArray, radius);
    }
}
