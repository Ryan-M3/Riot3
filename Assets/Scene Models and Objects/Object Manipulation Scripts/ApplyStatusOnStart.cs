using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyStatusOnStart : MonoBehaviour {
    public StatusFX_ScriptableObject statusFx;

    void Start () {
        Board board = FindObjectOfType<Board>();
        Unit unit = board.units.Get(new Point(transform.position));
        if (unit != null) {
            statusFx.Apply(unit);
        }
    }
}
