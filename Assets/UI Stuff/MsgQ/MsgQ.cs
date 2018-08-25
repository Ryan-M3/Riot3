using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Message Queue.
/// </summary>
public class MsgQ : MonoBehaviour {
    public float displayTime;
    protected Text displayText;
    protected Queue<string> messages = new Queue<string>();

    public void Awake() {
        displayText = GetComponent<Text>();
    }

    public void Display(string s) {
        messages.Enqueue(s);
        Invoke("Expire", displayTime);
        RefreshMsg();
    }

    protected void RefreshMsg() {
        displayText.text = string.Join("\n", messages.ToArray());
    }

    protected void Expire() {
        messages.Dequeue();
        RefreshMsg();
    }
}
