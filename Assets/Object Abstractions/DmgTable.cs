using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows multiple classes of damage. The unit taking the damge
/// should decide which fields are relevant and which are not.
/// </summary>
[System.Serializable]
public struct DmgTable {
    public int hp;
    public int ap;
    public int dp;
    public float fear;
    public float anger;
}
