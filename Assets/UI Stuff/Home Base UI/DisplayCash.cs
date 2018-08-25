using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DisplayCash : MonoBehaviour {
    protected Text txt; 

    protected void Awake() {
        txt = GetComponent<Text>();
        CashProxy.BalanceChange += OnCashChange;
        txt.text = "Cash: " + CashProxy.Balance();
    }

    protected void OnCashChange(int newBalance) {
        txt.text = "Cash: " + newBalance;
    }
}
