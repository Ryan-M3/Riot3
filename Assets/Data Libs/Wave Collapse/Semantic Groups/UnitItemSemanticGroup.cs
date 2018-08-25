using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Semantic Group")]
public class UnitItemSemanticGroup : ScriptableObject {
    public List<UnitItem> items = new List<UnitItem>();
}
