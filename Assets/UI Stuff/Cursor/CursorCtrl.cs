using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorCtrl : MonoBehaviour {
    public bool isPaused;
    public LayerMask groundLayer;
    public float lowerLevelY;
    public float upperLevelY;

    public void LateUpdate() {
        // Move cursor.
        if (!isPaused) {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 100, groundLayer)) {
                Cursor.visible = false;
                Vector3 tgt = hit.point;
                if (Input.GetKey(KeyCode.LeftShift))
                    tgt.y = tgt.y < upperLevelY ? lowerLevelY : upperLevelY;
                transform.position = tgt;
            }
            else {
                Cursor.visible = true;
            }
        }
    }
}
