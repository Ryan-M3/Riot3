using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusFX {
    void OnApply(Unit unit);
    void Update();
    void OnRemove();
    void UndoUpdate();
}
