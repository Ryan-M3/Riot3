using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A reversible dictionary.
/// </summary>
public class RevDict<T1, T2> {
    protected Dictionary<T1, T2> forwards  = new Dictionary<T1, T2>();
    protected Dictionary<T2, T1> backwards = new Dictionary<T2, T1>();

    public RevDict() {
        if (typeof(T1) == typeof(T2)) {
            Debug.LogError("RevDict requires each type be different.");
        }
    }

    public bool Contains(T1 key) {
        return forwards.ContainsKey(key);
    }

    public bool Contains(T2 key) {
        return backwards.ContainsKey(key);
    }

    public void Add(T1 key, T2 value) {
        forwards .Add(key, value);
        backwards.Add(value, key);
    }

    public void Add(T2 key, T1 value) {
        forwards .Add(value, key);
        backwards.Add(key, value);
    }

    public T2 this[T1 key] {
        get {
            return forwards[key];
        }

        set {
            forwards[key]    = value;
            backwards[value] = key;
        }
    }

    public T1 this[T2 key] {
        get {
            return backwards[key];
        }

        set {
            backwards[key]   = value;
            forwards[value]  = key;
        }
    }
}
