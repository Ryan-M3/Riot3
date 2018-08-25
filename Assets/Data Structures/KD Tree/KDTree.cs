using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This contains the root node only, which requires some special consideration
/// since it is possible to have a tree with no nodes in it.
/// </summary>
public class KDTree<T> {
    public KDNode<T> root;
    public int k;

    public KDTree(int k) {
        this.k = k;
    }

    public void Add(float[] key, T value) {
        if (root == null)
            root = new KDNode<T>(k, 0, key, value);
        else
            root.Add(key, value);
    }

    public T Get(float[] key) {
        return root.Get(key);
    }

    public KDNode<T> Subtree(float[] key) {
        return root.Subtree(key);
    }

    public List<KDNode<T>> ToList() {
        return root == null ? new List<KDNode<T>>() : root.ToList();
    }

    public void Del(float[] key) {
        if (!KDNode<T>.SameKey(root.key, key)) {
            root.Del(key);
        }

        // If it's a leaf node, just null out the value...
        else if (root.L == null && root.R == null) {
            root = null;
        }

        // ...otherwise just replace that branch with a new one.
        else {
            var nodes = root.ToList();
            nodes.Remove(root);
            root = new KDNode<T>(k, 0, nodes[0].key, nodes[0].value);
            nodes.RemoveAt(0);
            foreach (var node in nodes) {
                root.Add(node.key, node.value);
            }
        }
    }

    public List<T> SearchArea(float[] center, float radius) {
        if (root == null)
            return new List<T>();
        var nodes = root.NodesInArea(center, radius).ToList();
        if (nodes.Count == 0)
            return new List<T>();
        else
            return nodes.Select(node => node.value).ToList();
    }
}
