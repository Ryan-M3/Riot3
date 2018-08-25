using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Edges;


//[ExecuteInEditMode]
//public class Distances : MonoBehaviour {
//    public int maxDist;
//    public int[,] dists;
//    public ArraySaveLoad arraySaver;
//    public Board board;

//    public void Awake() {
//        dists = arraySaver.LoadMatrix<int>("bakedDists");
//        board = FindObjectOfType<Board>();
//    }

//    /// <summary>
//    /// Calculate and then save to file every distance from
//    /// every point on the board to every other point on the
//    /// board.
//    /// </summary>
//    public void Bake() {
//        if (board == null)
//            board = FindObjectOfType<Board>();

//        StartCoroutine(CalcDists());
//    }

//    /// <summary>
//    /// Precomputed based on static objects in the scene.
//    /// </summary>
//    public int HeuristicDist(Point beg, Point end) {
//        return dists[DistID(beg), DistID(end)];
//    }

//    /// <summary>
//    /// Maps a 2D point to a 1D point based on the dimensions of dists.
//    /// Used to create and adjacency matrix mapping each point to every
//    /// other point.
//    /// </summary>
//    public int DistID(Point c) {
//        // Because of zero-indexing v.y * width produces an
//        // area one row less than v.y * width. Thus, all we
//        // need to do to fill out that last row is add v.x.
//        return Mathf.RoundToInt(c.y * board.width + c.x);
//    }

//    /// <summary>
//    /// Map distance ID value to it's corresponding Coord position on the board.
//    /// The inverse function of DistID(Coord).
//    /// </summary>
//    public Point DistCoord(int idx) {
//        if (idx == 0)
//            return new Point(0f, 0f);
//        else
//            return new Point(idx % board.width, Mathf.Floor(idx / board.width));
//    }

//    /// <summary>
//    /// A* Search, implemented with reference to wikipedia article's pseudocode.
//    /// The heuristic function it uses, however, is a lookup of distances between
//    /// points that were earlier calculated based solely on static objects in the
//    /// scene. The algorithm used there was the Floyd-Warshall algorithm.
//    /// </summary>
//    public List<Point> NonStaticPath(Point beg, Point end) {
//        if (board.IsOccupied(end))
//            return new List<Point>();

//        if (board.GetUnit(end) != null)
//            return new List<Point>();

//        if (!board.InBounds(beg) || !board.InBounds(end))
//            return new List<Point>();

//        if ((beg - end).magnitude == 1)
//            return new List<Point>() { beg, end };

//        // points evaluated
//        List<Point> closedSet = new List<Point>();

//        // unevaluated points
//        List<Point> openSet = new List<Point>();

//        // best point to get to a given pointt
//        Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();

//        // distance from beg to a given point
//        Dictionary<Point, float> gScore = new Dictionary<Point, float>();

//        // gScore + distance to end; in other words, total distance to the end
//        Dictionary<Point, float> fScore = new Dictionary<Point, float>();

//        Point cur = beg;
//        openSet.Add(beg);
//        gScore[beg] = 0f;
//        fScore[beg] = HeuristicDist(beg, NextBest(beg, end));

//        for (int i = 0; i < board.width * board.height; i++) {
//            // Find the point in openSet with the lowest fScore.
//            float lowest_fScore = openSet.Min(pt => fScore.ContainsKey(pt) ? fScore[pt] : Mathf.Infinity);
//            cur = openSet.Find(pt => fScore[pt] == lowest_fScore);
//            if (cur == end)
//                break;

//            openSet.Remove(cur);
//            closedSet.Add(cur);

//            foreach (Point neighbor in board.Neighbors(cur)) {
//                if (closedSet.Contains(neighbor))
//                    continue;

//                if (board.IsOccupied(neighbor))
//                    continue;

//                if (board.GetUnit(neighbor) != null)
//                    continue;

//                if (!openSet.Contains(neighbor))
//                    openSet.Add(neighbor);

//                float tentative_gScore = Mathf.Infinity;
//                if (gScore.ContainsKey(cur))
//                    tentative_gScore = gScore[cur] + 1;

//                float neighbor_gScore = Mathf.Infinity;
//                if (gScore.ContainsKey(neighbor))
//                    neighbor_gScore = gScore[neighbor];

//                if (tentative_gScore >= neighbor_gScore)
//                    continue;

//                // Finally found a good node.
//                cameFrom[neighbor] = cur;
//                gScore[neighbor]   = tentative_gScore;
//                fScore[neighbor]   = gScore[neighbor] + HeuristicDist(neighbor, end);
//            }
//            if (openSet.Count == 0)
//                break;
//        }
//        return ReconstructA_StarPath(cameFrom, cur);
//    }

//    protected static List<Point> ReconstructA_StarPath(Dictionary<Point, Point> cameFrom, Point cur) {
//        List<Point> reconstructed = new List<Point>();
//        reconstructed.Add(cur);
//        while (cameFrom.ContainsKey(cur)) {
//            cur = cameFrom[cur];
//            reconstructed.Add(cur);
//        }
//        reconstructed.Reverse();
//        return reconstructed;
//    }

