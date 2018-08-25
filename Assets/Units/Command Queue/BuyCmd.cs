using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyCmd : ICommand {
    MeshSetter meshSetter;
    UnitItem item;
    protected int itemID;
    protected int onHand;
    protected int price;

    public BuyCmd(UnitItem item, int price) {
        this.item = item;
        this.price = price;
    }

    public bool CanDo() {
        if (price > CashProxy.Balance()) {
            View.ShowMsg("Too Poor");
            return false;
        }
        return true;
    }

    public void Do() {
        itemID = item.itemID;
        onHand = SaveWrapper.savef.GetAmtOnHand(itemID);
        onHand++;

        CashProxy.Withdraw(price);
        SaveWrapper.savef.SetAmtOnHand(itemID, onHand);
    }

    public void Undo() {
        onHand--;

        CashProxy.Deposit(price);
        SaveWrapper.savef.SetAmtOnHand(itemID, onHand);
    }
}
