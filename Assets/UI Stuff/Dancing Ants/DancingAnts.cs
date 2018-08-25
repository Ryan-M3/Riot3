using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

// This isn't actually quite dancing ants, but the same basic idea.
public class DancingAnts : MonoBehaviour {
    public float speed;
    public float length;
    public float heightOffset;
    public List<LineRenderer> lns = new List<LineRenderer>();
    private Vector3[] outline;

    protected virtual void Start() {
        for (int i = 0; i < lns.Count; i++) {
            // The magic number 4 is the number of corners in a square;
            // the algorithm dances from one vertex to the next in one
            // second (modified by the speed you set).
            StartCoroutine("Dance", new object[2] { lns[i], (float)i});
        }
        RenderLines(false);
    }

    public IEnumerator Dance(object[] parameters) {
        LineRenderer ln = (LineRenderer)parameters[0];
        float timeOffset = (float)parameters[1];

        // Wait until outline is set before we do anything.
        while (outline == null)
            yield return null;

        int sides = outline.Length;
        float perimeter = Perimeter();
        timeOffset *= perimeter / lns.Count;

        for (float t = timeOffset; true; t = (t + Time.deltaTime * speed) % perimeter) {
            // Each line consists of a starting point, and endpoint
            // and a middle point so that the line can bend around
            // corners. a, b, and c are indexes that represent which
            // sides of the rectangle the start, middle, and end
            // points should be drawn.
            float begT = t;
            float endT = (t + length) % perimeter;

            int a = Side(begT);
            int c = Side(endT);

            Vector3 beg = GetLerped(t);
            Vector3 mid = a == c ? beg : outline[c];
            Vector3 end = GetLerped(endT);

            ln.SetPositions(new Vector3[3] { beg, mid, end });

            yield return null;
        }
    }

    protected float SideLength(int side) {
        int beg = side;
        int end = (side + 1) % outline.Count();
        return Vector3.Distance(outline[beg], outline[end]);
    }

    [Test]
    public void TestSideLength() {
        outline = new Vector3[4] {
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0f, 1f),
            new Vector3(0f, 0f, 1f)
        };
        Debug.Assert(SideLength(0) == 1f);
        Debug.Assert(SideLength(1) == 1f);
        Debug.Assert(SideLength(2) == 1f);
        Debug.Assert(SideLength(3) == 1f);

        outline = new Vector3[4] {
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0f, 2f),
            new Vector3(0f, 0f, 2f)
        };
        Debug.Assert(SideLength(0) == 1f);
        Debug.Assert(SideLength(1) == 2f);
        Debug.Assert(SideLength(2) == 1f);
        Debug.Assert(SideLength(3) == 2f);
    }

    protected Vector3 GetLerped(float t) {
        int n = outline.Count();
        for (int i = 0; i < n; i++) {
            float sideLen = SideLength(i);
            if (sideLen > t) {
                int nxt = (i + 1) % n;
                return Vector3.Lerp(outline[i], outline[nxt], t / sideLen);
            }

            else {
                t -= sideLen;
            }
        }
        return outline.Last();
    }

    protected float Perimeter() {
        float p = 0f;
        for (int i = 0; i < outline.Length; i++) {
            p += Vector3.Distance(outline[i], outline[(i + 1) % outline.Length]);
        }
        return p;
    }

    [Test]
    public void TestPerimeter() {
        outline = new Vector3[4] {
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0f, 1f),
            new Vector3(0f, 0f, 1f)
        };
        Debug.Assert(Perimeter() == 4f);

        outline = new Vector3[4] {
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0f, 2f),
            new Vector3(0f, 0f, 2f)
        };
        Debug.Assert(Perimeter() == 6f);
    }

    protected int Side(float t) {
        int n = outline.Count();
        for (int i = 0; i < n; i++) {
            float sideLen = SideLength(i);
            if (sideLen > t)
                return i;
            else
                t -= sideLen;
        }
        return n;
    }

    [Test]
    public void TestSide() {
        outline = new Vector3[4] {
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0f, 1f),
            new Vector3(0f, 0f, 1f)
        };
        Debug.Assert(Side(0.3f) == 0);
        Debug.Assert(Side(1.2f) == 1);
        Debug.Assert(Side(2.1f) == 2);
        Debug.Assert(Side(3.0f) == 3);
    }

    public void Place(Point pt) {
        RenderLines(true);
        Vector3 here = transform.position;
        here.x = Mathf.Round(pt.x);
        here.z = Mathf.Round(pt.y);
        outline = GetOutline(here);
    }

    public virtual Vector3[] GetOutline(Vector3 here) {
        return new Vector3[4] {
            new Vector3(here.x + 0.5f, heightOffset, here.z + 0.5f),
            new Vector3(here.x + 0.5f, heightOffset, here.z - 0.5f),
            new Vector3(here.x - 0.5f, heightOffset, here.z - 0.5f),
            new Vector3(here.x - 0.5f, heightOffset, here.z + 0.5f),
        };
    }

    public void RenderLines(bool truth) {
        foreach (LineRenderer lnr in lns)
            lnr.enabled = truth;
    }
}
