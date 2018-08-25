using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldVisualizer : MonoBehaviour {
    public Board board;
    public Mesh mesh;
    public float meshScale;
    public float smoothAmt;
    public int smoothIters;
    public float lineScale = 0.5f;
    [Range(1, 16)] public int res = 1;
    protected int lastRes = 0;

    protected void UpdateData() {
        // On the one hand, it's easier to read if I place all my conditions
        // out on separate lines. But the downside is that I don't get to
        // short-circuit width and height, so I have to check for null to get
        // that bool.
        if (board == null) {
            board = FindObjectOfType<Board>();
        }

        if (board.flowField == null) {
            board.flowField = new VectorField(board.width, board.height);
        }

        bool dirty = res != lastRes;

        if (dirty) {
            lastRes = res;
            dirty   = false;
            board.CalculateFlowField();
        }
    }

    protected Vector3 SumWindow(int begX, int begY) {
        Vector3 sum = Vector3.zero;
        for (int i = begX; i < begX + res; i++) {
            for (int j = begY; j < begY + res; j++) {
                sum += board.GetFlow(i, j);
            }
        }
        return sum;
    }

    public void Smooth() {
        for (int i = 0; i < smoothIters; i++) {
            board.SmoothFlow(smoothAmt);
        }
    }

    public void OnDrawGizmosSelected() {
        UpdateData();

        for (int i = 0; i < board.width - res; i += res) {
            for (int j = 0; j < board.height - res; j += res) {
                Vector3 average = SumWindow(i, j) / Mathf.Max(1f, res * res);
                if (average.magnitude == 0)
                    continue;

                // We want to place the start of the arrow at the center
                // of the window. The reason we need to subtract 0.5f is
                // because a resolution of 1 would offset the arrow by a
                // half.
                float begX = i + res / 2f - 0.5f;
                float begZ = j + res / 2f - 0.5f;
                Vector3 beg = new Vector3(begX, 1f, begZ);
                Vector3 end = beg + average * lineScale;
                end.y = beg.y;
                Quaternion look;
                look = Quaternion.LookRotation(beg - end);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(beg, end);
                Gizmos.DrawMesh(mesh, end, look, Vector3.one * meshScale);
            }
        }
    }
}
