using System.Collections;using System.Collections.Generic;using UnityEngine;public class Spin : MonoBehaviour {    public Vector3 spin;    void Update () {        Vector3 rot = transform.rotation.eulerAngles;        rot += spin * Time.deltaTime;        transform.rotation = Quaternion.Euler(rot);    }}