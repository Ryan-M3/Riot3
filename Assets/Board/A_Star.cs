using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Star {
    Board board;
    //int[,] dists;
    public delegate int HeuristicDist(Point beg, Point end);
    public HeuristicDist Heuristic;
    protected int width;

    public A_Star(Board board, int[,] dists, HeuristicDist heuristic) {
        this.board = board;
        //this.dists = dists;
        this.width = board.width;
        Heuristic  = heuristic;
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

    /// <summary>
    /// A* Search, implemented with reference to wikipedia article's pseudocode.
    /// The heuristic function it uses, however, is a lookup of distances between
    /// points that were earlier calculated based solely on static objects in the
    /// scene. The algorithm used there was the Floyd-Warshall algorithm.
    /// </summary>
    public List<Point> A_StarPath(Point beg, Point end) {
        if (board.IsOccupied(end))
            return new List<Point>();

        if (board.units.Has(end))
            return new List<Point>();

        if (!board.InBounds(beg) || !board.InBounds(end))
            return new List<Point>();

        if ((beg - end).magnitude == 1)
            return new List<Point>() { beg, end };

        // points evaluated
        List<Point> closedSet = new List<Point>();

        // unevaluated points
        List<Point> openSet = new List<Point>();

        // best point to get to a given pointt
        Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();

        // distance from beg to a given point
        Dictionary<Point, float> gScore = new Dictionary<Point, float>();

        // gScore + distance to end; in other words, total distance to the end
        Dictionary<Point, float> fScore = new Dictionary<Point, float>();

        Point cur = beg;
        openSet.Add(beg);
        gScore[beg] = 0f;
        fScore[beg] = Heuristic(beg, NextBest(beg, end));

        for (int i = 0; i < board.width * board.height; i++) {
            // Find the point in openSet with the lowest fScore.
            float lowest_fScore = openSet.Min(pt => fScore.ContainsKey(pt) ? fScore[pt] : Mathf.Infinity);
            cur = openSet.Find(pt => fScore[pt] == lowest_fScore);
            if (cur == end)
                break;

            openSet.Remove(cur);
            closedSet.Add(cur);

            foreach (Point neighbor in board.Neighbors(cur)) {
                if (closedSet.Contains(neighbor))
                    continue;

                if (board.IsOccupied(neighbor))
                    continue;

                if (board.units.Has(neighbor))
                    continue;

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);

                float tentative_gScore = Mathf.Infinity;
                if (gScore.ContainsKey(cur))
                    tentative_gScore = gScore[cur] + 1;

                float neighbor_gScore = Mathf.Infinity;
                if (gScore.ContainsKey(neighbor))
                    neighbor_gScore = gScore[neighbor];

                if (tentative_gScore >= neighbor_gScore)
                    continue;

                // Finally found a good node.
                cameFrom[neighbor] = cur;
                gScore[neighbor]   = tentative_gScore;
                fScore[neighbor]   = gScore[neighbor] + Heuristic(neighbor, end);
            }
            if (openSet.Count == 0)
                break;
        }
        return ReconstructA_StarPath(cameFrom, cur);
    }

    protected static List<Point> ReconstructA_StarPath(Dictionary<Point, Point> cameFrom, Point cur) {
        List<Point> reconstructed = new List<Point>();
        reconstructed.Add(cur);
        while (cameFrom.ContainsKey(cur)) {
            cur = cameFrom[cur];
            reconstructed.Add(cur);
        }
        reconstructed.Reverse();
        return reconstructed;
    }

    public Point NextBest(Point beg, Point end) {
        var adjacent = board.Neighbors(beg).ToList();
        Debug.Assert(adjacent.Count > 0);
        float minDist = Mathf.Infinity;
        minDist = adjacent.Min(p => Heuristic(p, end));
        Point next = adjacent.Find(p => Heuristic(p, end) == minDist);
        return next;
    }
}
