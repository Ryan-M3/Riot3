using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackToGlobalUI : MonoBehaviour {
    protected GlobalUIColor globalColor;

    protected virtual void Start() {
        globalColor = FindObjectOfType<GlobalUIColor>();
    }
}
