using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

/// I copy-pasted the C++ code on the wikipedia page for "Marsaglia polar method."
/// I made only minimal alterations and added a unit test that needs to be
/// visually inspected to ensure you get what looks like a bell curve.
public class Gaussian {
    public float mean  = 0.5f;
    public float stdev = 0.1f;
    protected float a;
    protected float b;
    protected float s;
    protected bool hasSpare;
    protected float spare;

    public float Get() {
        if (hasSpare) {
            hasSpare = false;
            return mean + stdev * spare;
        }

        float u, v, s;
        do {
            u = Random.value * 2f - 1f;
            v = Random.value * 2f - 1f;
            s = u * u + v * v;
        } while (s >= 1f || s == 0f);
        s = Mathf.Sqrt(-2f * Mathf.Log(s) / s);

        spare = v * s;
        hasSpare = true;

        return mean + stdev * u * s;
    }

    [Test]
    public void PrintGaussians() {
        Dictionary<int, int> buckets = new Dictionary<int, int>();
        for (int i = 0; i < 1000; i++) {
            float got = Get();
            int bucket = Mathf.RoundToInt(got * 10f);
            if (buckets.ContainsKey(bucket))
                buckets[bucket]++;
            else
                buckets.Add(bucket, 1);
        }

        for (int i = 0; i < 10; i++) {
            char block = '█';
            int n = buckets.ContainsKey(i) ? Mathf.RoundToInt(buckets[i] / 100f) : 0;
            Debug.Log(i + " " + new string(block, n));
        }
    }
}
