using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

public class ProjectileArc {
    protected Vector3 begVel;
    protected Vector3 begPos;
    protected const float a = 9.81f;

    public ProjectileArc(Vector3 beg, Vector3 end, float angle) {
        begPos = beg;
        Vector3 toTgt = (end - beg).normalized;
        toTgt.x *= Mathf.Cos(angle);
        toTgt.y  = Mathf.Sin(angle);
        toTgt.z *= Mathf.Cos(angle);
        begVel = toTgt * InitSpeed(beg, end, angle);
    }

    public static float InitSpeed(Vector3 beg, Vector3 end, float angle) {
        float displacement = Vector3.Distance(beg, end);
        float numerator    = 0.5f * a * displacement * displacement;
        float denominator  = displacement * Mathf.Tan(angle) + beg.y;
        float speed = 1f / Mathf.Cos(angle) * Mathf.Sqrt(numerator / denominator);
        return speed;
    }

    public Vector3 Get(float t) {
        Vector3 accel = 0.5f * a * Vector3.down * t * t;
        return begPos + accel + begVel * t;
    }
}

public class TestProjectileArc {
    [Test]
    public void TestArc() {
        var arc = new ProjectileArc(Vector3.one, Vector3.forward * 10f, 35f);
        Debug.Assert(arc.Get(0f).sqrMagnitude == Vector3.one.sqrMagnitude);
        Debug.Assert(arc.Get(0.1f).magnitude > 0f);
    }
}
