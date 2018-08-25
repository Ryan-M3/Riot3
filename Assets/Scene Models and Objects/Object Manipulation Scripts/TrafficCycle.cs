using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficCycle : ModalObject {
    public Material redMat;
    public Light redLight;

    public Material yellowMat;
    public Light yellowLight;

    public Material greenMat;
    public Light greenLight;

    // Apparently 96:24 is most common for a major road,
    // 36:24 is used for when you want to give the roads
    // joining in equal footing.
    public float cycleLength = 120f;
    public float percentGreen = 96f - 4f;

    // There's a complicated formula for determining the
    // length of a yellow light, but we'll just pick one.
    public const float yellowDur = 5f;

    protected void Awake() {
        // green -> yellow -> red
        float yellowStart = cycleLength * percentGreen;
        float redStart    = cycleLength * percentGreen + yellowDur;
        InvokeRepeating("SetGreen",  0f         , cycleLength);
        InvokeRepeating("SetYellow", yellowStart, cycleLength);
        InvokeRepeating("SetRed"   , redStart   , cycleLength);
    }

    protected void SetYellow() {
        yellowMat.EnableKeyword("_EMISSION");
        yellowLight.enabled = true;

        redMat.DisableKeyword("_EMISSION");
        redLight.enabled = false;

        greenMat.DisableKeyword("_EMISSION");
        greenLight.enabled = false;
    }

    protected void SetRed() {
        redMat.EnableKeyword("_EMISSION");
        redLight.enabled = true;

        greenMat.DisableKeyword("_EMISSION");
        greenLight.enabled = false;

        yellowMat.DisableKeyword("_EMISSION");
        yellowLight.enabled = false;
    }

    protected void SetGreen() {
        greenMat.EnableKeyword("_EMISSION");
        greenLight.enabled = true;

        yellowMat.DisableKeyword("_EMISSION");
        yellowLight.enabled = false;

        redMat.DisableKeyword("_EMISSION");
        redLight.enabled = false;
    }
}
