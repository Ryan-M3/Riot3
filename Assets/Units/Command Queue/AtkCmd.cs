using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkCmd : ICommand {
    public Unit atkUnit;
    public Model model;
    public Point tgtPt;

    protected IAttackable attacked;

    public AtkCmd(Unit unit, Model model, Point tgtPt) {
        Debug.Assert(unit  != null);
        Debug.Assert(model != null);

        this.atkUnit  = unit;
        this.model    = model;
        this.tgtPt    = tgtPt;

        var attackables = model.board.GetAttackable(tgtPt);
        if (attackables.Count > 0)
            attacked = attackables[Random.Range(0, attackables.Count)];
    }

    ///// <summary>
    ///// If we draw a 1x1 square around the Coord beg, then
    ///// draw a line from beg to end. what is the x, y coord-
    ///// inate along that line which also intersects with the
    ///// square we drew?
    ///// </summary>
    public static Point IntersectsGrid(Point beg, Point end) {
        float denom = (beg - end).magnitude;
        if (float.IsNaN(denom) || float.IsInfinity(denom))
            denom = 1f;
        return Point.Lerp(beg, end, 0.5f / denom);
    }

    public bool CanDo() {
        // Target is a unit.
        if (atkUnit.equippedWeapon.requiresUnitTarget && attacked == null) {
            View.ShowMsg("There's no one there.");
            return false;
         }

        // Target is self.
        Point beg = new Point(atkUnit.transform.position);
        if (beg == tgtPt) {
            View.ShowMsg("You can't attack yourself.");
            return false;
        }

        // Enough AP.
        bool hasAP = atkUnit.AP.ap >= atkUnit.equippedWeapon.ap_cost;
        if (!hasAP) {
            View.ShowMsg("Not enough Action Points.");
            return false;
        }

        // In range.
        bool inRange = atkUnit.equippedWeapon.range >= Point.Distance(beg, tgtPt);
        if (!inRange) {
            View.ShowMsg("Target out of range.");
            return false;
        }

        // In sight.
        if (atkUnit.equippedWeapon.needsSight) {
            Vector3 src = IntersectsGrid(beg, tgtPt).ToV3();
            Vector3 tgt = tgtPt.ToV3();
            if (!Physics.Linecast(src, tgt)) {
                View.ShowMsg("You don't have a clean shot.");
                return false;
            }
        }

        return true;
    }

    public void Do() {
        atkUnit.Attack(tgtPt);
        if (attacked != null) {
            DmgTable dmg = atkUnit.equippedWeapon.dmgTbl;
            attacked.TakeDmg(dmg);
            View.ShowDmgTbl(atkUnit.Pos(), dmg);
        }
    }

    public void Undo() {
        atkUnit.equippedWeapon.Unuse(atkUnit);
        if (attacked != null) {
            attacked.UndoDmg();
        }
    }
}