//    protected Point NextBest(Point beg, Point end) {
//        var adjacent = board.Neighbors(beg).ToList();
//        Debug.Assert(adjacent.Count > 0);
//        float minDist = Mathf.Infinity;
//        minDist = adjacent.Min (p => dists[DistID(p), DistID(end)]);
//        Point next  = adjacent.Find(p => dists[DistID(p), DistID(end)] == minDist);
//        return next;
//    }

//    /// <summary>
//    /// Find the shortest path from point A to point B assuming only static objects in the scene.
//    /// </summary>
//    public List<Point> StaticPath(Point beg, Point end) {
//        if (dists == null)
//            dists = arraySaver.LoadMatrix<int>("bakedDists");

//        if (!board.InBounds(beg) || !board.InBounds(end) || board.GetUnit(end) != null) {
//            return new List<Point>();
//        }

//        else if (dists[DistID(beg), DistID(end)] > 999) {
//            return new List<Point>() { };
//        }

//        else if (beg == end) {
//            return new List<Point>(){ beg };
//        }

//        else if (dists[DistID(beg), DistID(end)] == 1) {
//            return new List<Point>() { beg, end };
//        }

//        else {
//            List<Point> path = new List<Point>();
//            path.Add(beg);
//            path.AddRange(StaticPath(NextBest(beg, end), end));
//            return path;
//        }
//    }

//    /// <summary>
//    /// Floyd-Warshall Algorithm for calculating the distance from each point
//    /// on the board to every other point on the board.
//    /// </summary>
//    public int[,] CalcDists(int width, int height) {
//        int area = width * height;
//        dists = new int[area, area];

//        // Set each distance to an arbitrarily big number (the algorithm
//        // replaces this if a smaller distance is found).
//        int bigNum = 256;
//        for (int i = 0; i < area; i++) {
//            for (int j = 0; j < area; j++) {
//                // Set the distance from each point to itself as zero.
//                if (i == j)
//                    dists[i, j] = 0;
//                else
//                    dists[i, j] = bigNum;
//            }
//        }

//        // We'll need to make sure occupied is up-to-date.
//        // Additionally, Bake has to be called to ensure
//        // that everything is loaded and assigned.
//        board.SetAllOccupied();

//        // Add a distance of 1 to all immediately adjacent tiles.
//        for (int y = 0; y < height; y++) {
//            for (int x = 0; x < width; x++) {
//                var cntr  = new Point(x    , y    );
//                var north = new Point(x    , y + 1);
//                var east  = new Point(x + 1, y    );
//                var south = new Point(x    , y - 1);
//                var west  = new Point(x - 1, y    );

//                if (board.IsOccupied(cntr))
//                    continue;

//                if (board.InBounds(north) && !board.IsOccupied(north))
//                    dists[DistID(cntr), DistID(north)] = 1;

//                if (board.InBounds(east ) && !board.IsOccupied(east))
//                    dists[DistID(cntr), DistID(east )] = 1;

//                if (board.InBounds(south) && !board.IsOccupied(south))
//                    dists[DistID(cntr), DistID(south)] = 1;

//                if (board.InBounds(west ) && !board.IsOccupied(west ))
//                    dists[DistID(cntr), DistID(west )] = 1;
//            }
//        }

//        // Now the meat of the algorithm.
//        // k == number of intermediate tiles one must pass through to get to your destination
//        //int maxDist = Mathf.Min(area, 16);
//        for (int k = 1; k < maxDist; k++) {
//            for (int i = 0; i < area; i++) {
//                for (int j = 0; j < area; j++) {
//                    if (!board.IsOccupied(DistCoord(i)) && !board.IsOccupied(DistCoord(j))) {
//                        int sum = dists[i, k] + dists[k, j];
//                        if (dists[i, j] > sum) {
//                            dists[i, j] = sum;
//                        }
//                    }
//                }
//            }
//        }
//        return dists;
//    }

//    public IEnumerator CalcDists() {
//        int width  = board.width;
//        int height = board.height;
//        int area   = width * height;
//        dists = new int[area, area];

//        // Set each distance to an arbitrarily big number (the algorithm
//        // replaces this if a smaller distance is found).
//        int bigNum = 256;
//        for (int i = 0; i < area; i++) {
//            for (int j = 0; j < area; j++) {
//                // Set the distance from each point to itself as zero.
//                if (i == j)
//                    dists[i, j] = 0;
//                else
//                    dists[i, j] = bigNum;
//            }
//        }

//        // We'll need to make sure occupied is up-to-date.
//        // Additionally, Bake has to be called to ensure
//        // that everything is loaded and assigned.
//        board.SetAllOccupied();

