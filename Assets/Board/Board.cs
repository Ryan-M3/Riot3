using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

/// <summary>
/// A facade for a variety of different data structures that store different
/// kinds of information about the board without the user having to think
/// about what those data structures are.
/// </summary>
public class Board : ModalObject {
    public int width;
    public int height;
    public VectorField flowField;
    public ArraySaveLoad distSavef;

    public PtTree<Unit>           units    = new PtTree<Unit>();
    public PtTree<bool>           occupied = new PtTree<bool>();
    public PtTree<Powerup>        powerups = new PtTree<Powerup>();
    public PtTree<BreakableGlass> glass    = new PtTree<BreakableGlass>();
    public PtTree<FlippableCar>   cars     = new PtTree<FlippableCar>();

    protected A_Star astar;
    protected FloydWarshallPathReconstructor floyd;
    protected Dictionary<Edges.Edge, List<Point>> pathCache = new Dictionary<Edges.Edge, List<Point>>();

    protected void Awake() {
        if (flowField == null) {
            flowField = new VectorField(width, height);
            flowField.Calculate(this);
        }
        int[,] dists = distSavef.LoadMatrix<int>("bakedDists");
        floyd = new FloydWarshallPathReconstructor(dists);
        astar = new A_Star(this, dists, floyd.GetDistance);
        ModalObject.PlayerTurn += ResetPathCache;
    }

    /// <summary>
    /// Clear all memos for shortest paths. This has to be
    /// called once the state of the board has changed, and
    /// thus the shortest paths might have changed.
    /// </summary>
    public void ResetPathCache() {
        pathCache.Clear();
    }

    /// <summary> Find the shortest path from point beg to point end. </summary>
    public List<Point> ShortestPath(Point beg, Point end) {
        Edges.Edge e = new Edges.Edge();
        e.beg = beg;
        e.end = end;
        if (!pathCache.ContainsKey(e))
            pathCache[e] = astar.A_StarPath(beg, end);
        return pathCache[e];
    }

    /// <summary> Move a piece on the board. </summary>
    public void Move(Point from, Point to) {
        if (!units.Has(from)) {
            Debug.LogError("Unit not found to move from.");
            return;
        }
        Unit unit = units.Get(from);
        Debug.Assert(unit != null);
        units.Del(from);
        units.Add(to, unit);
    }

    /// <summary>  Place a unit on the board. </summary>
    public void Place(Unit unit, Point at) {
        Vector3 pos = at.ToV3();
        unit.transform.position = pos;
        units.Add(at, unit);
    }

    public List<IAttackable> GetAttackable(Point pt) {
        List<IAttackable> atked = new List<IAttackable>();

        if (units.Has(pt))
            atked.Add(units.Get(pt));

        if (glass.Has(pt))
            atked.Add(glass.Get(pt));

        return atked;
    }

    public List<IAttackable> GetAttackablesInRange(Point center, float dist) {
        List<IAttackable> inRange = new List<IAttackable>();

        foreach (Unit unit in units.Find(center, dist)) {
            inRange.Add(unit);
        }

        foreach (var glass in glass.Find(center, dist)) {
            inRange.Add(glass);
        }

        return inRange;
    }

    /// <summary> Set the specified tile as occupied. </summary>
    public void SetOccupied(Point pt) {
        if (occupied.Has(pt)) {
            Debug.LogError("Point already added: " + pt);
            return;
        }
        occupied.Add(pt, true);
    }

    /// <summary> Check if the specified tile is occupied. </summary>
    public bool IsOccupied(Point pt) {
        return occupied.Has(pt) || units.Has(pt) || cars.Has(pt);
    }

    /// Set all spaces that are occupied to occupied.
    public void SetAllOccupied() {
        foreach (GridObj grob in FindObjectsOfType<GridObj>()) {
            if (!grob.traversible)
                continue;

            foreach (Point pt in grob.GetPtsInGridObj(grob.pos))
                occupied.Add(pt, true);
        }
    }

