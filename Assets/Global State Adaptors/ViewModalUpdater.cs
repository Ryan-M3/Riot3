using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewModalUpdater : ViewModalObject {
    private void Update() {
        switch (viewMode) {
            case ViewMode.move:
                CallMoveUpdate();
                break;

           case ViewMode.attack:
                CallAttackUpdate();
                break;

            case ViewMode.turn:
                CallTurnUpdate();
                break;

            case ViewMode.arrest:
                CallArrestUpdate();
                break;

            case ViewMode.heal:
                CallHealUpdate();
                break;

            case ViewMode.off:
                CallOffUpdate();
                break;
        }
    }
}
