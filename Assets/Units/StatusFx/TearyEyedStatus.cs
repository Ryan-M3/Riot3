using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TearyEyedStatus : IStatusFX {
    RioterAI rioterAI;
    Unit unit;
    int durLeft;
    float modAP;
    float modRnd;

    public TearyEyedStatus(Unit unit, int duration, float modAP, float modRnd) {
        durLeft     = duration;
        this.modAP  = modAP;
        this.modRnd = modRnd;
        unit.ApplyStatus(this);
    }

    public void OnApply(Unit unit) {
        rioterAI = unit.GetComponent<RioterAI>();
        this.unit = unit;
        if (rioterAI != null)
            rioterAI.randomness *= modRnd;
        else
            unit.AP.apModifier *= modAP;
    }

    public void Update() {
        durLeft--;
        if (durLeft == 0)
            unit.RemoveFx(this);
    }

    public void OnRemove() {
        if (rioterAI != null)
            rioterAI.randomness /= modRnd;
        else
            unit.AP.apModifier /= modAP;
    }

    public void UndoUpdate() {
        durLeft++;
    }
}
