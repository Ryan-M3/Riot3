using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

/// <summary>
/// Implementation of a Roulette Algorithm, but in queue form.
/// </summary>
public class ProbabilityQueue<T> where T: System.IEquatable<T> {
    public struct ItemProb {
        public T item;
        public float weight;
    };
    public float totalWeights = 0f;
    public float maxWeight;
    public bool empty {
        get {
            return items.Count == 0;
        }
    }
    public int Count {
        get {
            return items.Count;
        }
    }
    protected List<ItemProb> items = new List<ItemProb>();

    public void Enqueue(T item, float weight) {
        items.Add(new ItemProb { item=item, weight=weight });
        totalWeights += weight;
        CalculateMaxWeight();
    }

    public void Enqueue(ItemProb itemProb) {
        Enqueue(itemProb.item, itemProb.weight);
    }

    protected void CalculateMaxWeight() {
        if (items.Count == 0)
            maxWeight = 0f;
        else
            maxWeight = items.Max(itm => itm.weight);
    }

    /// <summary>
    /// The finite geometric series of x^0 + x^1 + x^2 + x^3...
    /// </summary>
    protected static float GeometricSeries(float q, float sequenceLength) {
        float numer = 1f - Mathf.Pow(q, sequenceLength + 1);
        float denom = 1f - q;
        return numer / denom;
    }

    public void Remove(T value) {
        for (int i = 0; i < items.Count; i++) {
            if (items[i].item.Equals(value)) {
                maxWeight -= items[i].weight;
                items.RemoveAt(i);
                CalculateMaxWeight();
                return;
            }
        }
        throw new KeyNotFoundException("Key not found in Queue.");
    }

    public bool Has(T value) {
        foreach (var itemProb in items) {
            if (itemProb.item.Equals(value)) {
                return true;
            }
        }
        return false;
    }

    public void ChangeWeight(T value, float weight) {
        for (int i = 0; i < items.Count; i++) {
            var item    = items[i];
            item.weight = weight;
            items[i]    = item;
        }
    }

    /// <summary>
    /// Algorithm for Roulette Wheel problem described here: https://arxiv.org/pdf/1109.3627.pdf.
    /// </summary>
    /// <returns></returns>
    public ItemProb Dequeue() {
        if (items.Count == 0)
            throw new System.IndexOutOfRangeException();
            
        while (true) {
            ItemProb rnd;
            if (items.Count == 1)
                rnd = items[0];
            else
                rnd = items[Random.Range(0, items.Count - 1)];
            float q = (items.Count - items.Select(itm => itm.weight).Sum()) / items.Count;
            float p = rnd.weight / items.Count / maxWeight * GeometricSeries(q, 1000f);
            if (Random.value <= p) {
                Remove(rnd.item);
                return rnd;
            }
        }
    }
}

public class ProbabilityQueueTests {
    [Test]
    public void AssertProbabilities() {
        var q = new ProbabilityQueue<int>();
        q.Enqueue(0, 0.5f);
        q.Enqueue(1, 0.3f);
        q.Enqueue(2, 0.2f);
        float a = 0;
        float b = 0;
        float c = 0;
        float iters = Mathf.Pow(10, 6);
        for (int i = 0; i < iters; i++) {
            var dqed = q.Dequeue();
            switch (dqed.item) {
                case 0:
                    a++;
                    break;

                case 1:
                    b++;
                    break;

                case 2:
                    c++;
                    break;
            }
            q.Enqueue(dqed);
        }
        float total = a + b + c;
        Debug.Log(string.Format("50%: {0} 30%: {1} 20%: {2}", a / total, b / total, c / total));
        Debug.Assert(0.5f - a / total <= 0.1f);
        Debug.Assert(0.3f - b / total <= 0.1f);
        Debug.Assert(0.2f - c / total <= 0.1f);
    }
}
