using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowArc : MonoBehaviour {
    public Vector3 tgt;
    public float angle;
    public float speed;
    public bool useRB;
    public bool destroyOnImpact;
    public GameObject impactFX;
    protected ProjectileArc arc;
    protected float t;
    protected Rigidbody rb;

    protected bool dmgOnImpact;
    protected int dmg;
    protected Point pt;

    protected void Awake() {
        transform.SetParent(null);
        if (useRB) {
            rb = GetComponent<Rigidbody>();
        }
    }

    protected void Start () {
        Vector3 here = transform.position;
        if (useRB) {
            float speed = ProjectileArc.InitSpeed(here, tgt, angle);
            rb.velocity = (tgt - here).normalized * speed + Vector3.up * 2f;
        } else {
            arc = new ProjectileArc(here, tgt, angle);
        }
    }
    
    protected void Update () {
        if (useRB)
            return;
        t += Time.deltaTime * speed;
        transform.position = arc.Get(t);
        if (transform.position.y <= 0f) {
            if (impactFX != null) {
                Instantiate(impactFX).transform.position = transform.position + Vector3.up;
                if (dmgOnImpact) {
                    View.ShowDmg(pt, dmg);
                }
            }

            if (destroyOnImpact)
                gameObject.SetActive(false);
        }
    }

    public void CallDmgOnImpact(Point pt, int dmg) {
        this.pt     = pt;
        this.dmg    = dmg;
        dmgOnImpact = true;
    }
}
