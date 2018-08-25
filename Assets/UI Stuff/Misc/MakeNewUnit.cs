using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeNewUnit : MonoBehaviour {
    public DefaultUnitLib unitLib;

    public NameGrabber guyNames;
    public NameGrabber galNames;
    public NameGrabber surnames;

    public Transform left;
    public Transform mid;
    public Transform right;

    public void Make(UnitType type) {
        GameObject uiIcon = Instantiate(unitLib.Get(type));
        Debug.Assert(uiIcon != null);

        HQ_UnitIcon iconCtrl = uiIcon.GetComponent<HQ_UnitIcon>();
        iconCtrl.left  = left;
        iconCtrl.mid   = mid;
        iconCtrl.right = right;
        uiIcon.transform.SetParent(transform);
        uiIcon.transform.position = Vector3.zero;

        foreach (Transform child in uiIcon.transform) {
            Unit unit = child.gameObject.GetComponent<Unit>();
            if (unit != null) {
                unit.name = guyNames.Grab() + " " + surnames.Grab();
                unit.unitName = unit.name;
                unit.unitID = SaveWrapper.savef.GetBiggestUnitID() + 1;
                var sheet = CharSheet.FromUnit(unit);
                CopyPresetIDs(unit, ref sheet);
                SaveWrapper.savef.Add(sheet);
                break;
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
    }

    /// <summary>
    /// Copies the preset IDs found in the Unit's MeshSetter's list
    /// of presets into the approriate CharSheet equipped field.
    /// </summary>
    protected void CopyPresetIDs(Unit unit, ref CharSheet sheet) {
        foreach (UnitItem preset in unit.meshSetter.presets) {
            switch (preset.meshPreset.meshSlot) {
                case MeshPreset.Slot.Boots:
                    sheet.boots = preset.itemID;
                    break;

                case MeshPreset.Slot.Bottom:
                    sheet.btm = preset.itemID;
                    break;

                case MeshPreset.Slot.Top:
                    sheet.top = preset.itemID;
                    break;

                case MeshPreset.Slot.Head:
                    sheet.head = preset.itemID;
                    break;

                case MeshPreset.Slot.LeftHand:
                    sheet.handL = preset.itemID;
                    break;

                case MeshPreset.Slot.RightHand:
                    sheet.handR = preset.itemID;
                    break;
            }
        }
    }


    // The enum version doesn't work
    // with unity buttons.
    public void Make(int unitType) {
        Make((UnitType)unitType);
    }
}
