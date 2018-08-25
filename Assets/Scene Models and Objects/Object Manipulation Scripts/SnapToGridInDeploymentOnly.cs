using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SnapToGridInDeploymentOnly : ModalObject {
    public bool x, y, z;

    protected void Awake() {
        DeployUpdate += OnDeploymentUpdate;
    }

    protected void OnDeploymentUpdate() {
        Vector3 pos = transform.position;
        pos.x = x ? Mathf.RoundToInt(pos.x) : pos.x;
        pos.y = y ? Mathf.RoundToInt(pos.y) : pos.y;
        pos.z = z ? Mathf.RoundToInt(pos.z) : pos.z;
        transform.position = pos;
    }

    protected void OnDestroy() {
        DeployUpdate -= OnDeploymentUpdate;
    }
}
