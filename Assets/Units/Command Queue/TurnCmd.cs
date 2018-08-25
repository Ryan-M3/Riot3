using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCmd : ICommand {
    Unit unit;
    int dir;

    public TurnCmd(Unit unit, int dir) {
        Debug.Assert(Mathf.Abs(dir) == 1);
        this.unit = unit;
        this.dir  = dir;
    }

    public bool CanDo() {
        bool hasAP = unit.AP.ap >= unit.turnCost;
        if (!hasAP)
            View.ShowMsg("Not enough AP.");
        return hasAP;
    }

    public void Do() {
        Vector3 euler = unit.transform.rotation.eulerAngles;
        euler.y += dir * 90f;
        unit.transform.rotation = Quaternion.Euler(euler);
    }

    public void Undo() {
        Vector3 euler = unit.transform.rotation.eulerAngles;
        euler.y -= dir * 90f;
        unit.transform.rotation = Quaternion.Euler(euler);
    }
}
