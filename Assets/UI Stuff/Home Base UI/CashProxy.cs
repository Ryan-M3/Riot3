using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides an interface to manipulate cash, should I move
/// it out of PlayerPrefs and alerts certain classes that
/// the amount of cash on hand has changed.
/// </summary>
public static class CashProxy {
    public delegate void CashChangeFn(int newBalance);
    public static event CashChangeFn BalanceChange;

    public static void Withdraw(int amt) {
        PlayerPrefs.SetInt("cash", PlayerPrefs.GetInt("cash") - amt);
        if (BalanceChange != null) {
            BalanceChange(Balance());
        }
    }

    public static int Balance() {
        return PlayerPrefs.GetInt("cash");
    }

    public static void Deposit(int amt) {
        PlayerPrefs.SetInt("cash", PlayerPrefs.GetInt("cash") + amt);
        if (BalanceChange != null) {
            BalanceChange(Balance());
        }
    }
}
