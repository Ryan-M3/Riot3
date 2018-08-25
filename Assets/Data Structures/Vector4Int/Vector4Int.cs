using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Seems kinda weird Unity has a float and integer version,
/// of every Vector with the sole exception of the Vector4
/// which only comes in float form.
/// </summary>
public struct Vector4Int {
    public int x, y, z, w;

    public Vector4Int(int x, int y, int z, int w) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
}

