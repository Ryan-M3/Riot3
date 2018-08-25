using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseGroup : MonoBehaviour {
    public List<GameObject> toClose = new List<GameObject>();

    public void Close() {
        foreach (GameObject go in toClose) {
            go.SetActive(false);
        }
    }

    public void CloseExcept(GameObject exception) {
        foreach (GameObject go in toClose) {
            if (go != exception) {
                go.SetActive(false);
            }
        }
    }

    public void Open() {
        foreach (GameObject go in toClose) {
            go.SetActive(true);
        }
    }

    public void OpenExcept(GameObject exception) {
        foreach (GameObject go in toClose) {
            if (go != exception) {
                go.SetActive(true);
            }
        }
    }
}
