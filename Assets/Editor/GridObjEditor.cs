using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridObj), true)]
public class GridObjEditor : Editor {
#if UNITY_EDITOR
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GridObj actor = target as GridObj;

        if (GUILayout.Button("Print Occupied")) {
            foreach (Point pt in actor.GetPtsInGridObj(actor.pos))
                Debug.Log(pt);
        }
    }
#endif
}
