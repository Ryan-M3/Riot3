using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitItem), true)]
public class UnitItemEditor : Editor {
#if UNITY_EDITOR
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        UnitItem actor = target as UnitItem ;

        if (GUILayout.Button("Assign IDs")) {
            actor.itemID = actor.GetHashCode();
            EditorUtility.SetDirty(actor);
        }

        if (GUILayout.Button("Generate Semantic Groups")) {
            actor.GenerateSemanticGroups();
        }

        if (GUILayout.Button("AddToSemanticGroups")) {
            actor.AddToSemanticGroups();
        }


    }
#endif
}
