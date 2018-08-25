using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

[System.Serializable]
public class Field<T> {
    public T[,] data;
    public int width {
        get { return data.GetLength(0); }
    }
    public int depth {
        get { return data.GetLength(1); }
    }

    public Field(int width, int height) {
        data = new T[width, height];
    }

    public T this[Point pt] {
        get {
            int x = Mathf.RoundToInt(pt.x);
            int y = Mathf.RoundToInt(pt.y);
            return this[x, y];
        }

        set {
            int x = Mathf.RoundToInt(pt.x);
            int y = Mathf.RoundToInt(pt.y);
            this[x, y] = value;
        }
    }

    public T this[int i, int j] {
        get {
            return data[i, j];
        }

        set {
            data[i, j] = value;
        }
    }
}

public class TestField {
    [Test]
    public void TestIntIndexer() {
        Field<int[]> intField = new Field<int[]>(64, 256);
        int[] ints = new int[3] { 1, 2, 3 };
        intField[0, 0] = ints;
        Debug.Assert(intField[0, 0] == ints);

        Field<Vector3> vfield = new Field<Vector3>(123, 654);
        vfield[10, 10] = Vector3.one;
        vfield[10, 10] += Vector3.one * 2;
        Debug.Assert(vfield[10, 10] == Vector3.one * 3);
    }

    [Test]
    public void TestPtIndexer() {
        Field<int[]> intField = new Field<int[]>(64, 256);
        int[] ints = new int[3] { 1, 2, 3 };
        intField[new Point(0, 0)] = ints;
        Debug.Assert(intField[new Point(0, 0)] == ints);

        Field<Vector3> vfield = new Field<Vector3>(123, 654);
        vfield[new Point(10, 10)] = Vector3.one;
        vfield[new Point(10, 10)] += Vector3.one * 2;
        Debug.Assert(vfield[new Point(10, 10)] == Vector3.one * 3);
    }
}
