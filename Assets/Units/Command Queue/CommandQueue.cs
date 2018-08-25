using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows the player to undo actions by using the command
/// pattern to encapsulate each action into an ICommand
/// object. Every such object should contain all of the
/// information necessary to check whether a command is
/// possible, to perform that command, and then undo that
/// command if necessary.
/// </summary>
public sealed class CommandQueue {
    private List<ICommand> cmdQ = new List<ICommand>();
    private int curCmd;

    public void Add(ICommand cmd) {
        cmdQ.Add(cmd);
    }

    public void ExecuteCmds() {
        if (curCmd > cmdQ.Count) {
            Debug.LogWarning("CommandQueue does not have any commands queued up to execute.");
            return;
        }

        for (; curCmd < cmdQ.Count; curCmd++) {
            cmdQ[curCmd].Do();
        }
    }

    public void Undo() {
        if (curCmd == 0) {
            Debug.LogWarning("No last move to undo.");
            return;
        }
        curCmd--;
        cmdQ[curCmd].Undo();
    }

    public void Redo() {
        if (curCmd > cmdQ.Count - 1) {
            Debug.LogWarning("No last command to redo.");
            return;
        }
        cmdQ[curCmd].Do();
        curCmd++;
    }
}
