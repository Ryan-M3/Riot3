using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class UnitItem : ScriptableObject, System.IEquatable<UnitItem> {
    [SerializeField] public int itemID;
    public string itemName;
    public MeshPreset meshPreset;
    public float probabilityToBeEquippedRandomly;
    public int price;

    public bool Equals(UnitItem item) {
        return item.itemID == this.itemID;
    }

#if UNITY_EDITOR
    public List<UnitItemSemanticGroup> semanticGroupCandidates = new List<UnitItemSemanticGroup>();

    public void AddToSemanticGroups() {
        foreach (var group in semanticGroupCandidates) {
            group.items.Add(this);
        }
        semanticGroupCandidates.Clear();
    }

    public void GenerateSemanticGroups() {
        foreach (string gui_id in AssetDatabase.FindAssets("t:UnitItemSemanticGroup")) {
            string path = AssetDatabase.GUIDToAssetPath(gui_id);
            var group = AssetDatabase.LoadAssetAtPath<UnitItemSemanticGroup>(path);
            semanticGroupCandidates.Add(group);
        }
    }
#endif
}
