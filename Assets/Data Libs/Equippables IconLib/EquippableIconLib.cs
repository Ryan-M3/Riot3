using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName ="Custom Unique/Equippable Icon Lib")]
public class EquippableIconLib : ScriptableObject, ILib<Sprite> {
    [System.Serializable]
    public struct Icon {
        public string desc;
        public UnitItem item;
        public Sprite img;
    }
    public List<Icon> icons = new List<Icon>();

    public Sprite Get(int itemID) {
        return icons.Find(icon => icon.item.itemID == itemID).img;
    }

    public void Populate() {
        #if UNITY_EDITOR
        foreach (string guid in AssetDatabase.FindAssets("t:UnitItem")) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            UnitItem item = AssetDatabase.LoadAssetAtPath<UnitItem>(path);
            Icon icon = new Icon();
            icon.desc = item.itemName;
            icon.item = item;
            icons.Add(icon);
        }
        #endif
    }
}
