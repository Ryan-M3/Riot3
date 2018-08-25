using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// View for use in the home base only.
/// </summary>
public class HQView : ViewModalObject {
    public MsgQ msgQ;
    public static MsgQ _msgQ;

    public void Awake() {
        _msgQ = msgQ;
    }

    public static void ShowMsg(string s) {
        _msgQ.Display(s);
    }
}
