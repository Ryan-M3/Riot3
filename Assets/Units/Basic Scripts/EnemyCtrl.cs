using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

/// <summary>
/// While each individual unit is controlled independently
/// boids-style, there's still a need to control all units
/// in the same way that the player controls each of his.
/// As an example, no individual rioter can end the enemy
/// turn, only the EnemyCtrl can, but what each enemy does
/// for its turn is controlled by that RioterAI, not the
/// EnemyCtrl. EnemyCtrl is the imagined enemy player in
/// the seat and RioterAI is the unit on the board.
/// </summary>
public class EnemyCtrl : ModalObject {
    public CommandQueue cmdQ = new CommandQueue();
    public Party enemyParty;
    public SimplePriorityQueue<RioterAI> unitQ = new SimplePriorityQueue<RioterAI>();
    public Model model;
    protected const int MAX_TURNS = 32;

    protected void Awake() {
        EnemyTurn += OnEnemyTurn;
        Deploy    += OnDeploy;
    }

    protected void OnEnemyTurn() {
        StartCoroutine(EnemyTurnRoutine());
    }

    protected IEnumerator EnemyTurnRoutine() {
        foreach (Unit unit in enemyParty.members) {
            unitQ.Enqueue(unit.GetComponent<RioterAI>(), -unit.AP.ap);
        }

        for (int i = 0; i < MAX_TURNS; i++) {
            if (unitQ.Count == 0)
                break;
            RioterAI ai = unitQ.Dequeue();
            if (!ai.ownUnit.busy)
                ai.DoTurn();

            if (ai.ownUnit.AP.ap > 1)
                unitQ.Enqueue(ai, -ai.ownUnit.AP.ap);
            yield return new WaitForEndOfFrame();
        }
        SetMode(Mode.playerTurn);
    }

    protected void OnDeploy() {
        foreach (Unit unit in enemyParty.members) {
            model.board.Place(unit, new Point(unit.transform.position));
        }
    }

    protected void OnDestroy() {
        EnemyTurn -= OnEnemyTurn;
        Deploy    -= OnDeploy;
    }
}
