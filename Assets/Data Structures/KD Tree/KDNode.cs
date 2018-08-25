using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KDNode<T> {
    public int k;           // number of dimensions this tree stores
    public int d;           // dimension we're splitting the data on
    public float[] key;     // k-dimensional vector
    public T value;         // data associated with the point/key
    public KDNode<T> L;     // left subtree
    public KDNode<T> R;     // right subtree

    public KDNode(int k, int d, float[] key, T value) {
        this.k     = k;
        this.d     = d;
        this.key   = key;
        this.value = value;
    }

    /// Insert a point with the associated value into the tree.
    public void Add(float[] key, T value) {
        // Grab the appropriate left or right node, then add entry.
        bool isLeft = key[d] < this.key[d];

        KDNode<T> node = isLeft ? L : R;
        if (node != null) {
            node.Add(key, value);
        }

        // Looks like the appropriate node was null, so we have to create it.
        else {
            node = new KDNode<T>(k, (d + 1) % key.Length, key, value);
            if (isLeft)
                L = node;
            else
                R = node;
        }
    }

    public bool HasKey(float[] key) {
        if (SameKey(key, this.key))
            return true;
        else if (L == null && R == null)
            return false;
        else if (key[d] < this.key[d])
            return L != null && L.HasKey(key);
        else
            return R != null && R.HasKey(key);
    }

    /// <summary> Compare two float arrays for piecewise equality. </summary>
    public static bool SameKey(float[] a, float[] b) {
        for (int i = 0; i < a.Length; i++) {
            if (Mathf.Abs(a[i] - b[i]) > Mathf.Epsilon)
                return false;
        }
        return true;
    }

    /// Retrieve that data stored in the tree at point, key.
    public T Get(float[] key) {
        if (key == null)
            throw new System.ArgumentNullException();

        if (SameKey(key, this.key)) {
            return value;
        }

        else if (key[d] < this.key[d]) {
            if (L == null) {
                throw new System.Exception("Key not found: " + key.ToString());
            }
            return L.Get(key);
        }

        else {
            if (R == null) {
                throw new System.Exception("Key not found: " + key.ToString());
            }
            return R.Get(key);
        }
    }

    /// Retrieve the subtree with the given key.
    public KDNode<T> Subtree(float[] key) {
        if (SameKey(key, this.key))
            return this;
        else if (key[d] < this.key[d])
            return L.Subtree(key);
        else
            return R.Subtree(key);
    }

    /// Delete the node with the given key from the tree.
    /// WARNING: simple, but inefficient
    public void Del(float[] key) {
        int dplus = (k + 1) % key.Length;
        // The L and R versions are otherwise identical.
        if (L != null && SameKey(L.key, key)) {
            // If it's a leaf node, just null out the value...
            if (L.L == null && L.R == null) {
                L = null;
            }

            // ...otherwise just replace that branch with a new one.
            else {
                var nodes = L.ToList();
                nodes.Remove(L);
                L = new KDNode<T>(k, dplus, nodes[0].key, nodes[0].value);
                nodes.RemoveAt(0);
                foreach (var node in nodes) {
                    L.Add(node.key, node.value);
                }
            }
        }

        else if (R != null && SameKey(R.key, key)) {
            // If it's a leaf node, just null out the value...
            if (R.L == null && R.R == null) {
                R = null;
            }

            // ...otherwise just replace that branch with a new one.
            else {
                var nodes = R.ToList();
                nodes.Remove(R);
                R = new KDNode<T>(k, dplus, nodes[0].key, nodes[0].value);
                nodes.RemoveAt(0);
                foreach (var node in nodes) {
                    R.Add(node.key, node.value);
                }
            }
        }

        else {
            if (key[d] < this.key[d])
                L.Del(key);
            else
                R.Del(key);
        }
    }

    /// <summary> List all nodes in no particular order. </summary>
    public List<KDNode<T>> ToList() {
        var nodes = new List<KDNode<T>>() { this };
        if (L != null)
            nodes.AddRange(L.ToList());
        if (R != null)
            nodes.AddRange(R.ToList());
        return nodes;
    }

    public List<KDNode<T>> NodesInArea(float[] center, float radius) {
        List<KDNode<T>> found = new List<KDNode<T>>();

        if (Dist(this.key, center) <= radius) {
            found.Add(this);
        }

        if (L != null) {
            // Does our search hypersphere cross the line Left is split on?
            if (Mathf.Abs(L.key[d] - center[d]) <= radius) {
                found.AddRange(L.NodesInArea(center, radius));
            }
        }

        if (R != null) {
            if (Mathf.Abs(R.key[d] - center[d]) <= radius) {
                found.AddRange(R.NodesInArea(center, radius));
            }
        }

        return found;
    }

    /// <summary> Pythagorean Theorem </summary>
    public static float Dist(float[] a, float[] b) {
        float sum = 0f;
        for (int i = 0; i < a.Length; i++) {
            sum += Mathf.Pow(a[i] - b[i], 2);
        }
        return Mathf.Sqrt(sum);
    }

    /// <summary>
    /// In typical KDTree explanations, there's a simple concept of a "line"
    /// you're splitting on and you 
    /// check if a rectangle representing the area intersecting with that line. But in k-dimensions,
    /// it's not a k - 1 hyperplane or anything. It just a
    /// matter of if you're looking for a range of values
    /// that is big enough you have to search both branches.
    /// </summary>
    public static bool CanContain(float[] queryCenter, int valueSplitOn, float radius) {
        throw new System.NotImplementedException();
    }
}
