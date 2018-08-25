using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ProceduralFire : MonoBehaviour {
    public int ntris;
    public int ntrisRandomness;
    public Vector2 dims;
    public Vector2 randomness;
    public float spacing;
    public float zSpacing;
    public float speed;
    public float timeRandomness = 2f;

    protected List<Vector3> verts;
    protected List<Vector2> uvs;
    protected List<int> tris;
    protected MeshFilter meshFilter;
    protected Mesh mesh;

    protected List<float> ts = new List<float>();

    protected void Awake() {
        meshFilter = GetComponent<MeshFilter>();

        mesh  = new Mesh();
        verts = new List<Vector3>();
        uvs   = new List<Vector2>();
        tris  = new List<int>();

        int rndTris = Random.Range(-ntrisRandomness, ntrisRandomness);
        rndTris = Mathf.Max(1, rndTris);
        for (int i = 0; i < rndTris; i++) {
            float w = dims.x + Random.value * randomness.x;
            float h = dims.y + Random.value * randomness.y;
            AddTriangle(w, h);
            ShiftLastTriangle(dims.x + Random.value * spacing);
        }

        mesh.vertices   = verts.ToArray();
        mesh.uv         = uvs  .ToArray();
        mesh.triangles  = tris .ToArray();
        meshFilter.mesh = mesh;
    }

    protected void AddTriangle(float width, float height) {
        float zshift = tris.Count / 3 * zSpacing;

        Vector3 a = new Vector3(-width,     0f, zshift);
        Vector3 b = new Vector3( width,     0f, zshift);
        Vector3 c = new Vector3(    0f, height, zshift);

        verts.Add(a);
        verts.Add(b);
        verts.Add(c);

        uvs.Add(new Vector2(a.x, a.y));
        uvs.Add(new Vector2(b.x, b.y));
        uvs.Add(new Vector2(c.x, c.y));

        tris.Add(verts.Count - 3);
        tris.Add(verts.Count - 2);
        tris.Add(verts.Count - 1);

        ts.Add(Random.value);
        ts.Add(Random.value);
        ts.Add(Random.value);
    }

    protected void ShiftLastTriangle(float amt) {
        int n = verts.Count;
        for (int i = 0; i < 3; i++) {
            Vector3 v = verts[n - i - 1];
            v.x += amt;
            verts[n - i - 1] = v;
        }
    }

    protected void Update() {
        for (int i = 0; i < ts.Count; i++) {
            ts[i] += Time.deltaTime * Mathf.Max(Random.value * timeRandomness, 1f) * speed;
            if (Random.value <= 0.01f)
                ts[i] += 0.2f;
        }

        List<Vector3> vertsFlickered = new List<Vector3>();
        vertsFlickered.AddRange(verts);

        for (int i = 2; i < verts.Count; i += 3) {
            Vector3 v = vertsFlickered[i];
            v.x *= Mathf.PerlinNoise(ts[i] * randomness.x, i);
            v.y *= Mathf.Cos(ts[i]) + 1f;
            vertsFlickered[i] = (verts[i] + v) / 2f;
        }

        meshFilter.mesh.vertices = vertsFlickered.ToArray();
    }
}
