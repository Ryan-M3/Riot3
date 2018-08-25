using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCamYLookAtCursor : MonoBehaviour {
    public Transform target;
    protected Vector3 toLookAt;

    public void Update () {
        toLookAt = target.position;
        toLookAt.y = -Camera.main.transform.rotation.eulerAngles.y;
        transform.LookAt(toLookAt);
    }
}
