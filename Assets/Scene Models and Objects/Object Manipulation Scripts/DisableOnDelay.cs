using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnDelay : MonoBehaviour {
    public float delay;

    protected void Start() {
        TurnOn();
    }

    public void TurnOn() {
        Invoke("TurnOff", delay);
    }

    protected void TurnOff() {
        gameObject.SetActive(false);
    }

    protected void OnEnable() {
        TurnOn();
    }
}
