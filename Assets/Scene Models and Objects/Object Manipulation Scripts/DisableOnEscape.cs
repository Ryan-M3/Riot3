using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnEscape : MonoBehaviour {
    protected void Update() {
        if (Input.GetButtonDown("Cancel"))
            gameObject.SetActive(false);
    }
}
