using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FloydWarshallMono), true)]
public class FloydWarshallEditor : Editor {
#if UNITY_EDITOR
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        FloydWarshallMono actor = target as FloydWarshallMono;

        if (GUILayout.Button("Caluclate Coroutine"))
            actor.Calculate();

        if (GUILayout.Button("Calculate No Coroutine"))
            actor.CalculateNoCoroutine();

        if (GUILayout.Button("Save"))
            actor.Save();
    }
#endif
}
