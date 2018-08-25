using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum UnitType {
    infantry, mounted, bike, dog, van, rioter
}

[System.Serializable]
public class CharSheet {
    public int unitID;
    public string unitName;
    public UnitType unitType;
    public int xp;

    public int boots;
    public int btm;
    public int top;
    public int head;
    public int handL;
    public int handR;

    public CharSheet() { }

    public CharSheet(Unit unit) {
        FromUnit(unit);
    }

    public void FillUnit(ref Unit unit, UnitItemLib itemLib) {
        unit.unitID   = unitID;
        unit.unitName = unitName;
        unit.type     = unitType;
        unit.xp       = xp;

        if (unit.type == UnitType.dog || unit.type == UnitType.van)
            return;

        // This can be null if the Unit isn't actually "Awake" yet.
        if (unit.meshSetter == null) {
            unit.Awake();
        }
        Debug.Assert(unit.meshSetter != null);
        Debug.Assert(itemLib != null);
        UnitItem item;
        item = itemLib.Get(boots);
        if (item == null)
            throw new System.NullReferenceException();
        unit.meshSetter.Equip(itemLib.Get(boots).meshPreset);
        item = itemLib.Get( btm );
        if (item == null)
            throw new System.NullReferenceException();
        unit.meshSetter.Equip(itemLib.Get( btm ).meshPreset);
        item = itemLib.Get( top );
        if (item == null)
            throw new System.NullReferenceException();
        unit.meshSetter.Equip(itemLib.Get( top ).meshPreset);
        item = itemLib.Get(head );
        if (item == null)
            throw new System.NullReferenceException();
        unit.meshSetter.Equip(itemLib.Get(head ).meshPreset);
        item = itemLib.Get(handL);
        if (item == null)
            throw new System.NullReferenceException();
        unit.meshSetter.Equip(itemLib.Get(handL).meshPreset);
        item = itemLib.Get(handR);
        if (item == null)
            throw new System.NullReferenceException();
        unit.meshSetter.Equip(itemLib.Get(handR).meshPreset);
        unit.meshSetter.GetDressed();

        Weapon weapon = itemLib.Get(handR) as Weapon;
        if (weapon != null)
            unit.Equip(weapon);
        else
            Debug.LogError("No usable item found with ID " + handR);
    }

    public static CharSheet FromUnit(Unit unit) {
        CharSheet sheet = new CharSheet();
        sheet.unitID   = unit.unitID;
        sheet.unitName = unit.unitName;
        sheet.unitType = unit.type;
        sheet.xp       = unit.xp;

        if (unit.type == UnitType.dog)
            return sheet;

        foreach (UnitItem item in unit.meshSetter.presets) {
            switch (item.meshPreset.meshSlot) {
                case MeshPreset.Slot.Boots:
                    sheet.boots = item.itemID;
                    break;

                case MeshPreset.Slot.Bottom:
                    sheet.btm = item.itemID;
                    break;

                case MeshPreset.Slot.Top:
                    sheet.top = item.itemID;
                    break;

                case MeshPreset.Slot.Head:
                    sheet.head = item.itemID;
                    break;

                case MeshPreset.Slot.LeftHand:
                    sheet.handL = item.itemID;
                    break;

                case MeshPreset.Slot.RightHand:
                    sheet.handR = item.itemID;
                    break;
            }
        }
        return sheet;
    }

    public void SetFromUnitID(SaveFile savef, int unitID) {
        this.unitID = unitID;
        CharSheet temp = savef.GetCharSheet(unitID);

        unitID   = temp.unitID;
        unitName = temp.unitName;
        unitType = temp.unitType;
        xp       = temp.xp;
        boots    = temp.boots;
        btm      = temp.btm;
        top      = temp.top;
        head     = temp.head;
        handL    = temp.handL;
        handR    = temp.handR;
    }
}
