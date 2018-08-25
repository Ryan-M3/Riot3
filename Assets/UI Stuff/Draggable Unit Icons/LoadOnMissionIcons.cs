using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadOnMissionIcons : MonoBehaviour {
    public DefaultUnitLib prefabLib;
    public Party playerParty;

    protected void Start() {
        foreach (int unitID in SaveWrapper.savef.GetUnitsOnMission()) {
            MakeIcon(SaveWrapper.savef.GetCharSheet(unitID));
        }
    }

    protected void MakeIcon(CharSheet sheet) {
        GameObject go = Instantiate(prefabLib.Get((int)sheet.unitType));
        go.transform.SetParent(transform);
        var iconCtrl = go.GetComponent<MissionUnitChimera>();
        iconCtrl.playerParty = playerParty;
        iconCtrl.Fill(sheet);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
}
