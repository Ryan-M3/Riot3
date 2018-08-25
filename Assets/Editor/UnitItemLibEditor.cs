using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitItemLib), true)]
public class UnitItemLibEditor : Editor {
#if UNITY_EDITOR
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        UnitItemLib actor = target as UnitItemLib;

        if (GUILayout.Button("Assign IDs")) {
            actor.AssignIDs();
        }

        if (GUILayout.Button("Sort")) {
            actor.Sort();
        }

        if (GUILayout.Button("Repopulate")) {
            actor.items.Clear();
            foreach (string gui_id in AssetDatabase.FindAssets("t:UnitItem")) {
                string assetPath = AssetDatabase.GUIDToAssetPath(gui_id);
                UnitItem item = AssetDatabase.LoadAssetAtPath<UnitItem>(assetPath);
                if (item != null)
                    actor.items.Add(item);
                else
                    Debug.LogError("Failure on asset path: " + assetPath);
            }
        }
    }
#endif
}
