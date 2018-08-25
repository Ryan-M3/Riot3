using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMaterial : TrackToGlobalUI {
    public MeshRenderer rend;
    public float emission;

    public void Update() {
        rend.sharedMaterial.SetColor("_Color", globalColor.GetColor());
        rend.sharedMaterial.SetColor("_EmissionColor", globalColor.GetColor() * emission);
    }
}