//        // Add a distance of 1 to all immediately adjacent tiles.
//        for (int y = 0; y < height; y++) {
//            for (int x = 0; x < width; x++) {
//                var cntr  = new Point(x    , y    );
//                var north = new Point(x    , y + 1);
//                var east  = new Point(x + 1, y    );
//                var south = new Point(x    , y - 1);
//                var west  = new Point(x - 1, y    );

//                if (board.IsOccupied(cntr))
//                    continue;

//                if (board.InBounds(north) && !board.IsOccupied(north))
//                    dists[DistID(cntr), DistID(north)] = 1;

//                if (board.InBounds(east ) && !board.IsOccupied(east))
//                    dists[DistID(cntr), DistID(east )] = 1;

//                if (board.InBounds(south) && !board.IsOccupied(south))
//                    dists[DistID(cntr), DistID(south)] = 1;

//                if (board.InBounds(west ) && !board.IsOccupied(west ))
//                    dists[DistID(cntr), DistID(west )] = 1;
//            }
//        }

//        // Now the meat of the algorithm.
//        // k == number of intermediate tiles one must pass through to get to your destination
//        //int maxDist = Mathf.Min(area, 16);
//        for (int k = 1; k < maxDist; k++) {
//            for (int i = 0; i < area; i++) {
//                Debug.Log(string.Format("k={0}, i={1}", k, i));
//                yield return null;
//                Debug.Log(".");
//                yield return null;
//                for (int j = 0; j < area; j++) {
//                    if (!board.IsOccupied(DistCoord(i)) && !board.IsOccupied(DistCoord(j))) {
//                        int sum = dists[i, k] + dists[k, j];
//                        if (dists[i, j] > sum) {
//                            dists[i, j] = sum;
//                        }
//                    }
//                }
//            }
//        }
//        Debug.Log("Calculation complete. Saving...");
//        yield return null;
//        arraySaver.SaveData(dists, "bakedDists");
//        Debug.Log("Saved.");
//        yield return null;
//    }

//    /// <summary>
//    /// Check if the point is in the grid, if it is occupied by an obstacle,
//    /// or if it has a unit there.
//    /// </summary>
//    protected bool CanPassThrough(Point pt) {
//        bool onGrid = pt.x >= 0 && pt.x < board.width && pt.y >= 0 && pt.y < board.height;
//        bool noObst = !board.IsOccupied(pt);
//        return onGrid && noObst;
//    }


//    /// <summary>
//    /// Get every point within a certain Manhattan distance.
//    /// </summary>
//    public Point[] GetAllInDistance(Point fromPt, int dist) {
//        int origin = DistID(fromPt);
//        List<Point> closePts = new List<Point>();
//        for (int travelTo = 0; travelTo < board.width * board.height; travelTo++) {
//            if (dists[origin, travelTo] <= dist)
//                closePts.Add(DistCoord(travelTo));
//        }
//        return closePts.ToArray();
//    }

//    /// <summary>
//    /// Get every point at an exact Manhattan distance. Use the
//    /// GetAllInDistance function if you want to also return any
//    /// points which are less than the distance parameter.
//    /// </summary>
//    public List<Point> GetAtExactDistance(Point fromPt, int dist) {
//        int origin = DistID(fromPt);
//        List<Point> closePts = new List<Point>();
//        for (int i = 0; i < board.width * board.height; i++) {
//            if (dists[origin, i] == dist)
//                closePts.Add(DistCoord(i));
//        }
//        return closePts.ToList();
//    }

//    /// <summary>
//    /// Find a point which is guaranteed to be on the convex hull.
//    /// </summary>
//    protected static Point PointOnHull(List<Point> pts) {
//        return (from pt in pts where pt.y == pts.Min(p => p.y) select pt)
//               .OrderBy(p => p.x)
//               .First();
//    }

//    /// <summary>
//    /// "Gift-wrapping algorithm" - implemented by referencing wikipedia.
//    /// Finds the convex hull of a set of points.
//    /// </summary>
//    protected List<Point> GiftWrap(List<Point> pts) {
//        List<Point> hull = new List<Point>();
//        Point ptOnHull = PointOnHull(pts);
//        Point endPt = pts[0];
//        int i = 0;
//        do {
//            hull.Add(ptOnHull);
//            endPt = pts[0];
//            for (int j = 1; j < pts.Count; j++) {
//                if (endPt == ptOnHull)
//                    endPt = pts[j];
//                if (LeftOf(hull[i], endPt, pts[j]))
//                    endPt = pts[j];
//            }
//            i++;
//            ptOnHull = endPt;
//            if (i > pts.Count)
//                break;
//        } while (ptOnHull != hull[0]);
//        return hull;
//    }

//    /// <summary>
//    /// If we a draw an infinitely long line which passes through points
//    /// beg and end, determine if the query point lies on the left of that
//    /// line. Returns false if the query point is colinear to beg and end.
//    /// </summary>
//    protected static bool LeftOf(Point query, Point beg, Point end) {
//        return (end.x - beg.x) * (query.y - beg.y) - (end.y - beg.y) * (query.x - beg.x) < 0;
//    }
//}
