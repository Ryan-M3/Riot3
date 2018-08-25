using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SnapToGrid : MonoBehaviour {
    public bool x, y, z;

    protected void Update() {
        Vector3 pos = transform.position;
        pos.x = x ? Mathf.RoundToInt(pos.x) : pos.x;
        pos.y = y ? Mathf.RoundToInt(pos.y) : pos.y;
        pos.z = z ? Mathf.RoundToInt(pos.z) : pos.z;
        transform.position = pos;
    }
}
