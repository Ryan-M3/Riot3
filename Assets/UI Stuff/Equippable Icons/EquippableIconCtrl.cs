using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquippableIconCtrl : MonoBehaviour {
    public UnitItem setOnStart;
    public Image lockImg;
    public Image iconImg;
    public Text descTxt;
    public EquippableIconLib eqpIconLib;
    public UnitItemLib itemLib;
    public CurrentlySelected cur;
    protected UnitItem item;

    protected void Start() {
        if (setOnStart != null)
            Set(setOnStart.itemID);
        if (cur == null)
            cur = FindObjectOfType<CurrentlySelected>();
    }

    public void Lock() {
        lockImg.enabled = true;
    }

    public void Unlock() {
        lockImg.enabled = false;
    }

    public void SetText(string itemName, int amtOnHand, int price) {
        descTxt.text  = itemName + "\n";
        descTxt.text += amtOnHand > 0 ? "Click to Equip" : ("Price: " + price);
    }

    public void SetImg(int itemID) {
        iconImg.sprite = eqpIconLib.Get(itemID);
    }

    public void Set(int itemID) {
        item = itemLib.Get(itemID);
        if (item == null)
            throw new KeyNotFoundException();
        int amtOnHand = SaveWrapper.savef.GetAmtOnHand(item.itemID);
        SetText(item.itemName, amtOnHand, item.price);
        if (amtOnHand > 0)
            Unlock();
        else
            Lock();
        SetImg(itemID);
    }

    public void Buy() {
        int cash = CashProxy.Balance();
        if (cash < item.price) {
            HQView.ShowMsg("Insufficient funds.");
        } else {
            CashProxy.Withdraw(item.price);
            int amtOnHand = SaveWrapper.savef.GetAmtOnHand(item.itemID);
            amtOnHand++;
            SaveWrapper.savef.SetAmtOnHand(item.itemID, amtOnHand);
            Unlock();
        }
    }

    public void Equip() {
        MeshSetter mset = cur.unit.GetComponent<MeshSetter>();
        if (mset == null)
            Debug.LogError("error in equip");
        else
            mset.Equip(item.meshPreset);
        Unit unit = cur.unit.GetComponent<Unit>();
        if (unit == null)
            throw new System.Exception("Current unit is null and cannot equip this item.");
        SaveWrapper.savef.Equip(unit.unitID, item.meshPreset.meshSlot, item.itemID);
    }

    public void OnClick() {
        int amtOnHand = SaveWrapper.savef.GetAmtOnHand(item.itemID);
        if (lockImg.enabled && amtOnHand == 0)
            Buy();

        if (SaveWrapper.savef.GetAmtOnHand(item.itemID) > 0)
            Equip();
        else
            Debug.Log(SaveWrapper.savef.GetAmtOnHand(item.itemID));
    }
}
