using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCmd : ICommand {
    public Unit unit;
    public List<Point> path;
    public Model model;
    public bool verbose = true;
    protected int apCost;
    protected Point tgt;
    protected bool partial;

    /// <summary>
    /// Move a unit from one position to another.
    /// </summary>
    /// <param name="unit"> The unit being moved. </param>
    /// <param name="model"> Model. </param>
    /// <param name="tgt"> Target position to move unit. </param>
    /// <param name="dontSetOccupied">
    /// By default, this will alert the Board class that a unit
    /// has moved from one position to antoher. Set this to "true"
    /// to override this behavior.
    /// </param>
    /// <param name="partial"> If set to true, unit will move along the path
    /// until it's out of AP, rather than refuse to move there on the basis
    /// that it doesn't have enough AP to complete the journey.
    /// </param>
    public MoveCmd(Unit unit, Model model, Point tgt, bool partial=false) {
        this.unit   = unit;
        this.model  = model;
        this.tgt    = tgt;
        Point beg   = new Point(unit.transform.position);
        Point end   = tgt;
        path = model.board.ShortestPath(beg, end);
        // This is -1 because the path includes
        // the spot we're already on.
        apCost = unit.MoveCost(path);
        this.partial = partial;
    }

    public bool CanDo() {
        bool hasAP = unit.AP.ap >= apCost;
        if (!hasAP && !partial) {
            if (verbose)
                View.ShowMsg("Not enough AP.");
            return false;
        }

        // A path with count 1 has a start, but no destination.
        // A valid path has to have at least two points in it.
        bool noPath = path.Count < 2;
        if (noPath) {
            if (verbose)
                View.ShowMsg("Invalid path.");
            return false;
        }

        bool isMoving = unit.busy;
        if (isMoving) {
            if (verbose)
                View.ShowMsg("Can't move a unit while unit is moving.");
            return false;
        }
        return true;
    }

    public void Do() {
        Point tgtPt = partial ? path[unit.AP.ap - 1] : path.Last();
        model.board.Move(unit.Pos(), tgtPt);
        unit.Move(path, apCost);
    }

    public void Undo() {
        model.board.Move(path.Last(), unit.Pos());
        path.Reverse();
        unit.UndoMove(path, apCost);
    }
}
