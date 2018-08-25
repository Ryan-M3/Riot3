using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepulsiveObject : ModalObject {
    public float radius;
    public float strength;
    public Board board;
    public Field<Vector3> lastCircle;
    public bool automaticallyAddToBoard;
    protected Point lastPt;
    protected bool added;

    protected void Awake() {
        if (board == null)
            board = FindObjectOfType<Board>();

        if (automaticallyAddToBoard)
            AddToBoard();

        EnemyTurn  += OnTurnEnd;
        PlayerTurn += OnTurnEnd;
    }

    protected void OnTurnEnd() {
        Point thisPt = new Point(transform.position);
        if (lastPt == null) {
            AddToBoard();
        } else if (thisPt != lastPt) {
            RemoveFromBoard();
            AddToBoard();
        }
        lastPt = thisPt;
    }

    public Field<Vector3> GetCircle() {
        Field<Vector3> circle = new Field<Vector3>(board.width, board.height);
        Point ctrPt = new Point(transform.position);
        for (int i = 0; i < board.width; i++) {
            for (int j = 0; j < board.height; j++) {
                Point candidate = new Point(i, j);
                Point toHere = candidate - ctrPt;
                if (toHere.magnitude <= radius && toHere.magnitude >= float.Epsilon) {
                    toHere = toHere.normalized * strength / Mathf.Max(1f, toHere.magnitude);
                    circle[candidate] = toHere.ToV3();
                }
            }
        }
        return circle;
    }

    public void AddToBoard() {
        added = true;
        lastCircle = GetCircle();
        board.AddToField(GetCircle());
    }

    public Field<Vector3> Reversed(Field<Vector3> field) {
        Field<Vector3> rev = field;
        for (int i = 0; i < field.data.GetLength(0); i++) {
            for (int j = 0; j < field.data.GetLength(1); j++) {
                rev[i, j] *= -1;
            }
        }
        return rev;
    }

    public void RemoveFromBoard() {
        added = false;
        // Sometimes board is already deleted and we don't need to do this.
        if (board != null)
            board.AddToField(Reversed(lastCircle));
    }

    protected void OnDisable() {
        if (added)
            RemoveFromBoard();
    }

    protected void OnDestroy() {
        EnemyTurn  -= OnTurnEnd;
        PlayerTurn -= OnTurnEnd;
    }
}
