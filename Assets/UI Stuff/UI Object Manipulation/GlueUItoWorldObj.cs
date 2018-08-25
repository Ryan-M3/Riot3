using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GlueUItoWorldObj : MonoBehaviour {
    public GameObject worldObj;

    public void Update() {
        transform.position = Camera.main.WorldToScreenPoint(worldObj.transform.position);
    }
}
