using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunCmd : ICommand {
    public Unit unit;
    public UnitItem item;

    public StunCmd(Unit unit, UnitItem item) {
        this.unit = unit;
        this.item = item;
    }

    public bool CanDo() {
        return unit != null;
    }

    public void Do() {
        unit.Stun(item.itemName);
    }

    public void Undo() {
        unit.Recover();
    }
}
