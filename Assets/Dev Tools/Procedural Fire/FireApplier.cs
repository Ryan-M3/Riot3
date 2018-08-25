using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates multiple instances of the procedural fire effect and randomly 
/// varies them so that two fires don't look quite identical.
/// </summary>
public class FireApplier : MonoBehaviour {
    public int nfires;
    public float radius;
    public GameObject proceduralFireFX;
    protected List<GameObject> fires = new List<GameObject>();

    protected void Start () {
        for (int i = 0; i < nfires; i++) {
            fires.Add(Instantiate(proceduralFireFX));
            fires[i].transform.SetParent(transform);
            Vector3 pos = transform.position;
            pos.y = transform.position.y;
            pos.x += Random.Range(-radius, +radius);
            pos.z += Random.Range(-radius, +radius);
            fires[i].transform.position = pos;
            Vector3 euler = fires[i].transform.rotation.eulerAngles;
            euler.y += Random.Range(0, 360);
            fires[i].transform.rotation = Quaternion.Euler(euler);
        }
    }
}
