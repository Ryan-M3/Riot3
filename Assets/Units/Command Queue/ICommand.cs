using UnityEngine;

public interface ICommand {
    bool CanDo();
    void Do();
    void Undo();
}
