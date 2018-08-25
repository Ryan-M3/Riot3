using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ctrl : ModalObject {
    protected View view;
    protected Model model;
    protected CommandQueue cmdQ = new CommandQueue();
    private float inputFreqLimit = 0.5f;
    private bool undoLocked;

    protected void Awake() {
        view  = FindObjectOfType<View>();
        model = FindObjectOfType<Model>();

        DeployUpdate += OnDeployUpdate;
        PlayerUpdate += OnPlayerUpdate;

        ModalObject.Deploy     += OnDeploy;
        ModalObject.PlayerTurn += OnPlayerTurn;
        ModalObject.EnemyTurn  += OnEnemyTurn;
        ModalObject.Paused     += OnPaused;
        ModalObject.LvlOver    += OnLvlOver;
    }

    #region OnModeFunctions
    protected void OnDeploy() {
        View.ShowMsg("Deployment Phase");
    }

    protected void OnPlayerTurn() {
        View.ShowMsg("Player Turn");
    }

    protected void OnEnemyTurn() {
        View.ShowMsg("Enemy Turn");
    }

    protected void OnPaused() {
        View.ShowMsg("Paused");
    }

    protected void OnLvlOver() {
        View.ShowMsg("Level Complete");
    }
    #endregion

    public void MoveCurUnit(Point pt) {
        if (model.board.InBounds(pt)) {
            EvaluateCmd(new MoveCmd(model.curUnit, model, pt));
        }
    }

    public void Attack(Point pt) {
        EvaluateCmd(new AtkCmd(model.curUnit, model, pt));
    }

    public void Arrest(Point pt) {
        if (model.board.InBounds(pt)) {
            EvaluateCmd(new ArrestCmd(model.curUnit, pt, model));
        }
    }

    public void ResetCurUnit() {
        Point focusPt = model.curPos;
        view.FocusCam(focusPt);
    }

    public void EvaluateCmd(ICommand cmd) {
        if (cmd.CanDo()) {
            cmdQ.Add(cmd);
            cmdQ.ExecuteCmds();
        }
    }

    public void EvaluateCmd(AtkCmd cmd) {
        if (cmd.CanDo()) {
            cmdQ.Add(cmd);
            cmdQ.ExecuteCmds();
        }
    }

    public void UndoCmd() {
        cmdQ.Undo();
    }

    #region GetKeyboardInputs
    public void OnDeployUpdate() {
        if (Input.GetButtonDown("Submit")) {
            if (model.curUnit == null)
                View.ShowMsg("Must deploy at least one unit.");
            else
                SetMode(Mode.playerTurn);
        }
    }

    protected bool GetUndoCmd() {
        bool ctrl = Input.GetKeyDown(KeyCode.LeftControl);
        bool z    = Input.GetKeyDown(KeyCode.Z);

        if (undoLocked) {
            return false;
        }

        else if (!ctrl || !z) {
            Debug.Assert(!undoLocked);
            return false;
        }

        else {
            Debug.Assert(ctrl && z && !undoLocked);
            undoLocked = true;
            Invoke("UndoLocked", inputFreqLimit);
            return true;
        }
    }

    private void UnlockUndo() {
        undoLocked = false;
    }

    public void OnPlayerUpdate() {
        if (Input.GetButtonDown("NextUnit")) {
            model.playerParty.CycleNext();
            view.FocusCam(model.curPos);
        }

        else if (Input.GetButtonDown("Submit")) {
            SetMode(Mode.enemyTurn);
        }

        else if (GetUndoCmd()) {
            cmdQ.Undo();
        }
    }
    #endregion

    public void OnDestroy() {
        DeployUpdate += OnDeployUpdate;
    }
}
