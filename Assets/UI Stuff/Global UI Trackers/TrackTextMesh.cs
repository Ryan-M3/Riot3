using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackTextMesh : MonoBehaviour {
    protected GlobalUIColor globalColor;
    protected TextMesh txt;

    public void Start() {
        globalColor = FindObjectOfType<GlobalUIColor>();
        txt = GetComponent<TextMesh>();
    }

    public void Update() {
        txt.color = globalColor.GetColor();
    }
}
