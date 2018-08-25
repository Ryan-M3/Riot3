using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlComplete : ModalObject {
    public GameObject activate;

    protected void Awake() {
        activate.SetActive(false);
        LvlOver += OnLvlOver;
    }

    protected void OnLvlOver() {
        activate.SetActive(true);
    }

    protected void OnDestroy() {
        LvlOver -= OnLvlOver;
    }
}
