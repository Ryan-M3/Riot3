using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFireStatus : IStatusFX {
    public float spreadProb;
    public float burnoutProb;
    public int dmg;
    protected int nfires = 10;
    protected Unit unit;
    protected Board board;
    protected GameObject vfx;
    protected List<GameObject> fires = new List<GameObject>();

    public OnFireStatus(float spreadProb, float burnoutProb, int dmg, GameObject vfx) {
        this.spreadProb  = spreadProb;
        this.burnoutProb = burnoutProb;
        this.dmg = dmg;
        board = MonoBehaviour.FindObjectOfType<Board>();
        this.vfx = vfx;
    }

    public void OnApply(Unit unit) {
        for (int i = 0; i < nfires; i++) {
            fires.Add(GameObject.Instantiate(vfx));
            Vector3 pos = unit.transform.position;
            pos.y = 0.5f;
            pos.x += Random.Range(-0.5f, +0.5f);
            pos.z += Random.Range(-0.5f, +0.5f);
            fires[i].transform.position = pos;
            Vector3 euler = fires[i].transform.rotation.eulerAngles;
            euler.y += Random.Range(0, 360);
            fires[i].transform.rotation = Quaternion.Euler(euler);
        }
    }

    public void Update() {
        DmgTable dmgTbl = new DmgTable();
        dmgTbl.hp = dmg;
        unit.TakeDmg(dmgTbl);
        if (Random.value <= spreadProb) {
            List<Unit> units = board.units.Find(unit.pt, 1f);
            if (units.Count > 0) {
                var fireStatus = new OnFireStatus(spreadProb, burnoutProb, dmg, vfx);
                units[Random.Range(0, units.Count)].ApplyStatus(fireStatus);
            }
        }

        else if (Random.value <= burnoutProb) {
            unit.RemoveFx(this);
        }
    }

    public void UndoUpdate() {

    }

    public void OnRemove() {
        GameObject.Destroy(vfx);
    }
}
