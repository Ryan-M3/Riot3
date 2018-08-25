using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows the placing of dancing ants with arbitrary height
/// and width.
/// </summary>
public class RectangularAnts : DancingAnts {
    public int width;
    public int depth;
    protected Point lastPos;

    protected override void Start() {
        base.Start();
    }

    protected void Update() {
        Point thisPos = new Point(transform.position);
        if (lastPos != thisPos)
            Place(thisPos);
    }

    public override Vector3[] GetOutline(Vector3 here) {
        return new Vector3[4] {
            new Vector3(here.x + 0.5f * width, heightOffset, here.z + 0.5f * depth),
            new Vector3(here.x + 0.5f * width, heightOffset, here.z - 0.5f * depth),
            new Vector3(here.x - 0.5f * width, heightOffset, here.z - 0.5f * depth),
            new Vector3(here.x - 0.5f * width, heightOffset, here.z + 0.5f * depth),
        };
    }
}
