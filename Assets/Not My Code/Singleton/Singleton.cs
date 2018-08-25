using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <attribution> wiki.unity3d.com/index.php/Singleton </attribution>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    public static Singleton<T> instance {
        get; private set;
    }

    protected virtual void Awake() {
        if (instance != null) {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }
}
