using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class View : ViewModalObject {
    public Transform _cursor;
    public Point cursorPt {
        get {
            return new Point(_cursor.position);
        }
    }
    public MsgQ msgQ;
    public static MsgQ _msgQ;
    public DancingAnts ants;
    public MakeButtons mkBtn;
    public PlaceableObject dmgVisualizer;
    public Text dmgTxt;

    protected static PlaceableObject _dmgVis;
    protected static Text _dmgTxt;

    protected Ctrl ctrl;
    protected MoveCamera mvCam;
    protected IndicatorPolygon indicator;

    public void Awake() {
        if (_cursor == null)
            _cursor = GameObject.FindWithTag("cursor").transform;
        _msgQ   = msgQ;
        _dmgVis = dmgVisualizer;
        _dmgTxt = dmgTxt;

        ctrl      = FindObjectOfType<Ctrl>();
        mvCam     = FindObjectOfType<MoveCamera>();
        indicator = FindObjectOfType<IndicatorPolygon>();

        PlayerTurn   += OnPlayerTurn;
        Deploy       += OnDeploy;

        PlayerUpdate += OnPlayerUpdate;
        AttackUpdate += OnAttackUpdate;
        MoveUpdate   += OnMoveUpdate;
        ArrestUpdate += OnArrestUpdate;

        mkBtn.AddButton(CallMove  , "Move"  );
        mkBtn.AddButton(CallAtk   , "Attack");
        mkBtn.AddButton(CallTurn  , "Turn"  );
        mkBtn.AddButton(CallArrest, "Arrest");
        mkBtn.AddButton(CallHeal  , "Heal"  );
    }

    public static void ShowMsg(string s) {
        _msgQ.Display(s);
    }

    public static void ShowDmg(Point pt, int hpDmg) {
        _dmgVis.Place(pt);
        _dmgTxt.text = string.Format("-{0} hp", hpDmg);
        _dmgTxt.gameObject.SetActive(true);
    }

    public static void ShowDmgTbl(Point pt, DmgTable dmgTbl) {
        _dmgVis.Place(pt);
        _dmgTxt.text = "";

        if (dmgTbl.hp > 0)
            _dmgTxt.text += "-" + dmgTbl.hp + " hp\n";

        if (dmgTbl.ap > 0)
            _dmgTxt.text += "-" + dmgTbl.ap + " ap\n";

        if (dmgTbl.dp > 0)
            _dmgTxt.text += "-" + dmgTbl.dp + " dp\n";

        if (dmgTbl.fear > 0)
            _dmgTxt.text += "+" + dmgTbl.fear + " fear\n";

        if (dmgTbl.anger > 0)
            _dmgTxt.text += "+" + dmgTbl.anger + " violence\n";

        _dmgTxt.gameObject.SetActive(true);
    }

    public void CallMove() {
        SetViewMode(ViewMode.move);
    }

    public void CallAtk() {
        SetViewMode(ViewMode.attack);
    }

    public void CallTurn() {
        SetViewMode(ViewMode.turn);
    }

    public void CallHeal() {
        SetViewMode(ViewMode.heal);
    }

    public void CallArrest() {
        Debug.Log("calling arrest.");
        SetViewMode(ViewMode.arrest);
    }

    public void FocusCam(Point pt, float zoom=5f) {
        mvCam.StartCoroutine("FocusOn", pt.ToV3());
    }

    public void PlaceAnts(Point pt) {
        ants.Place(cursorPt);
    }

    protected void OnMoveUpdate() {
        if (cursorPt.x < 0 || cursorPt.y < 0)
            return;
        if (Input.GetMouseButtonDown(0)) {
            ctrl.MoveCurUnit(cursorPt);
        }
    }

    protected void OnAttackUpdate() {
        if (Input.GetMouseButtonDown(0))
            ctrl.Attack(cursorPt);
    }

    protected void OnArrestUpdate() {
        if (Input.GetMouseButtonDown(0)) {
            ctrl.Arrest(cursorPt);
        }
    }

    protected void OnPlayerTurn() {
        // Default to move view mode when starting normal mode.
        SetViewMode(ViewMode.move);
    }

    protected void OnDeploy() {
        // Turn off anything that might be on by default
        // when just starting a new level.
        SetViewMode(ViewMode.off);
    }

    protected void OnPlayerUpdate() {
        if (Input.GetButtonDown("EnterMoveMode"))
            SetViewMode(ViewMode.move);
        else if (Input.GetButtonDown("EnterAttackMode"))
            SetViewMode(ViewMode.attack);
        else if (Input.GetButtonDown("EnterTurnMode"))
            SetViewMode(ViewMode.turn);
        else if (Input.GetButtonDown("EnterArrestMode"))
            SetViewMode(ViewMode.arrest);
        else if (Input.GetButtonDown("EnterHealMode"))
            SetViewMode(ViewMode.heal);
    }

    public void OnDestroy() {
        PlayerTurn   -= OnPlayerTurn;
        Deploy       -= OnDeploy;
        PlayerUpdate -= OnPlayerUpdate;
        AttackUpdate -= OnAttackUpdate;
        MoveUpdate   -= OnMoveUpdate;
        ArrestUpdate -= OnArrestUpdate;
    }
}
