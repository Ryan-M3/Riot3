using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ILib<T> {
    T Get(int itemID);
}
