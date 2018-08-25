using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewMode = ViewModalObject.ViewMode;

/// <summary>
/// Displays the amount of action points needed to do an action (moving, attacking)
/// at a specific spot enclosed within a polygon or circle.
/// </summary>
public class IndicatorPolygon : ViewModalObject {
    public TextMesh txt;
    public LineRenderer ln;
    public float yOffset;
    protected int sides;

    protected void Awake() {
        Move   += SetCircle;
        Attack += SetPentagon;
        Arrest += SetSquare;

        Deploy    += SetNoWhere;
        EnemyTurn += SetNoWhere;
        LvlOver   += SetNoWhere;
        Paused    += SetNoWhere;
    }

    protected void SetCircle() {
        sides = 20;
    }

    protected void SetPentagon() {
        sides = 5;
    }

    protected void SetSquare() {
        sides = 4;
    }

    public void Place(Point where, int apCost) {
        transform.position = where.ToV3() + Vector3.up * yOffset;
        txt.text = apCost.ToString();
        DrawPolygon(sides);
    }

    public void SetNoWhere() {
        DrawPolygon(0);
        txt.text = "";
    }

    protected Vector3 PtAt(float t, float r) {
        return new Vector3(Mathf.Cos(t * 2f * Mathf.PI) * r, 0f, Mathf.Sin(t * 2f * Mathf.PI) * r);
    }

    protected void DrawPolygon(int sides) {
        ln.positionCount = sides;
        for (int i = 0; i < sides; i++) {
            Vector3 vertex = PtAt((float)i / (float)sides, 0.5f) + Vector3.up * yOffset;
            ln.SetPosition(i, vertex);
        }
    }

    protected void OnDestroy() {
        Move   -= SetCircle;
        Attack -= SetPentagon;
        Arrest -= SetSquare;

        Deploy    -= SetNoWhere;
        EnemyTurn -= SetNoWhere;
        LvlOver   -= SetNoWhere;
        Paused    -= SetNoWhere;
    }
}
