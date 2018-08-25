using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RepulsiveObject), true)]
public class RepulsiveObjectEditor : Editor {
#if UNITY_EDITOR
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        RepulsiveObject actor = target as RepulsiveObject;

        if (GUILayout.Button("Add")) {
            actor.AddToBoard();
        }

        if (GUILayout.Button("Remove")) {
            actor.RemoveFromBoard();
        }
    }
#endif
}
