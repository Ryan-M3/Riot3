using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStunnable {
    void Stun(int amt);
    void Unstun(int amt);
    Point Pos();
}
