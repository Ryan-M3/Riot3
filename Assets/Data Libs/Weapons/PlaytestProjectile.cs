using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaytestProjectile : MonoBehaviour {
    public ProjectileWeapon weapon;
    public Unit srcUnit;
    public Unit tgtUnit;

    public void Start() {
        InvokeRepeating("Fire", 0f, 3f);
    }

    public void Fire() {
        weapon.Use(srcUnit, tgtUnit.Pos());
    }
}
