using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Weapon")]
public class Weapon : UnitItem {
    public int ap_cost;

    public float range    = Mathf.Infinity;
    public float initAmmo = Mathf.Infinity;

    public bool requiresUnitTarget;
    public bool needsSight;

    public DmgTable dmgTbl;
    public GameObject vfx;

    protected string UpperCase(string s) {
        return char.ToUpper(itemName[0]) + itemName.Substring(1);
    }

    public virtual void Use(Unit user, Point tgt) {
        if (user.Ammo(this) < 1) {
            user.Equip(user.noWeapon);
            return;
        } else {
            user.AddAmmo(this, -1);
        }

        user.AP.Reduce(ap_cost);
        user.anim.SetTrigger("use" + UpperCase(itemName));

        if (vfx != null) {
            Vector3 halfway = (user.Pos() + tgt).ToV3() / 2f;
            Vector3 toTgt   = (user.Pos() - tgt).ToV3();
            Quaternion rot  = Quaternion.LookRotation(toTgt.normalized, Vector3.up);
            Instantiate(vfx, halfway, rot);
        }
    }

    public virtual void Unuse(Unit user) {
        user.AP.UndoChange();
        user.AddAmmo(this, 1f);
    }
}
