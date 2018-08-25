using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TxtTrackerDisablable : ViewModalObject {
    public float saturation = 1f;
    public Color offColor;
    protected GlobalUIColor globalColor;
    protected Text txt;
    protected bool on = true;
    public List<ViewMode> activeModes = new List<ViewMode>();

    protected void Awake() {
        globalColor = FindObjectOfType<GlobalUIColor>();
        txt = GetComponent<Text>();
        TurnOff();

        // There's probably a better way of doing this, but
        // I can't think of one right now.
        if (activeModes.Contains(ViewMode.move))
            Move += TurnOn;
        else
            Move += TurnOff;

        if (activeModes.Contains(ViewMode.attack))
            Attack += TurnOn;
        else
            Attack += TurnOff;

        if (activeModes.Contains(ViewMode.turn))
            Turn += TurnOn;
        else
            Turn += TurnOff;

        if (activeModes.Contains(ViewMode.arrest))
            Arrest += TurnOn;
        else
            Arrest += TurnOff;

        if (activeModes.Contains(ViewMode.heal))
            Heal += TurnOn;
        else
            Heal += TurnOff;

        if (activeModes.Contains(ViewMode.off))
            Off += TurnOn;
        else
            Off += TurnOff;
    }

    public void TurnOff() {
        on = false;
        txt.color = offColor;
    }

    public void TurnOn() {
        on = true;
    }

    protected void Update() {
        if (on) txt.color = globalColor.GetColor() / saturation;
    }

    protected void OnDisable() {
        if (activeModes.Contains(ViewMode.move))
            Move -= TurnOn;
        else
            Move -= TurnOff;

        if (activeModes.Contains(ViewMode.attack))
            Attack -= TurnOn;
        else
            Attack -= TurnOff;

        if (activeModes.Contains(ViewMode.turn))
            Turn -= TurnOn;
        else
            Turn -= TurnOff;

        if (activeModes.Contains(ViewMode.arrest))
            Arrest -= TurnOn;
        else
            Arrest -= TurnOff;

        if (activeModes.Contains(ViewMode.heal))
            Heal -= TurnOn;
        else
            Heal -= TurnOff;

        if (activeModes.Contains(ViewMode.off))
            Off -= TurnOn;
        else
            Off -= TurnOff;
    }
}
