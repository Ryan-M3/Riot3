using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderBehind : MonoBehaviour {
    protected void Update () {
        gameObject.transform.SetAsFirstSibling();
    }
}
