using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveDuringViewmode : ViewModalObject {
    public List<ViewMode> activeModes = new List<ViewMode>();
    public List<GameObject> targets = new List<GameObject>();

    protected void Awake() {
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

    protected void TurnOn() {
        foreach (GameObject tgt in targets) {
            tgt.SetActive(true);
        }
    }

    protected void TurnOff() {
        foreach (GameObject tgt in targets) {
            tgt.SetActive(false);
        }
    }

    protected void OnDestroy() {
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
