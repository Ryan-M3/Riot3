using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds OnEnable and Disable functionality
/// to SaveFile, as well as provides a
/// static reference to it.
/// </summary>
public class SaveWrapper : MonoBehaviour {
    public SaveFile _savef;
    public static SaveFile savef;

    private void Awake() {
        savef = _savef;
        savef.Init();
    }

    private void OnEnable() {
        savef.Init();
    }

    private void OnDisable() {
        savef.CloseDatabase();
    }

    private void OnDestroy() {
        savef.CloseDatabase();
    }
}
