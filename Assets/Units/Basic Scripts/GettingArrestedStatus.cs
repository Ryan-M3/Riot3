using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GettingArrestedStatus : IStatusFX {
    Model model;
    Ctrl ctrl;
    Unit officer;
    Unit arrested;
    int APcost;

    /// <summary>
    /// AP Cost is the amount of AP it takes to successfully evade an arrest.
    /// </summary>
    public GettingArrestedStatus(Unit officer, int ap_cost, Model model, Ctrl ctrl) {
        this.APcost = ap_cost;
        this.model   = model;
        this.officer = officer;
        this.ctrl    = ctrl;
    }

    public void OnApply(Unit unit) {
        this.arrested = unit;
    }

    public void Update() {
        arrested.AP.ap -= APcost;
        if (arrested.AP.ap <= 0) {
            ctrl.EvaluateCmd(new ArrestCmd(officer, arrested.Pos(), model));
        }

        else {
            arrested.RemoveFx(this);
        }
    }

    public void UndoUpdate() {
        if (arrested.AP.ap <= 0) {
            ctrl.UndoCmd();
        }
        arrested.AP.ap += APcost;
    }

    public void OnRemove() {
    }
}
