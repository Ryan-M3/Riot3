using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The number of action points and operations that alter those action points available.
/// </summary>
[RequireComponent(typeof(HP))]
public class AP : Stat<int> {
    public int ap;
    public int apRefreshRate;
    public float apModifier = 1f;
    public HP health;

    protected void Awake() {
        if (health == null)
            health = GetComponent<HP>();
        PlayerTurn += OnPlayerTurn;
    }

    protected void OnPlayerTurn() {
        if (health != null && health.hp < 1)
            return;
        ap += apRefreshRate;
        ap = Mathf.Min(ap, Mathf.RoundToInt(health.hp * apModifier));
    }

    public override void Reduce(int amt) {
        ap -= amt;
        AddToHistory(ap);
    }

    public override void UndoChange() {
        UndoAdd();
    }
}
