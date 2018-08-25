using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanAttackCar))]
public class RioterAI : MonoBehaviour {
    public float anger;
    public float fear;

    public float attackProb;
    public float vandalizeProb;
    public float randomness;
    public float sightDist;

    public Model model;
    public Ctrl ctrl;
    public Party playerParty;
    public EnemyCtrl ectrl;
    public Unit ownUnit;

    protected Point here {
        get {
            return new Point(transform.position);
        }
    }
    protected ProbabilityQueue<string> aiq = new ProbabilityQueue<string>();
    protected CanAttackCar carAttacker;
    protected bool isAttackingCar;

    protected void Awake() {
        ownUnit = GetComponent<Unit>();
        if (ectrl == null) ectrl = FindObjectOfType<EnemyCtrl>();
        if (model == null) model = FindObjectOfType<Model>();
        if (ctrl  == null) ctrl  = FindObjectOfType<Ctrl>();

        carAttacker = GetComponent<CanAttackCar>();

        // Did you know Enums don't implement IEquatable, despite the face that
        // strings do and there's a conversion convention from enums to strings?
        // C#s type system annoys me. D:<
        aiq.Enqueue("Equip"    , 0.900f);
        aiq.Enqueue("Move"     , 0.075f);
        aiq.Enqueue("Attack"   , 0.015f);
        aiq.Enqueue("Vandalize", 0.010f);
        aiq.Enqueue("AttackCar", carAttacker.TipCarProb());
    }

    public bool Move() {
        if (ownUnit.stunned)
            return true;

        Point here = new Point(transform.position);
        // At the moment, the rioters will only move about.
        List<Point> adjacent = new List<Point>();
        foreach (Point adj in here.Adjacent()) {
            if (model.board.units.Has(adj))
                continue;

            if (model.board.IsOccupied(adj))
                continue;

            if (!model.board.InBounds(adj))
                continue;

            adjacent.Add(adj);
        }
        if (adjacent.Count == 0)
            return false;

        Vector3 ptsTo = model.board.GetFlow(new Point(transform.position));
        ptsTo = here.ToV3() + ptsTo;

        adjacent = adjacent
                  .OrderBy(adj => Vector3.Distance(adj.ToV3(), ptsTo)
                                + Random.value * randomness)
                  .ToList();

        for (int i = 0; i < adjacent.Count; i++) {
            if (model.board.units.Has(adjacent[i]))
                continue;
            MoveCmd mv = new MoveCmd(ownUnit, model, adjacent[i]);
            mv.verbose = false;
            if (mv.CanDo()) {
                ectrl.cmdQ.Add(mv);
                ectrl.cmdQ.ExecuteCmds();
                return true;
            }
        }

        return false;
    }

    public bool EquipPowerup() {
        List<Powerup> powerups = model.board.powerups.Find(here, 1f);
        if (powerups.Count > 0) {
            Debug.Log("equipping powerup");
            Powerup powerup = powerups[Random.Range(0, powerups.Count - 1)];
            ownUnit.Equip(powerup.Collect());
            if (powerup.ammo > 0)
                ownUnit.AddAmmo(ownUnit.equippedWeapon, powerup.ammo);
            if (ownUnit.Ammo(ownUnit.equippedWeapon) == 0)
                return false;
            ownUnit.AP.Reduce(1);
            return true;
        }
        return false;
    }

    public bool AttackUnits() {
        if (ownUnit.equippedWeapon == null)
            return false;

        if (ownUnit.Ammo(ownUnit.equippedWeapon) < 1f)
            return false;

        List<Unit> inRange = model.board.units.Find(ownUnit.pt, ownUnit.equippedWeapon.range);
        inRange.Distinct();
        inRange.TrimExcess();
        inRange = inRange.Where(unit => unit.tag == "Player").ToList();
        if (inRange.Count == 0)
            return false;

        Unit tgt = inRange[Random.Range(0, inRange.Count - 1)];
        ctrl.EvaluateCmd(new AtkCmd(ownUnit, model, tgt.pt));

        return true;
    }

    public bool Vandalize() {
        if (ownUnit.equippedWeapon == null)
            return false;

        if (ownUnit.Ammo(ownUnit.equippedWeapon) < 1f)
            return false;

        List<BreakableGlass> inRange = model.board.glass.Find(ownUnit.pt, ownUnit.equippedWeapon.range);
        inRange.Distinct();
        inRange.TrimExcess();
        inRange = inRange.Where(unit => unit.tag == "Player").ToList();
        if (inRange.Count == 0)
            return false;

        BreakableGlass tgt = inRange[Random.Range(0, inRange.Count - 1)];
        Point tgtPt = new Point(tgt.transform.position);
        ctrl.EvaluateCmd(new AtkCmd(ownUnit, model, tgtPt));

        return true;
    }

    public void StopAttackingCar() {
        isAttackingCar = false;
    }

    public void DoTurn() {
        if (ownUnit.busy)
            return;

        if (isAttackingCar) {
            carAttacker.Attack();
            return;
        }

        while (ownUnit.AP.ap > 1) {
            var action = aiq.Dequeue();
            switch (action.item) {
                case "Move":
                    Move();
                    break;

                case "Attack":
                    AttackUnits();
                    break;

                case "Vandalize":
                    Vandalize();
                    break;

                case "Equip":
                    EquipPowerup();
                    break;

                case "AttackCar":
                    isAttackingCar = carAttacker.Attack();
                    break;

                default:
                    Debug.Log("Invalid RioterAI action called.");
                    break;
            }
            aiq.Enqueue(action);
        }
    }
}
