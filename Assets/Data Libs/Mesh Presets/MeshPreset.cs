using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Mesh Preset")]
[System.Serializable]
public class MeshPreset : ScriptableObject {
    public string presetDescription;
    public enum Slot {
        Head, Top, Bottom, Boots, LeftHand, RightHand
    }
    public Slot meshSlot;
    public Mesh mesh;
    public List<Material> mats = new List<Material>();
    public int skinMatIndex = -1;
}
