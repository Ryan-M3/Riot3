using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridObj))]
[RequireComponent(typeof(MeshSetter))]
[RequireComponent(typeof(HP))]
[RequireComponent(typeof(AP))]
[RequireComponent(typeof(DP))]
public class Unit : ModalObject, IAttackable {
    public string unitName;
    public UnitType type;

    public int unitID;
    public int xp;
    public int intimidation;
    public int startAmmo;

    public float speed;
    public float turnCost;
    public float push {
        get {
            if (type == UnitType.rioter)
                return 0f;

            if (equippedWeapon == null)
                return intimidation;

            return (float)(equippedWeapon.dmgTbl.hp + intimidation);
        }
    }
    public float fear;
    public float anger;

    public bool busy;
    public bool stunned;
    public bool dead;

    public HP HP;
    public AP AP;
    public DP DP;

    public Party      party;
    public Board      board;
    public Animator   anim;
    public MeshSetter meshSetter;
    public Weapon     equippedWeapon;
    public Weapon     noWeapon;
    public Transform  projectileSpawn;
    public Point pt {
        get {
            return new Point(transform.position);
        }

        set {
            transform.position = value.ToV3();
        }
    }

    public List<StatusFX_ScriptableObject> statusFx = new List<StatusFX_ScriptableObject>();
    public List<UnitItem> equippedItems = new List<UnitItem>();

    protected List<IStatusFX> _istatusFx = new List<IStatusFX>();
    protected Dictionary<Weapon, float> ammo = new Dictionary<Weapon, float>();
    protected GridObj gridObj;

    // This is public becase Units can be created on the fly and may
    // need Awake manually called before filling out their details.
    public void Awake() {
        board = FindObjectOfType<Board>();
        gridObj = GetComponent<GridObj>();

        if (meshSetter == null)
            meshSetter = GetComponent<MeshSetter>();

        if (anim == null)
            anim = GetComponent<Animator>();

        if (HP == null)
            HP = GetComponent<HP>();

        if (AP == null)
            AP = GetComponent<AP>();

        if (DP == null)
            DP = GetComponent<DP>();

        PlayerTurn += OnPlayerTurn;

        meshSetter.GetDressed();

        if (equippedWeapon != null)
            ammo.Add(equippedWeapon, startAmmo);

        foreach (var statusObj in statusFx) {
            statusObj.Apply(this);
        }
    }

    /// <summary> How many action points is takes to travel along a given path. </summary>
    public int MoveCost(List<Point> path) {
        if (path.Count == 0)
            return 0;
        Point beg = path.First();
        Point end = path.Last();
        Point diff = end - beg;
        int manhattanX    = Mathf.Abs(Mathf.RoundToInt(diff.x));
        int manhattanY    = Mathf.Abs(Mathf.RoundToInt(diff.y));
        int manhattanDist = manhattanX + manhattanY;
        float numTurns = 0;
        for (int i = 1; i < path.Count - 2; i++) {
            Point lastMv = (path[i] - path[i - 1]).normalized;
            Point nextMv = (path[i + 1] - path[i]).normalized;
            // Did we make a turn?
            if (Mathf.RoundToInt(Point.SignedAngle(lastMv, nextMv)) == 90)
                numTurns++;
        }
        return Mathf.RoundToInt(numTurns * turnCost) + manhattanDist;
    }

    public void Move(List<Point> path, int apCost) {
        if (path.Count == 0)
            return;
        AP.Reduce(MoveCost(path));
        Point heading = (path.Last() - path[path.Count - 2]).normalized;
        ClearBoardPosition();
        StartCoroutine("AnimateMovement", path);
    }

    protected void ClearBoardPosition() {
        foreach (Point pt in GetOccupiedPoints()) {
            board.ClearTile(pt);
        }
    }

