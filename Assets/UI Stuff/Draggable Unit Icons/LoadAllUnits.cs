using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAllUnits : MonoBehaviour {
    public DefaultUnitLib unitLib;
    public Transform left;
    public Transform mid;
    public Transform right;

    protected void Start() {
        MakeAllIcons();
    }

    protected void MakeAllIcons() {
        foreach (CharSheet sheet in SaveWrapper.savef.GetAllUnits()) {
            GameObject chimera = Instantiate(unitLib.Get(sheet.unitType));
            chimera.transform.SetParent(transform);
            chimera.transform.position = Vector3.zero;
            var unitIcon = chimera.GetComponent<HQ_UnitIcon>();
            Debug.Assert(unitIcon != null);
            unitIcon.left  = left;
            unitIcon.mid   = mid;
            unitIcon.right = right;
            unitIcon.Set(sheet);
        }
    }
}
