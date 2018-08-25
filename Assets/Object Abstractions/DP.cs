using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Unit))]
public class DP : Stat<int> {
    public int dp {
        get {
            return CalculateDP();
        }
    }
    public float dpModPercent = 1f;
    public int dpModPts = 0;
    protected int unitDP;
    protected Unit unit;

    protected void Awake() {
        unit = GetComponent<Unit>();
    }

    public override void Reduce(int amt) {
        AddToHistory(dpModPts);
        dpModPts -= amt;
    }

    public override void UndoChange() {
        UndoAdd();
    }

    protected int CalculateDP() {
        int baseDP = unit.equippedItems.OfType<Armor>().Sum(a => a.dp);
        float moded = baseDP * dpModPercent;
        return Mathf.RoundToInt(moded) + dpModPts;
    }
}
