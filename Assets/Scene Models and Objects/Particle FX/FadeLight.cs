using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FadeLight : MonoBehaviour {
    public float rate = 0.5f;
    public Light light;
    public float initIntensity;
    protected float t;

    public void Awake() {
        if (light != null)
            light = GetComponent<Light>();
    }

    public void Update() {
        if (light.intensity > 0.0f) {
            t += Time.deltaTime;
            light.intensity -= rate;
        }
    }

    public void OnEnable() {
        t = 0f;
    }
}
