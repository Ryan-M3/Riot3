using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EquippableIconLib), true)]
public class EquippableIconLibEditor : Editor {
#if UNITY_EDITOR
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EquippableIconLib actor = target as EquippableIconLib;

        if (GUILayout.Button("Populate"))
            actor.Populate();
    }
#endif
}
