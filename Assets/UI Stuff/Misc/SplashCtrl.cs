using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashCtrl : MonoBehaviour {
    public MakeButtons mkBtns;

    protected void Awake() {
        mkBtns.AddButton(() => SceneManager.LoadScene(1), "Continue");
        mkBtns.AddButton(() => Debug.Log("error.")      , "Options" );
        mkBtns.AddButton(Application.Quit               , "Exit"    );
    }
}
