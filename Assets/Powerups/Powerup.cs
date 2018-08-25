using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour {
    public Weapon weapon;
    public int ammo;

    protected Model model;

    protected void Awake() {
        model = FindObjectOfType<Model>();
        Debug.Assert(weapon != null);
    }

    public Weapon Collect() {
        gameObject.SetActive(false);
        Debug.Log("removing powerup");
        model.board.powerups.Del(new Point(transform.position));
        return weapon;
    }

    protected void Start() {
        model.board.powerups.Add(new Point(transform.position), this);
    }
}
