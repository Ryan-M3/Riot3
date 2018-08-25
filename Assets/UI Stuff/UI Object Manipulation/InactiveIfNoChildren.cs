using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactiveIfNoChildren : MonoBehaviour {
    public GameObject parentObj;
    public List<GameObject> toInactivate = new List<GameObject>();
    protected bool isActivated;
    protected bool hasKids;

    protected void Start() {
        isActivated = toInactivate[0].activeInHierarchy;
    }

    protected void Update() {
        hasKids = parentObj.transform.childCount != 0;
        if (hasKids != isActivated) {
            foreach (var tgt in toInactivate) {
                tgt.SetActive(hasKids);
            }
            isActivated = hasKids;
        }
    }
}
