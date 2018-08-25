using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    public int level;
    protected void Awake() {

        int cur = level > 2 ? level : PlayerPrefs.GetInt("level");
        if (cur == 0) {
            PlayerPrefs.SetInt("level", 3);
            cur = 3;
        }
        SceneManager.LoadSceneAsync(cur, LoadSceneMode.Additive);
    }
}
