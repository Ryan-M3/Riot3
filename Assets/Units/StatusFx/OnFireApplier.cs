using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Status Fx/On Fire")]
public class OnFireStatusApplier : StatusFX_ScriptableObject {
    public float spreadProb;
    public float burnoutProb;
    public int dmg = 1;
    public GameObject vfx;

    public override void Apply(Unit unit) {
        unit.ApplyStatus(new OnFireStatus(spreadProb, burnoutProb, dmg, vfx));
    }
}
