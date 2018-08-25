using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorField : Field<Vector3> {
    public VectorField(int width, int height) : base(width, height) {
        // Code goes here.
    }

    public void Calculate(Board board) {
        ZeroOutField();
        AddGridObjFlow(board);
    }

    public void ZeroOutField() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < depth; j++) {
                data[i, j] = Vector3.zero;
            }
        }
    }

    public void AddGridObjFlow(Board board) {
        foreach (FlowObj flow in GameObject.FindObjectsOfType<FlowObj>()) {
            foreach (Point pt in flow.GetPtsInGridObj(flow.pos)) {
                if (!flow.traversible) {
                    continue;
                }

                if (pt.x < width && pt.y < depth && pt.x >= 0 && pt.y >= 0) {
                    try {
                        this[pt] += flow.defaultFlow;
                    } catch (System.IndexOutOfRangeException) {
                        Debug.LogError("Invalid Point Found in VectorField: " + pt + " coming from object: " + flow.gameObject.name);
                        pt.x = Mathf.RoundToInt(pt.x);
                        pt.y = Mathf.RoundToInt(pt.y);
                        this[pt] += flow.defaultFlow;
                    }
                }
            }
        }
    }

    public void AddToField(Field<Vector3> addend) {
        Debug.Assert(addend.data.GetLength(0) == data.GetLength(0));
        Debug.Assert(addend.data.GetLength(1) == data.GetLength(1));
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < depth; j++) {
                Vector3 v = addend[i, j];
                Debug.Assert(!float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z));
                this[i, j] += v;
            }
        }
    }

    /// <summary> Determine if a given point is in bounds. </summary>
    public bool InBounds(Point pt) {
        return pt.x >= 0 && pt.x < width && pt.y >= 0 && pt.y < depth;
    }

    public IEnumerable<Point> Neighbors(Point pt) {
        if (InBounds(pt.North))
            yield return pt.North;

        if (InBounds(pt.East))
            yield return pt.East;

        if (InBounds(pt.South))
            yield return pt.South;

        if (InBounds(pt.West))
            yield return pt.West;
    }

    public void SmoothFlow(float amt) {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < depth; j++) {
                Point pt = new Point(i, j);
                // This check is necessary so that smoothing
                // doesn't increase the number of affected
                // vectors.
                Vector3 smooth = this[pt];
                if (smooth == Vector3.zero)
                    continue;

                int count = 0;
                foreach (Point neighbor in Neighbors(pt)) {
                    smooth += this[neighbor] * amt;
                    count++;
                }
                smooth /= 1 + count * amt;
                this[pt] = smooth;
            }
        }
    }
}
