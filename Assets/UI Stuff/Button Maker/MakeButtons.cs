using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MakeButtons : MonoBehaviour {
    public GameObject prefab;

    protected List<Button> btns = new List<Button>();

    public void AddButton(UnityAction fn, string txt) {
        btns.Add(Instantiate(prefab).GetComponent<Button>());
        Button btn = btns.Last();
        btn.transform.SetParent(transform);
        btn.onClick.AddListener(fn);

        Transform txt1 = btn.gameObject.transform.GetChild(0);
        Debug.Assert(txt1.name == "btnTxt");
        txt1.gameObject.GetComponent<Text>().text = txt;

        Transform txt2 = btn.gameObject.transform.GetChild(1);
        Debug.Assert(txt2.name == "shortcutTxt");
        txt2.gameObject.GetComponent<Text>().text = "[" + btns.Count + "]";
    }
}
