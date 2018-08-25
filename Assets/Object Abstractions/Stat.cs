using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stat<T> : ModalObject {
    private List<T> statHistory = new List<T>(256);

    protected void AddToHistory(T value) {
        if (statHistory.Count == 256)
            statHistory.RemoveAt(0);
        statHistory.Add(value);

    }

    protected void UndoAdd() {
        if (statHistory.Count != 0)
            statHistory.RemoveAt(statHistory.Count - 1);
    }

    public abstract void Reduce(T amt);

    public abstract void UndoChange();
}
