using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Status Fx/Adjacency Bonus")]
public class AdjacencyApplier : StatusFX_ScriptableObject {
    public int bonusAmt;
    public int dropoff;

    public override void Apply(Unit unit) {
        unit.ApplyStatus(new AdjacencyBonus(unit.board, unit, bonusAmt, dropoff));
    }
}
