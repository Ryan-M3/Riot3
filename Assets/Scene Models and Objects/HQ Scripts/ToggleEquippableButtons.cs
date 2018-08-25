using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleEquippableButtons : MonoBehaviour {
    public List<GameObject> btns = new List<GameObject>();
    protected bool isOn = true;

    protected void Awake() {
        TurnOff();
    }

    protected void TurnOn() {
        foreach (GameObject btn in btns) {
            btn.SetActive(true);
        }
        isOn = true;
    }

    protected void TurnOff() {
        foreach (GameObject btn in btns) {
            btn.SetActive(false);
        }
        isOn = false;
    }

    protected bool ShouldBeOn() {
        if (transform.childCount > 0) {
            HQ_UnitIcon icon = transform.GetComponentInChildren<HQ_UnitIcon>();
            return icon != null
                && icon.unit.type != UnitType.dog
                && icon.unit.type != UnitType.van;
        }
        return false;
    }

    protected void Update () {
        if (ShouldBeOn() && !isOn)
            TurnOn();

        else if (!ShouldBeOn() && isOn)
            TurnOff();
    }
}
