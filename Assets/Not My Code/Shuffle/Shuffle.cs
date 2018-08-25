using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace Shuffle {
    /// <attribution>
    /// https://stackoverflow.com/questions/273313/randomize-a-listt
    /// </attribution>
    public static class RandomExtensions {
        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    public class ShuffleTests {
        [Test]
        public void TestShuffle() {
            List<int> ints = new List<int>();
            for (int i = 0; i < 99; i++) {
                ints.Add(i);
            }

            List<int> oldInts = new List<int>();

            for (int i = 0; i < 100; i++) {
                ints.Shuffle();
                Debug.Assert(!ints.SequenceEqual(oldInts));
                oldInts.Clear();
                oldInts.AddRange(ints);
            }

            for (int i = 0; i < 99; i++) {
                Debug.Assert(ints.Contains(i));
            }
        }
    }
}
