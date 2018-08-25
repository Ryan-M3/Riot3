using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDrawer : ViewModalObject {
    Model model;
    View view;
    public IndicatorPolygon indicatorPolygon;
    public LineRenderer ln;
    public int width;
    public int height;

    protected void Awake() {
        model = FindObjectOfType<Model>();
        view  = FindObjectOfType<View>();

        Attack  += TurnOn;
        Move    += TurnOn;
        Arrest  += TurnOn;
        Turn    += TurnOff;
        LvlOver += TurnOff;
        Heal    += TurnOff;

        AttackUpdate += OnAttackUpdate;
        MoveUpdate   += OnMoveUpdate;
        ArrestUpdate += OnArrestUpdate;

        var board = FindObjectOfType<Board>();
        width  = board.width;
        height = board.height;
    }

    public void FocusOnUnit() {
        ln.enabled = true;
        indicatorPolygon.gameObject.SetActive(true);
    }

    public void TurnOn() {
        ln.enabled = true;
        indicatorPolygon.gameObject.SetActive(true);
    }

    public void TurnOff() {
        ln.enabled = false;
        indicatorPolygon.gameObject.SetActive(false);
    }

    protected void OnMoveUpdate() {
        int mvPts = model.curUnit.AP.ap;
        if (mvPts == 0) {
            TurnOff();
            return;
        }
        Point beg = model.curPos;
        Point end = view.cursorPt;

        if (beg == end) {
            TurnOff();
            return;
        }

        if (Point.ManhattanDistance(beg, end) > mvPts) {
            TurnOff();
            return;
        }

        if (end.x < 0 || end.y < 0) {
            TurnOff();
            return;
        }

        if (end.x > width || end.y > height) {
            TurnOff();
            return;
        }

        List<Point> path = model.board.ShortestPath(beg, end);
        if (path.Count == 0) {
            TurnOff();
            return;
        }
        Debug.Assert(mvPts >= 0);
        path = path.GetRange(0, Mathf.Min(mvPts + 1, path.Count));

        ln.positionCount = path.Count;
        int n = path.Count;

        if (n == 0) {
            TurnOff();
            return;
        }

        if (n > 0) {
            indicatorPolygon.Place(path[n - 1], model.curUnit.MoveCost(path));
        }

        if (n > 1) {
            // Stop halfway at the last point so our path doesn't cross into our circle.
            path[n - 1] = Point.Lerp(path[n - 1], path[n - 2], 0.5f);
            ln.SetPositions(path.Select(pt => pt.ToV3()).ToArray());
        }

        TurnOn();
    }

    protected void OnArrestUpdate() {
        indicatorPolygon.SetNoWhere();
        //int mvPts = 2;
        //Point beg = model.curPos;
        //Point end = view.cursorPt;
        //if (end.x < 0 || end.y < 0) {
        //    indicatorPolygon.SetNoWhere();
        //    return;
        //}
        //List<Point> path = model.StaticPath(beg, end);
        //path = path.GetRange(0, Mathf.Min(mvPts, path.Count));
        //ln.positionCount = path.Count;
        //int n = path.Count;

        //if (n == 0) {
        //    indicatorPolygon.SetNoWhere();
        //}

        //if (n > 0) {
        //    indicatorPolygon.Place(path[n - 1], 1);
        //}

        //if (n > 1) {
        //    // Stop halfway at the last point so our path doesn't cross into our triangle.
        //    path[n - 1] = Point.Lerp(path[n - 1], path[n - 2], 0.5f);
        //    ln.SetPositions(path.Select(pt => pt.ToV3()).ToArray());
        //}
    }

    /// <summary>
    /// If we draw a 1x1 square around the Coord beg, then
    /// draw a line from beg to end. what is the x, y coord-
    /// inate along that line which also intersects with the
    /// square we drew?
    /// </summary>
    public static Point IntersectsGrid(Point beg, Point end) {
        float denom = (beg - end).magnitude;
        if (float.IsNaN(denom) || float.IsInfinity(denom))
            denom = 1f;
        return Point.Lerp(beg, end, 0.5f / denom);
    }

    protected void OnAttackUpdate() {
        indicatorPolygon.Place(view.cursorPt, model.curUnit.equippedWeapon.ap_cost);

        Point beg = model.curPos;
        if (float.IsNaN(beg.x) || float.IsNaN(beg.y))
            return;

        Point end = IntersectsGrid(view.cursorPt, beg);
        if (float.IsNaN(end.x) || float.IsNaN(end.y))
            return;

        Vector3[] pts = new Vector3[2] {
            new Vector3(beg.x, 0.1f, beg.y),
            new Vector3(end.x, 0.1f, end.y)
        };

        if (beg.magnitude == Mathf.Infinity || end.magnitude == Mathf.Infinity) {
            const string msg = "Invalid coordinates in PathDrawer:\nbeg: {0}, end: {1}, ";
            Debug.LogError(string.Format(msg, beg, end));
            return;
        }


        if (ln.positionCount != 2)
            ln.positionCount = 2;
        ln.SetPositions(pts.ToArray());
    }

    public void OnDestroy() {
        Attack  -= TurnOn;
        Move    -= TurnOn;
        Turn    -= TurnOff;
        LvlOver -= TurnOff;
        Arrest  -= TurnOn;
        Heal    -= TurnOff;

        AttackUpdate -= OnAttackUpdate;
        MoveUpdate   -= OnMoveUpdate;
        ArrestUpdate -= OnArrestUpdate;
    }
}
