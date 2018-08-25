using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour {
    public Unit curUnit {
        get {
            return playerParty.cur;
        }
    }
    public Point curPos {
        get {
            return new Point(playerParty.cur.transform.position);
        }
    }
    public Party playerParty;
    public Board board;

    protected View view;

    protected void Awake() {
        view = FindObjectOfType<View>();
    }
}
