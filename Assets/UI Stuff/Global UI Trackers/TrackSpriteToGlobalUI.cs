using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackSpriteToGlobalUI : TrackToGlobalUI {
    public SpriteRenderer spr;

    public void Update() {
        spr.color = globalColor.GetColor();
    }
}
