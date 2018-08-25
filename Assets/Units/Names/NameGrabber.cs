using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

[CreateAssetMenu(menuName ="Custom/Name Grabber")]
public class NameGrabber : ScriptableObject {
    public TextAsset nameList;

    public string Grab() {
        return _Grab(nameList.text);
    }

    // We need to separate this out from Grab since Unity
    // doesn't let you assign TextAsset's text attribute.
    public string _Grab(string s) {
        string[] lines = Regex.Split(s, "\n");
        int rnd = Random.Range(0, lines.Length);
        return lines[rnd];
    }
}

public class TestNameGrabber {
    [Test]
    public void TestGrab() {
        NameGrabber grabber = ScriptableObject.CreateInstance<NameGrabber>();
        string s = "Adam\nBob\nCharlie\nDan\nErasmus\nFrank\nGerald";
        List<string> names = new List<string>();
        for (int i = 0; i < 10; i++) {
            string name = grabber._Grab(s);
            Debug.Assert(name.Length > 1);
            names.Add(name);
        }
    }
}
