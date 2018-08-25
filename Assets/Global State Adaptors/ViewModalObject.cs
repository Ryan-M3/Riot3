using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows an object differnt update methods depending on the state/mode of
/// of the View. View has modes like "move mode," and "attack mode." Very
/// similar to the ModalObject class.
/// </summary>
public abstract class ViewModalObject : ModalObject {
    public enum ViewMode {
        move, attack, turn, arrest, heal, off
    }
    protected static ViewMode viewMode = ViewMode.off;

    private void Awake() {
        ModalObject.PlayerTurn += SetMove;
    }

    // These events will be called once as soon as a view mode is set.
    public static event SetFn Move;
    public static event SetFn Attack;
    public static event SetFn Turn;
    public static event SetFn Arrest;
    public static event SetFn Heal;
    public static event SetFn Off;

    // These events will be called every update while in that veiw mode.
    public static event UpdateFn MoveUpdate;
    public static event UpdateFn AttackUpdate;
    public static event UpdateFn TurnUpdate;
    public static event UpdateFn ArrestUpdate;
    public static event UpdateFn HealUpdate;
    public static event UpdateFn OffUpdate;

    protected static void SetMove() {
        SetViewMode(ViewMode.move);
    }

    public static void SetViewMode(ViewMode m) {
        viewMode = m;

        switch (m) {
            case ViewMode.move:
                if (Move != null)
                    Move();
                break;

            case ViewMode.attack:
                if (Attack != null)
                    Attack();
                break;

            case ViewMode.turn:
                if (Turn != null)
                    Turn();
                break;

            case ViewMode.arrest:
                if (Arrest != null)
                    Arrest();
                break;

            case ViewMode.heal:
                if (Heal != null)
                    Heal();
                break;

            case ViewMode.off:
                if (Off != null)
                    Off();
                break;

            default:
                Debug.LogWarning("Unrecognized mode encountered in ViewModalObject.");
                break;
        }
    }

    protected static void CallMoveUpdate() {
        if (MoveUpdate != null)
            MoveUpdate();
    }

    protected static void CallAttackUpdate() {
        if (AttackUpdate != null)
            AttackUpdate();
    }

    protected static void CallTurnUpdate() {
        if (TurnUpdate != null)
            TurnUpdate();
    }

    protected static void CallArrestUpdate() {
        if (ArrestUpdate != null)
            ArrestUpdate();
    }

    protected static void CallHealUpdate() {
        if (HealUpdate != null)
            HealUpdate();
    }

    protected static void CallOffUpdate() {
        if (OffUpdate != null)
            OffUpdate();
    }

    private void OnDestroy() {
        ModalObject.PlayerTurn += SetMove;
    }
}
