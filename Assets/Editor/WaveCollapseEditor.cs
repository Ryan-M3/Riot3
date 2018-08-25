using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaveCollapse), true)]
public class WaveCollapseEditor : Editor {
#if UNITY_EDITOR
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        WaveCollapse actor = target as WaveCollapse;

        if (GUILayout.Button("Apply"))
            actor.ApplyConditions();

        if (GUILayout.Button("Apply With Random"))
            actor.ApplyWithRandom();
    }
#endif
}
