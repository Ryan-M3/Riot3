using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackIsActive : MonoBehaviour {
    public GameObject source;
    public List<GameObject> targets = new List<GameObject>();
    protected bool active;

    protected void Update() {
        if (source.activeInHierarchy != active)
            foreach (GameObject tgt in targets) {
                tgt.SetActive(source.activeInHierarchy);

            active = source.activeInHierarchy;
        }
    }
}
