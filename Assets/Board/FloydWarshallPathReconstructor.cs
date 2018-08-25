using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloydWarshallPathReconstructor {
    public int[,] dists;
    public int width;

    public FloydWarshallPathReconstructor(int[,] dists) {
        this.dists = dists;
        width = Mathf.RoundToInt(Mathf.Sqrt(dists.GetLength(0)));
    }

    public int GetDistance(Point beg, Point end) {
        try {
            return dists[DistID(beg), DistID(end)];
        } catch {
            Debug.Log(DistID(beg) + " " + DistID(end));
            throw new System.Exception();
        }
    }

    /// <summary>
    /// Maps a 2D point to a 1D point based on the dimensions of dists.
    /// Used to create and adjacency matrix mapping each point to every
    /// other point.
    /// </summary>
    public int DistID(Point c) {
        // Because of zero-indexing v.y * width produces an
        // area one row less than v.y * width. Thus, all we
        // need to do to fill out that last row is add v.x.
        return Mathf.RoundToInt(c.y * width + c.x);
    }

    /// <summary>
    /// Map distance ID value to it's corresponding Coord position on the board.
    /// The inverse function of DistID(Coord).
    /// </summary>
    public Point DistCoord(int idx) {
        if (idx == 0) {
            return new Point(0f, 0f);
        } else {
            return new Point(idx % width, Mathf.Floor(idx / width));
        }
    }
}
