using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides movement to an object that can be used in
/// an animation that doesn't reset when the animation
/// is finished.
/// </summary>
public class PersistenAnimationMovement : MonoBehaviour {
    public Vector3 velocity;
    public bool isAnimating;

    public void Update() {
        transform.position += velocity * Time.deltaTime;
    }
}
