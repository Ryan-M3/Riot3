using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLayers : MonoBehaviour {
    public LayerMask grid;
    protected bool _gridOn;

    public void Start() {
        _gridOn = PlayerPrefs.GetInt("GridOn") != 1;
        ToggleGrid();
    }

    public void TurnOnGrid() {
        Camera.main.cullingMask |= grid;
        _gridOn = true;
    }

    public void TurnOffGrid() {
        Camera.main.cullingMask &= ~grid;
        _gridOn = false;
    }

    public void ToggleGrid() {
        if (_gridOn)
            TurnOffGrid();
        else
            TurnOnGrid();
    }

    public void Update() {
        if (Input.GetButtonDown("ToggleGrid"))
            ToggleGrid();
    }

    public void OnDestroy() {
        PlayerPrefs.SetInt("GridOn", _gridOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
