using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DressupCamera : MonoBehaviour {
    public Transform rotateAround;
    public float speed;
    public float inputBoost = 10f;
    protected float dir = 1;

    public void Update() {
        float h = Input.GetAxisRaw("Horizontal");
        // We want to keep the last direction the player inputted,
        // so we have to remember dir and switch it out only when
        // it's changed.
        if (h != 0 && Mathf.Sign(h) != dir)
            dir = Mathf.Sign(h);
        float speed_ = (h * inputBoost + dir) * Time.deltaTime * speed;
        transform.RotateAround(rotateAround.position, Vector3.up, speed_);
    }
}
