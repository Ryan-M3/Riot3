using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides distinct update functions for each game mode. For
/// instance, paused, turn ended, moving, and attacking. Will
/// also call events that occur when switching to that mode.
/// </summary>
public class ModalObject : MonoBehaviour {
    // Set up each type of mode the game can be in.
    public enum Mode {
        deployment, playerTurn, enemyTurn, paused, lvlOver
    }
    protected static Mode mode = Mode.deployment;

    // These events will be called once as soon as a mode is set.
    public delegate void SetFn();
    public static event SetFn Deploy;
    public static event SetFn PlayerTurn;
    public static event SetFn EnemyTurn;
    public static event SetFn Paused;
    public static event SetFn LvlOver;

    // These events will be called every update while in that mode.
    public delegate void UpdateFn();
    public static event UpdateFn DeployUpdate;
    public static event UpdateFn PlayerUpdate;
    public static event UpdateFn EnemyUpdate;
    public static event UpdateFn PausedUpdate;
    public static event UpdateFn LvlOverUpdate;

    public static void SetMode(Mode m) {
        mode = m;

        switch (m) {
            case Mode.deployment:
                if (Deploy != null)
                    Deploy();
                break;

            case Mode.playerTurn:
                if (PlayerTurn != null)
                    PlayerTurn();
                break;

            case Mode.paused:
                if (Paused != null)
                    Paused();
                break;

            case Mode.lvlOver:
                if (LvlOver != null)
                    LvlOver();
                break;

            case Mode.enemyTurn:
                if (EnemyTurn != null)
                    EnemyTurn();
                break;
        }
    }

    protected void CallPlayerUpdate() {
        if (PlayerUpdate != null)
            PlayerUpdate();
    }

    protected void CallPausedUpdate() {
        if (PausedUpdate != null)
            PausedUpdate();
    }

    protected void CallEnemyUpdate() {
        if (EnemyUpdate != null)
            EnemyUpdate();
    }

    protected void CallLvlOverUpdate() {
        if (LvlOverUpdate != null)
            LvlOverUpdate();
    }

    protected void CallDeploymentUpdate() {
        if (DeployUpdate != null)
            DeployUpdate();
    }
}
