using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : ModalObject {
    public float camTurnSpeed = 75;
    public float camMoveSpeed = 5f;
    public float zoomSpeed    = 200f;
    public float zoomMax      = 20f;
    public float zoomMin      = 0.75f;
    public float defaultZoom  = 5f;
    public float lerpSpeed    = 25f;
    // Angling the camera makes focusing on an
    // object difficult, so we need an offset.
    public float lerpToOffset = -3f;
    public DancingAnts ants;

    protected void Awake() {
        PlayerUpdate += MoveOnInput;
        EnemyUpdate  += MoveOnInput;
        DeployUpdate += MoveOnInput;
    }

    protected void Rotate(float dir) {
        Vector3 euler = gameObject.transform.rotation.eulerAngles;
        // 360 / camTurnSpeed makes it so that the cam
        // turn speed is the time in seconds.
        euler.y += dir * 360 / camTurnSpeed * Time.deltaTime;
        gameObject.transform.rotation = Quaternion.Euler(euler);
    }

    protected void Move(float h, float v) {
        float y = transform.position.y;
        Vector3 hVec = h * transform.right;
        Vector3 vVec = v * transform.up;
        Vector3 p = transform.position + (hVec + vVec) * Time.deltaTime * camMoveSpeed;
        p.y = y;
        transform.position = p;

    }

    protected void Zoom(float zoom) {
        float ortho = Camera.main.orthographicSize;
        float zoomDelta =  zoom * -zoomSpeed;
        ortho += zoomDelta * Time.deltaTime;
        Camera.main.orthographicSize = Mathf.Clamp(ortho, zoomMin, zoomMax);
    }

    public IEnumerator FocusOn(Vector3 target) {
        CancelInvoke("FocusOn");
        if (ants != null)
            ants.Place(new Point(target));
        target  += transform.up * lerpToOffset;
        target.y = transform.position.y;
        Vector3 original = transform.position;
        float lerpTime = Vector3.Distance(original, target) / lerpSpeed;
        for (float t = 0f; t < lerpTime; t += Time.deltaTime) {
            transform.position = Vector3.Lerp(original, target, t / lerpTime);
            yield return null;
        }
        yield return null;
    }

    protected void MoveOnInput() {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw( "Vertical" );
        float r = Input.GetAxisRaw( "Rotation" );
        float z = Input.GetAxisRaw("Mouse ScrollWheel");
        Move(h, v);
        Zoom(z);
        Rotate(r);
    }

    protected void OnDestroy() {
        PlayerUpdate -= MoveOnInput;
        EnemyUpdate  -= MoveOnInput;
        DeployUpdate -= MoveOnInput;
    }
}
