using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Catenary))]
public class CatenayEditor : Editor {
#if UNITY_EDITOR
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUI.changed) {
            Catenary catenary = (Catenary)target;
            catenary.Regenerate();
        }
    }
#endif
}
