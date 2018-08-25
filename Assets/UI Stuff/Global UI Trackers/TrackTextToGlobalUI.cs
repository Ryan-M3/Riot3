using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackTextToGlobalUI : MonoBehaviour {
    GlobalUIColor globalColor;
    Text txt;
    public float saturation = 1f;

    public void Start() {
        globalColor = FindObjectOfType<GlobalUIColor>();
        txt = GetComponent<Text>();
    }

    public void Update() {
        txt.color = globalColor.GetColor() / saturation;
    }
}
