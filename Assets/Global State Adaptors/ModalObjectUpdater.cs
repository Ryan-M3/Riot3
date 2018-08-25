using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalObjectUpdater : ModalObject {
    private void Start() {
        SetMode(Mode.deployment);
    }

    private void Update() {
        switch (mode) {
            case Mode.playerTurn:
                CallPlayerUpdate();
                break;

            case Mode.paused:
                CallPausedUpdate();
                break;

            case Mode.enemyTurn:
                CallEnemyUpdate();
                break;

            case Mode.lvlOver:
                CallLvlOverUpdate();
                break;

            case Mode.deployment:
                CallDeploymentUpdate();
                break;
        }
    }
}
