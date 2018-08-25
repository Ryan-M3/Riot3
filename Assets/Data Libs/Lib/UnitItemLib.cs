using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom Unique/UnitItemLib")]
[System.Serializable]
public class UnitItemLib : ScriptableObject, ILib<UnitItem> {
    public List<UnitItem> items = new List<UnitItem>();

    public UnitItem Get(int itemID) {
        var found = items.Find(item => item.itemID == itemID);
        if (found == null)
            Debug.LogError(itemID + " not found in ItemLib");
        return found;
    }

    public void Sort() {
        items = items.OrderBy(item => (int)item.meshPreset.meshSlot).ToList();
    }

    public void AssignIDs() {
        foreach (UnitItem item in items) {
            item.itemID = item.GetHashCode();
        }
    }
}
