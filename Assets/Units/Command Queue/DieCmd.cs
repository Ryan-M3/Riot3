using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieCmd : ICommand {
    Unit unit;

    public DieCmd(Unit unit) {
        this.unit = unit;
    }

    public bool CanDo() {
        return unit.HP.hp <= 0;
    }

    public void Do() {
        unit.Die();
    }

    public void Undo() {
        unit.Resurrect();
    }
}
