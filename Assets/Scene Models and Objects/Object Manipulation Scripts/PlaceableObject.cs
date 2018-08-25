using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour {
    public Animator anim;

    public void Place(Point pt) {
        gameObject.transform.position = pt.ToV3();
        if (anim != null)
            anim.SetTrigger("place");
    }
}
