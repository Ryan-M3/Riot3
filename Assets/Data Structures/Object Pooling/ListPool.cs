using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Provides an alternative way of creating object pools that
// sacrifices centralized control in estimating demand, and
// spreading out object instantiation across all pools.
public class ListPool : IPool {
    protected Stack<Poolable> contents = new Stack<Poolable>();
    protected GameObject prefab;
    protected int min;
    protected float rate;

    public ListPool(GameObject prefab, int min=0, float rate=2f) {
        Debug.Assert(prefab.GetComponent<Poolable>() != null);
        this.min  = min;
        this.rate = rate;
    }

    public Poolable Borrow() {
        if (contents.Count == 0)
            AddNew();
        var p = contents.Pop();
        p.motherpool = this;
        p.Reactivate();
        return p;
    }

    public void Return(Poolable poolable) {
        poolable.Deactivate();
        contents.Push(poolable);
    }

    protected void AddNew() {
        contents.Push(GameObject.Instantiate(prefab).GetComponent<Poolable>());
    }

    protected IEnumerable Populate() {
        while (contents.Count < min) {
            AddNew();
            yield return null;
        }
    }
}
