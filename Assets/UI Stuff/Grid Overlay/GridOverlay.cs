using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridOverlay : MonoBehaviour {
    MeshRenderer rend;

    protected void Awake() {
        rend = GetComponent<MeshRenderer>();
    }

    protected void Update() {
        QuantizePosition();
        ScalePlane();
        ScaleTexture();
    }

    protected void ScalePlane() {
        // The magic number 100 was found by trial and error.
        float x = Mathf.Round(Screen.width  / 100f);
        float y = Mathf.Round(Screen.height / 100f);

        // An orthographic size of 5 is our default middle.
        // We then scale it how zoomed in or out we are.
        float ortho = Camera.main.orthographicSize / 5f;
        ortho = ortho > 1 ? Mathf.Round(ortho) : Mathf.Round(ortho * 4f) / 4f;
        x *= ortho;
        y *= ortho;

        transform.localScale = new Vector3(x, 1f, y);
    }

    protected void QuantizePosition() {
        float x = Mathf.Round(Camera.main.transform.position.x);
        float y = transform.position.y;
        float z = Mathf.Round(Camera.main.transform.position.z);
        transform.position = new Vector3(x, y, z);
    }

    protected void ScaleTexture() {
        float scaleX = transform.lossyScale.x * 10f;
        float scaleY = transform.lossyScale.z * 10f;
        rend.sharedMaterial.mainTextureScale = new Vector2(scaleX, scaleY);
    }
}