    /// <summary> Clear a tile of anything which might be there. </summary>
    public void ClearTile(Point pt) {
        if (occupied.Has(pt))
            occupied.Del(pt);

        if (units.Has(pt))
            units.Del(pt);

        if (cars.Has(pt))
            cars.Del(pt);
    }

    /// <summary> Determine if a given point is in bounds. </summary>
    public bool InBounds(Point pt) {
        return pt.x >= 0 && pt.x < width && pt.y >= 0 && pt.y < height;
    }

    public IEnumerable<Point> Neighbors(Point pt) {
        if (InBounds(pt.North))
            yield return pt.North;

        if (InBounds(pt.East))
            yield return pt.East;

        if (InBounds(pt.South))
            yield return pt.South;

        if (InBounds(pt.West))
            yield return pt.West;
    }

    public void CalculateFlowField() {
        flowField.Calculate(this);
    }

    public Vector3 GetFlow(Point pt) {
        return flowField[pt];
    }

    public Vector3 GetFlow(int i, int j) {
        return flowField[i, j];
    }

    public void AddToField(Field<Vector3> addend) {
        if (flowField == null) {
            flowField = new VectorField(width, height);
            flowField.Calculate(this);
        }
        flowField.AddToField(addend);
    }

    public void SmoothFlow(float amt) {
        flowField.SmoothFlow(amt);
    }

    public void OnDestroy() {
        ModalObject.PlayerTurn -= ResetPathCache;
    }
}

public class BoardTests {
    [Test]
    public void TestUnitStuff() {
        GameObject go = new GameObject();
        go.AddComponent<Unit>();
        Unit unit = go.GetComponent<Unit>();
        go.AddComponent<Board>();
        Board board = go.GetComponent<Board>();

        Point zero = new Point(Vector3.zero);
        Point ones = new Point(Vector3.one);

        board.Place(unit, zero);
        Debug.Assert(!board.units.Has(ones));
        Debug.Assert(board.units.Get(zero)== unit);

        board.Move(zero, ones);
        Debug.Assert(board.units.Get(ones) == unit);
        Debug.Assert(!board.units.Has(zero));

        board.units.Del(ones);
        Debug.Assert(!board.units.Has(zero));
        Debug.Assert(!board.units.Has(ones));
    }

    [Test]
    public void TestOccupiedStuff() {
        GameObject go = new GameObject();
        go.AddComponent<Board>();
        Board board = go.GetComponent<Board>();

        Point zero = new Point(Vector3.zero);
        Point ones = new Point(Vector3.one);

        board.SetOccupied(zero);
        Debug.Assert( board.IsOccupied(zero));
        Debug.Assert(!board.IsOccupied(ones));

        board.SetOccupied(ones);
        Debug.Assert( board.IsOccupied(zero));
        Debug.Assert( board.IsOccupied(ones));

        board.occupied.Del(zero);
        Debug.Assert( board.IsOccupied(ones));
        Debug.Assert(!board.IsOccupied(zero));

        board.occupied.Del(ones);
        Debug.Assert(!board.IsOccupied(ones));
        Debug.Assert(!board.IsOccupied(zero));
    }

    [Test]
    public void TestInBounds() {
        GameObject go = new GameObject();
        go.AddComponent<Board>();
        Board board = go.GetComponent<Board>();
        board.width  = 10;
        board.height =  5;

        Debug.Assert( board.InBounds(new Point( 0, 0)));
        Debug.Assert(!board.InBounds(new Point(-1, 0)));
        Debug.Assert( board.InBounds(new Point( 9, 4)));
        Debug.Assert(!board.InBounds(new Point(10, 5)));

        Debug.Assert(board.Neighbors(new Point(0, 0)).Count() == 2);
        Debug.Assert(board.Neighbors(new Point(3, 3)).Count() == 4);
        Debug.Assert(board.Neighbors(new Point(9, 4)).Count() == 2);
    }
}
