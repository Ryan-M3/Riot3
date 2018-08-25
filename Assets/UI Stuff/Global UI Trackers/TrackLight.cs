using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackLight : TrackToGlobalUI {
    Light lightTarget;

    public void Awake() {
        lightTarget = GetComponent<Light>();
    }

    public void Update() {
        lightTarget.color = globalColor.GetColor();
    }
}
