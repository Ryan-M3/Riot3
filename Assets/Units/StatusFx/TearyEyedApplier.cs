using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Status Fx/Teary-Eyed")]
public class TearyEyedApplier : StatusFX_ScriptableObject {
    public int duration;
    public float modAP = 0.5f;
    public float modRnd = 2f;

    public override void Apply(Unit unit) {
        unit.ApplyStatus(new TearyEyedStatus(unit, duration, modAP, modRnd));
    }
}
