using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : Stat<int> {
    public int hp;
    public Animator anim;

    public void Awake() {
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    public override void Reduce(int amt) {
        hp -= amt;
        if (hp <= 0)
            Die();
        AddToHistory(hp);
    }

    public override void UndoChange() {
        if (hp <= 0)
            Resurrect();
        UndoAdd();
    }

    public virtual void Die() {
        anim.SetTrigger("die");
    }

    public virtual void Resurrect() {
        anim.SetTrigger("resurrect");
    }
}
