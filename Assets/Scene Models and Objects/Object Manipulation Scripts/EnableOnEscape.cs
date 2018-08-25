using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnEscape : MonoBehaviour {
    public List<GameObject> toEnable = new List<GameObject>();

    protected void Update() {
        if (Input.GetButtonDown("Cancel")) {
            foreach (GameObject go in toEnable)
                go.SetActive(true);
        }
    }
}
