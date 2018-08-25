using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackImageToGlobalUI : MonoBehaviour {
    protected GlobalUIColor globalColor;
    protected Image img;
    public float saturation = 1f;

    public void Start() {
        globalColor = FindObjectOfType<GlobalUIColor>();
        img = GetComponent<Image>();
    }

    public void Update() {
        img.color = globalColor.GetColor() / saturation;
    }
}
