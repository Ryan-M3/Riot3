using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable {
    void TakeDmg(DmgTable dmgTbl);
    void UndoDmg();
    Point Pos();
}
