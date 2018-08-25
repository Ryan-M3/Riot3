using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class FloydWarshallMono : MonoBehaviour {
    public ArraySaveLoad savef;
    public Board board;
    FloydWarshallBaker flyd;

    public void Calculate() {
        if (flyd == null)
            flyd = new FloydWarshallBaker(board, savef);
        StartCoroutine(flyd.CalculateCoroutine());
    }

    public void CalculateNoCoroutine() {
        if (flyd == null)
            flyd = new FloydWarshallBaker(board, savef);
        flyd.Calculate();
    }

    public void Save() {
        savef.SaveData<int>(flyd.dists, "bakedDists");
    }

    [Test]
    public void TestSave() {
        ArraySaveLoad savef = ScriptableObject.CreateInstance<ArraySaveLoad>();
        int[,] nums = new int[2, 2] { { 1, 2 }, { 3, 4 } };
        savef.SaveData<int>(nums, "testSave");
    }
}
