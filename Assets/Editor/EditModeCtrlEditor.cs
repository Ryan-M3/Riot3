using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EditModeCtrl), true)]
public class EditModeCtrlEditor : Editor {
#if UNITY_EDITOR
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        //EditModeCtrl actor = target as EditModeCtrl;

        if (GUILayout.Button("Free Money")) {
            CashProxy.Deposit(1);
        }
    }
#endif
}
