using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Projectile Weapon")]
public class ProjectileWeapon : Weapon {
    public GameObject projectilePrefab;
    public float angle;
    public float speed;
    protected Stack<GameObject> projected = new Stack<GameObject>();

    public override void Use(Unit user, Point tgt) {
        base.Use(user, tgt);
        GameObject clone = Instantiate(projectilePrefab);
        clone.transform.position = user.projectileSpawn.position;
        FollowArc farc = clone.GetComponent<FollowArc>();
        farc.angle = angle;
        farc.speed = speed;
        farc.tgt = tgt.ToV3();
        farc.CallDmgOnImpact(tgt, dmgTbl.hp);
        projected.Push(clone);

        var tr = clone.GetComponent<TrailRenderer>();
        if (tr != null)
            tr.Clear();
    }

    public override void Unuse(Unit user) {
        base.Unuse(user);
        Destroy(projected.Pop());
    }
}
