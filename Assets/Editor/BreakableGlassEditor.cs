using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(BreakableGlass), true)]
public class BreakableGlassEditor : Editor {
#if UNITY_EDITOR
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        BreakableGlass actor = target as BreakableGlass;

        if (GUILayout.Button("Add Physics")) {
            actor.AddPhysics();
        }

        if (GUILayout.Button("Break Glass")) {
            DmgTable dmgTbl = new DmgTable();
            dmgTbl.hp = 1;
            actor.TakeDmg(dmgTbl);
        }

        if (GUILayout.Button("Unbreak Glass")) {
            actor.UndoDmg();
        }
    }
#endif
}
