using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Constraint), true)]
public class ConstraintEditor : Editor {
#if UNITY_EDITOR
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        Constraint actor = target as Constraint;

        if (GUILayout.Button("Flatten Group Items"))
            actor.FlattenGroupItems();
    }
#endif
}
