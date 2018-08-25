using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTracker : TrackToGlobalUI {
    public bool normal, highlighted, pressed, disabled;
    protected Button btn;

    protected void Awake() {
        btn = GetComponent<Button>();
    }

    protected void Update() {
        Color curC = globalColor.GetColor();
        ColorBlock colors = btn.colors;

        if (  normal   ) colors.normalColor      = curC;
        if (highlighted) colors.highlightedColor = curC;
        if (  pressed  ) colors.highlightedColor = curC;
        if (  disabled ) colors.disabledColor    = curC;

        btn.colors = colors;
    }
}
