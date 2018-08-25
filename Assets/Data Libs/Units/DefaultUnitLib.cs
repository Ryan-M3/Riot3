using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PrefabMap {
    public GameObject prefab;
    public UnitType unitType;
}

[CreateAssetMenu(menuName = "Custom Unique/Default Unit Lib")]
public class DefaultUnitLib : ScriptableObject, ILib<GameObject> {
    public List<PrefabMap> defaults = new List<PrefabMap>();

    public GameObject Get(UnitType unitType) {
        return defaults.Find(map => map.unitType == unitType).prefab;
    }

    public GameObject Get(int unitType) {
        return Get((UnitType)unitType);
    }
}
