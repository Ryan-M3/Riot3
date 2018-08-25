using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackRotation : MonoBehaviour {
    public Transform track;
    public bool x, y, z;

    public void Update() {
        Vector3 rotation = transform.rotation.eulerAngles;
        if (x) rotation.x = track.rotation.eulerAngles.x;
        if (y) rotation.y = track.rotation.eulerAngles.y;
        if (z) rotation.z = track.rotation.eulerAngles.z;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
