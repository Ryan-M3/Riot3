using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusFX_ScriptableObject : ScriptableObject {
    public abstract void Apply(Unit unit);
}
