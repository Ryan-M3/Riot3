using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoardDataStruct<T> {
    void Add(Point pt, T item);
    void Del(Point pt);
    bool Has(Point pt);
    T    Get(Point pt);

    List<T> Find(Point center, float radius);
}
