using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableGlass : MonoBehaviour, IAttackable {
    public GameObject unbroken;
    public GameObject broken;
    public float randomness;
    public Model model;
    public Vector3 vel = Vector3.up * 4f;
    public Dictionary<Transform, Vector3> startPos = new Dictionary<Transform, Vector3>();
    public Dictionary<Transform, Quaternion> startRot = new Dictionary<Transform, Quaternion>();

    protected void Start() {
        if (model == null)
            model = FindObjectOfType<Model>();
        model.board.glass.Add(new Point(transform.position), this);
        foreach (Transform t in broken.transform) {
            startPos.Add(t, t.position);
            startRot.Add(t, t.rotation);
        }
    }

    public void TakeDmg(DmgTable dmgTbl) {
        if (dmgTbl.hp < 1)
            return;
        unbroken.SetActive(false);
        broken.SetActive(true);
        model.board.glass.Del(new Point(transform.position));
        foreach (Transform t in broken.transform) {
            if (t == broken.transform)
                continue;
            Vector3 rnd = Random.onUnitSphere * randomness;
            Rigidbody rb = t.gameObject.GetComponent<Rigidbody>();
            if (rb == null)
                Debug.Log(t.gameObject.name);
            rb.velocity = vel + rnd;
            rb.angularVelocity = Random.insideUnitSphere;
        }
    }

    public IEnumerator ReverseEntropy(Transform glass) {
        Debug.Assert(startPos.ContainsKey(glass));
        Debug.Assert(startRot.ContainsKey(glass));
        Vector3    begPos = glass.transform.position;
        Quaternion begRot = glass.transform.rotation;
        Vector3    endPos = startPos[glass];
        Quaternion endRot = startRot[glass];
        for (float time = 0; time < 1f; time += Time.deltaTime) {
            glass.transform.position = Vector3.Lerp(begPos, endPos, time);
            glass.transform.rotation = Quaternion.Slerp(begRot, endRot, time);
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        broken.SetActive(false);
        unbroken.SetActive(true);
    }

    public void UndoDmg() {
        foreach (Transform t in broken.transform) {
            if (t != broken.transform) {
                StartCoroutine(ReverseEntropy(t));
            }
        }
    }

    public void AddPhysics() {
        foreach (Transform t in broken.transform) {
            t.gameObject.AddComponent<BoxCollider>();
            t.gameObject.AddComponent< Rigidbody >();
        }
    }

    public Point Pos() {
        return new Point(transform.position);
    }
}
