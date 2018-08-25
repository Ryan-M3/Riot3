using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowObj : GridObj {
    protected Board board;
    public Vector3 defaultFlow = Vector3.zero;

    protected void Awake() {
        if (board == null)
            board = FindObjectOfType<Board>();

        if (!traversible) {
            foreach (Point pt in GetPtsInGridObj(pos)) {
                if (!board.IsOccupied(pt)) {
                    board.SetOccupied(pt);
                }
            }
        }
    }
}
