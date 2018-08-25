using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMaterialLeaky : TrackToGlobalUI {
    public Material mat;
    public float emission;

    public void Update() {
        mat.SetColor("_Color", globalColor.GetColor());
        mat.SetColor("_EmissionColor", globalColor.GetColor() * emission);
    }
}
