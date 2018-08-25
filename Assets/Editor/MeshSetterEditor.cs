using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(MeshSetter), true)]
public class MeshSetterEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        MeshSetter actor = target as MeshSetter;

        if (GUILayout.Button("Get Dressed"))
            actor.GetDressed();
    }
}
