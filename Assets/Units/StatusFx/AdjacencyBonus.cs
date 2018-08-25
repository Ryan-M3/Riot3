using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjacencyBonus : IStatusFX {
    protected int amt;
    protected int dropoff;
    protected Board board;
    protected int lastLastBonus;
    protected int lastBonus;
    protected Unit unit;
    protected bool undid = false;

    public AdjacencyBonus(Board board, Unit unit, int amt, int dropoff) {
        this.board   = board;
        this.unit    = unit;
        this.amt     = amt;
        this.dropoff = dropoff;
    }

    public void OnApply(Unit unit) {
    }

    public void Update() {
        float range = Mathf.Ceil(amt / dropoff);
        var units = board.units.Find(unit.pt, range);
        if (units.Contains(unit))
            units.Remove(unit);

        int bonus = 0;
        foreach (Point pt in units.Select(u => u.pt)) {
            if (pt == unit.pt)
                continue;
            float mdist = Point.ManhattanDistance(pt, unit.pt);
            bonus += Mathf.RoundToInt(amt - dropoff * mdist);
        }

        int diff = bonus - lastBonus;
        unit.DP.dpModPts += diff;
        lastBonus = bonus;
        undid = false;
    }

    public void OnRemove() {
        unit.DP.dpModPts -= lastBonus;
    }

    public void UndoUpdate() {
        if (undid) {
            Debug.LogError("Can't undo Update twice.");
        }
        int bonus = lastBonus;
        lastBonus = lastLastBonus;
        unit.DP.dpModPts += lastBonus - bonus;
        undid = true;
    }
}
