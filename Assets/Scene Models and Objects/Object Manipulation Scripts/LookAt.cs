using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {
    public Transform target;
    protected Vector3 toLookAt;

    public void Update () {
        toLookAt   = target.position;
        toLookAt.y = transform.position.y;
        transform.LookAt(toLookAt);
    }
}
