using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ImgMapping {
    public Sprite img;
    public UnitType unitType;
    public int classID {
        get {
            return (int)unitType;
        }
    }
}

[CreateAssetMenu(menuName = "Custom Unique/Class Icon Lib")]
public class ClassIcons : ScriptableObject, ILib<Sprite> {
    public List<ImgMapping> data = new List<ImgMapping>();

    public Sprite Get(int classID) {
        return data.Find(imgMap => imgMap.classID == classID).img;
    }
}
