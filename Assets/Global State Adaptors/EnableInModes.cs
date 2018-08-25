using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableInModes : ModalObject {
    public List<GameObject> targets = new List<GameObject>();
    public List<Mode> modes = new List<Mode>();

    protected void Awake() {
        // There's probably a better way to do this, I just don't know what it is.
        if (modes.Contains(Mode.deployment))
            ModalObject.Deploy += TurnOn;
        else
            ModalObject.Deploy += TurnOn;

        if (modes.Contains(Mode.enemyTurn))
            ModalObject.PlayerTurn += TurnOn;
        else
            ModalObject.PlayerTurn += TurnOff;

        if (modes.Contains(Mode.enemyTurn))
            ModalObject.EnemyTurn += TurnOn;
        else
            ModalObject.EnemyTurn += TurnOff;

        if (modes.Contains(Mode.paused))
            ModalObject.Paused += TurnOn;
        else
            ModalObject.Paused += TurnOff;

        if (modes.Contains(Mode.lvlOver))
            ModalObject.LvlOver += TurnOn;
        else
            ModalObject.LvlOver += TurnOff;
    }

    protected void TurnOn() {
        foreach (GameObject tgt in targets)
            tgt.SetActive(true);
    }

    protected void TurnOff() {
        foreach (GameObject tgt in targets)
            tgt.SetActive(false);
    }

    protected void OnDestroy() {
        if (modes.Contains(Mode.deployment))
            ModalObject.Deploy -= TurnOn;
        else
            ModalObject.Deploy -= TurnOn;

        if (modes.Contains(Mode.enemyTurn))
            ModalObject.PlayerTurn -= TurnOn;
        else
            ModalObject.PlayerTurn -= TurnOff;

        if (modes.Contains(Mode.enemyTurn))
            ModalObject.EnemyTurn -= TurnOn;
        else
            ModalObject.EnemyTurn -= TurnOff;

        if (modes.Contains(Mode.paused))
            ModalObject.Paused -= TurnOn;
        else
            ModalObject.Paused -= TurnOff;

        if (modes.Contains(Mode.lvlOver))
            ModalObject.LvlOver -= TurnOn;
        else
            ModalObject.LvlOver -= TurnOff;
    }
}