    /// <summary> 
    /// Get the points that this unit would occupy
    /// if it were centered around a given point.
    /// </summary>
    protected List<Point> GetOccupiedPoints(Point theoretical) {
        return gridObj.GetPtsInGridObj(theoretical);
        //List<Point> pts = new List<Point>();
        //if (gridSize == 0) {
        //    Debug.LogWarning("Is this unit supposed to occupy no space whatsoever?");
        //    return pts;
        //}
        //Point center  = theoretical;
        //Point fwd     = new Point(transform.forward);
        //Point startPt = center - (fwd * Mathf.Round(gridSize / 2f));
        //for (int i = 0; i < gridSize; i++) {
        //    pts.Add(startPt + fwd * i);
        //}
        //Debug.Assert(pts.Count == gridSize);
        //return pts;
    }

    /// <summary>
    /// Get every grid position that this unit currently occupies.
    /// </summary>
    public List<Point> GetOccupiedPoints() {
        return GetOccupiedPoints(Pos());
    }

    public void UndoMove(List<Point> path, int apCost) {
        throw new System.NotImplementedException();
    }

    public void TakeDmg(DmgTable dmgTbl) {
        HP.Reduce(Mathf.Max(0, dmgTbl.hp - DP.dp));
        AP.Reduce(dmgTbl.ap);
        DP.Reduce(dmgTbl.dp);
        fear  += dmgTbl.fear;
        anger += dmgTbl.anger;
    }

    public void UndoDmg() {
        HP.UndoChange();
        AP.UndoChange();
    }

    public void Die() {
        dead = true;
    }

    public void Resurrect() {
        dead = false;
    }

    public void AddAmmo(Weapon weapon, float amt) {
        if (!ammo.ContainsKey(weapon))
            ammo.Add(weapon, amt);
        else
            ammo[weapon] += amt;
    }

    public float Ammo(Weapon weapon) {
        return ammo.ContainsKey(weapon) ? ammo[weapon] : 0;
    }

    public void Stun(string stunnedBy) {
        anim.SetTrigger("getAttacked_" + stunnedBy);
        stunned = true;
    }

    public void Recover() {
        anim.SetTrigger("recover");
        stunned = false;
    }

    protected IEnumerator AnimateMovement(List<Point> path) {
        busy = true;
        anim.SetFloat("speed", speed);

        for (int i = 1; i < path.Count; i++) {
            var q = Quaternion.LookRotation(path[i].ToV3() - transform.position);
            for (float t = 0f; t < 1f; t += speed * Time.deltaTime) {
                Vector3 lerped = Vector3.Lerp(path[i - 1].ToV3(), path[i].ToV3(), t);
                transform.position = lerped;
                transform.rotation = Quaternion.Lerp(transform.rotation, q, t);
                yield return null;
            }
        }
        busy = false;
        anim.SetFloat("speed", 0f);
    }

    public void Equip(UnitItem item) {
        equippedItems.Add(item);
        meshSetter.Equip(item.meshPreset);
    }

    public void Equip(Weapon weapon) {
        meshSetter.Equip(weapon.meshPreset);
        equippedWeapon = weapon;
        AddAmmo(weapon, weapon.initAmmo);
    }

    public void UnequipWeapon() {
        equippedItems.Remove(equippedWeapon);
        Equip(noWeapon);
    }

    public void Unequip(UnitItem item) {
        equippedItems.Remove(item);
    }

    public void Attack(Point tgt) {
        equippedWeapon.Use(this, tgt);
    }

    public Point Pos() {
        return pt;
    }

    protected void OnPlayerTurn() {
        if (dead)
            return;
        foreach (IStatusFX fx in _istatusFx) {
            fx.Update();
        }
    }

    /// <summary>
    /// Adds and applies status to list of status effects.
    /// </summary>
    public void ApplyStatus(IStatusFX fx) {
        _istatusFx.Add(fx);
        fx.OnApply(this);
    }

    public void RemoveFx(IStatusFX fx) {
        _istatusFx.Remove(fx);
        fx.OnRemove();
    }
}
