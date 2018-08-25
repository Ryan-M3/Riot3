using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

[ExecuteInEditMode]
public class GridObj : MonoBehaviour {
    public bool traversible = false;
    public Vector3 dims;
    public Point pos {
        get {
            return new Point(transform.position);
        }
    }

    private void Start() {
        if (!traversible) {
            Board board = FindObjectOfType<Board>();
            foreach (Point pt in GetPtsInGridObj(pos)) {
                if (!board.occupied.Has(pt.Rounded()))
                    board.SetOccupied(pt.Rounded());
            }
        }
    }

    protected void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        bool isRotated = transform.rotation.eulerAngles.y / 90f % 2 != 0;
        float x = isRotated ? dims.z : dims.x;
        float y = dims.y;
        float z = isRotated ? dims.x : dims.z;
        Gizmos.DrawWireCube(transform.position + Vector3.up * y / 2f, new Vector3(x, y, z));
    }

    protected List<Point> RotateAround(List<Point> pts, Point pivot, float angle) {
        return pts.Select(pt => RotateAround(pt, pivot, angle)).ToList();
    }

    protected static Point RotateAround(Point tgt, Point pivot, float angle) {
        Point toTgt = tgt - pivot;
        var v3 = Quaternion.Euler(0f, angle, 0f) * toTgt.ToV3();
        return pivot + new Point(v3);
    }

    public List<Point> GetPtsInGridObj(Point center) {
        // Using an area and checking every valid point in that area
        // is robust enough to function correctly when the dimensions
        // are weird, or the placement is odd.
        Rect area = new Rect(center.x - dims.x, center.y - dims.y, dims.x, dims.y);

        List<Point> occupied = new List<Point>();
        int ybeg = Mathf.FloorToInt(center.y - dims.z/2f);
        int yend = Mathf.CeilToInt (center.y + dims.z/2f);
        int xbeg = Mathf.FloorToInt(center.x - dims.x/2f);
        int xend = Mathf.CeilToInt (center.x + dims.x/2f);
        for (int y = ybeg; y < yend; y++) {
            for (int x = xbeg; x < xend; x++) {
                Point thisPt = new Point(x, y);
                if (area.Contains(thisPt.ToV2()))
                    occupied.Add(thisPt);
            }
        }
        return RotateAround(occupied, pos, transform.rotation.eulerAngles.y);
    }

    [Test]
    public void TestGetOccupiedPts() {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<GridObj>();
        GridObj gridObj = cube.GetComponent<GridObj>();
        gridObj.dims = new Vector3(6f, 1f, 10f);
        cube.transform.position   = new Vector3(3f, 1f,  5f);
        List<Point> occupied = gridObj.GetPtsInGridObj(pos);
        Debug.Assert(occupied.Count == 60);
        Debug.Assert(occupied.Contains(new Point(0f, 0f)));
        Debug.Assert(occupied.Contains(new Point(3f, 7f)));
    }
}
