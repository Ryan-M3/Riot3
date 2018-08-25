using System.Collections;using System.Collections.Generic;using UnityEngine;/// <summary>
/// Attach this script to any object that you think is going to need
/// to use a pool. This will tell the water park about how much of an
/// object to start pooling in anticipation of demand.
/// </summary>public class Swimmer : MonoBehaviour {    public Pool poolToUse;    public int estimatedPoolDemand;    public void Start () {        poolToUse.AddSwimmer(this);    }    public void OnEnable() {        poolToUse.AddSwimmer(this);    }    public void OnDestroy() {        poolToUse.RemoveSwimmer(this);    }    public void OnDisable() {        poolToUse.RemoveSwimmer(this);    }}