using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edges;
using System.Linq;


//public class ColonizationAlgo : MonoBehaviour {
//    public Vector3[] verts;
//    public Vector2[] uvs;
//    public int[] tris;
//    public MeshFilter meshFilter;
//    public Gaussian gauss = new Gaussian();
//    public int pullCount     = 16;
//    public int startBranches = 5;
//    public int candidateSize = 5;
//    public float killDist    = 0.1f;

//    protected void Awake() {
//        if (meshFilter == null)
//            meshFilter = GetComponent<MeshFilter>();
//        uvs = GetUVs(verts);
//    }

//    protected void Start() {
//        Mesh mesh = new Mesh();
//        meshFilter.mesh = mesh;
//        mesh.vertices = verts;
//        mesh.uv = uvs;
//        mesh.triangles = tris;
//    }


//    protected List<Vector3> Splatter(int dots) {
//        List<Vector3> splat = new List<Vector3>();
//        for (int i = 0; i < dots; i++) {
//            splat.Add(Random.insideUnitSphere * gauss.Get());
//        }
//        return splat;
//    }

//    protected static KDNode<Point> ToTree(List<Vector3> splatter) {
//        if (splatter.Count < 1) {
//            throw new System.Exception("Splatter can't be empty");
//        }

//        KDNode<Point> tree = new KDNode<Point>();
//        foreach (Point pt in splatter.Select(v => new Point(v))) {
//            tree.Add(pt, null);
//        }

//        return tree;
//    }

//    protected static Point GetNear(Point pt, KDNode<Point> tree) {
//        List<Point> nearBy = new List<Point>();
//        for (float side = 0f; side < 1f; side += 0.01f) {
//            nearBy = tree.Neighborhood(new Rect(pt.x, pt.y, 0.01f, 0.01f))
//                             .Select(node => node.pt)
//                             .ToList();
//            if (nearBy.Count > 0)
//                break;
//        }
//        return nearBy[0];
//    }

//    protected static List<Point> GetManyNear(Point pt, KDNode<Point> tree, int minResults) {
//        List<Point> nearBy = new List<Point>();
//        for (float side = 0f; side < 1f; side += 0.01f) {
//            nearBy = tree.Neighborhood(new Rect(pt.x, pt.y, 0.01f, 0.01f))
//                             .Select(node => node.pt)
//                             .ToList();
//            if (nearBy.Count >= minResults)
//                break;
//        }
//        return nearBy;
//    }

//    protected static Vector2 Pull(Point query, KDNode<Point> tree, int querySize) {
//        List<Point> near = GetManyNear(query, tree, querySize);
//        Vector2 sum = Vector2.zero;
//        foreach (Point pt in near) {
//            Vector2 toPt = pt.ToV2() - query.ToV2();
//            sum += toPt / toPt.sqrMagnitude;
//        }
//        return sum;
//    }

//    protected static List<Edge> StartBranches(Point center, KDNode<Point> tree, int branches) {
//        List<Edge> edges = new List<Edge>();
//        tree.Remove(center);
//        List<Point> nearPts = GetManyNear(center, tree, branches);
//        foreach (Point near in nearPts) {
//            Edge e = new Edge();
//            e.beg = center;
//            e.end = near;
//            edges.Add(e);
//        }
//        return edges;
//    }

//    protected static List<Point> Candidates(int n, List<Edge> edges) {
//        List<Point> candidates = new List<Point>();
//        for (int i = 0; i < n; i++) {
//            int rnd = Random.Range(0, edges.Count - 1);
//            candidates.Add(edges[rnd].end);
//        }
//        return candidates;
//    }

//    //protected List<Edge> Colonize(List<Vector3> splatter) {
//    //    int uncolonized = splatter.Count;
//    //    var tree = ToTree(splatter);
//    //    Point center = GetNear(new Point(0f, 0f), tree);
//    //    tree.Remove(center);

//    //    List<Edge> edges = StartBranches(center, tree, startBranches);
//    //    uncolonized -= edges.Count;
//    //    foreach (Edge e in edges) {
//    //        tree.Remove(e.end);
//    //    }

//    //    while (uncolonized > 0) {
//    //        List<Point> candidates = Candidates(candidateSize, edges);
//    //        candidates.OrderBy(pt => Pull)
//    //    }
//    //}

//    protected static Vector2[] GetUVs(Vector3[] verts) {
//        var uvs = new Vector2[verts.Length];
//        for (int i = 0; i < verts.Length; i++) {
//            uvs[i] = new Vector2(verts[i].x, verts[i].z);
//        }
//        return uvs;
//    }
//}
