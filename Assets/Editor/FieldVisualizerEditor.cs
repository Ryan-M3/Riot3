using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldVisualizer), true)]
public class FieldVisualizerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        FieldVisualizer actor = target as FieldVisualizer;

        if (GUILayout.Button("Refresh")) {
            actor.board.CalculateFlowField();
        }

        if (GUILayout.Button("Smooth Flow")) {
            actor.Smooth();
        }
    }
}
