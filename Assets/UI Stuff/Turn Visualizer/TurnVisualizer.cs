using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnVisualizer : ViewModalObject {
    public int dir { get; protected set; }
    public float yOffset;
    public float turnSpeed = 100f;
    public SpriteRenderer spriteRend;
    protected Model model;
    protected Ctrl ctrl;
    protected bool unitChanged {
        get {
            if (model.curUnit != lastSeenUnit) {
                lastSeenUnit = model.curUnit;
                return true;
            }
            return false;
        }
    }
    protected Unit lastSeenUnit;
    protected Vector3 unitScreenPt;

    protected void Awake() {
        dir = 1;

        Turn    += TurnOn;
        Move    += TurnOff;
        Attack  += TurnOff;
        Arrest  += TurnOff;
        Heal    += TurnOff;

        TurnUpdate += OnTurnUpdate;

        Deploy     += TurnOff;
        EnemyTurn  += TurnOff;
        Paused     += TurnOff;
        LvlOver    += TurnOff;

        model = FindObjectOfType<Model>();
        ctrl  = FindObjectOfType<Ctrl>();
    }

    protected void Place(Point pt) {
        transform.position = new Vector3(pt.x, yOffset, pt.y);
    }

    protected void TurnOn() {
        spriteRend.enabled = true;
    }

    protected void TurnOff() {
        spriteRend.enabled = false;
    }

    protected void Mirror() {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.x *= -1;
        // We have to add 180 degrees to the y rotation because
        // otherwise the arrow will change which side of the
        // circle it's on, which I found to be very jarring.
        euler.y += 180f;
        transform.rotation = Quaternion.Euler(euler);
    }

    protected void OnTurnUpdate() {
        if (unitChanged) {
            Place(new Point(lastSeenUnit.transform.position));
            unitScreenPt = Camera.main.WorldToScreenPoint(lastSeenUnit.transform.position);
        }

        if (dir != (int)Mathf.Sign(unitScreenPt.x - Input.mousePosition.x)) {
            dir *= -1;
            Mirror();
        }

        Vector3 rot = transform.rotation.eulerAngles;
        rot.y += turnSpeed * Time.deltaTime * dir;
        transform.rotation = Quaternion.Euler(rot);

        if (Input.GetMouseButtonDown(0)) {
            ctrl.EvaluateCmd(new TurnCmd(model.curUnit, dir));
        }
    }

    public void OnDestroy() {
        Turn    -= TurnOn;
        Move    -= TurnOff;
        Attack  -= TurnOff;
        Arrest  -= TurnOff;
        Heal    -= TurnOff;

        TurnUpdate -= OnTurnUpdate;

        Deploy     -= TurnOff;
        EnemyTurn  -= TurnOff;
        Paused     -= TurnOff;
        LvlOver    -= TurnOff;
    }
}
