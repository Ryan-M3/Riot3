using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shuffle;
using NUnit.Framework;
using UnityEngine.TestTools;

public class WaveCollapse : MonoBehaviour {
    public UnitItemLib itemLib;
    public List<Constraint> constraints = new List<Constraint>();
    public List<UnitItem> observations  = new List<UnitItem>();
    protected List<UnitItem> superstate = new List<UnitItem>();
    protected static List<MeshPreset.Slot> requiredSlots = new List<MeshPreset.Slot>() {
        MeshPreset.Slot.Boots,
        MeshPreset.Slot.Bottom,
        MeshPreset.Slot.Top,
        MeshPreset.Slot.Head
    };
    protected ProbabilityQueue<UnitItem> itemQ = new ProbabilityQueue<UnitItem>();

    protected void Awake() {
        superstate.AddRange(itemLib.items);
    }

    protected void Start() {
    }

    public void ApplyConditions() {
        List<UnitItem> lastSequence = new List<UnitItem>();

        while (!lastSequence.SequenceEqual(superstate)) {
            lastSequence.Clear();
            lastSequence.AddRange(superstate);
            foreach (Constraint c in constraints) {
                if (c.MatchAntecdent(observations)) {
                    superstate = c.Filter(superstate);
                }
            }

            foreach (UnitItem must in MustContain()) {
                observations.Add(must);
                superstate.Remove(must);
            }

            observations = observations.Distinct().ToList();
        } 
    }

    protected UnitItem GetRndFromPossible(MeshPreset.Slot slot) {
        foreach (var possibility in superstate) {
            itemQ.Enqueue(possibility, possibility.probabilityToBeEquippedRandomly);
        }

        while (!itemQ.empty) {
            var pair = itemQ.Dequeue();
            if (pair.item.meshPreset.meshSlot == slot)
                itemQ.Enqueue(pair);
                return pair.item;
        }
        return null;
    }

    public void ApplyWithRandom() {
        for (int i = 0; i < 64; i++) {
            ApplyConditions();
            var observedSlots = observations.Select(c => c.meshPreset.meshSlot);
            foreach (var slot in requiredSlots) {
                if (!observedSlots.Contains(slot)) {
                    var observation = GetRndFromPossible(slot);
                    if (observation != null)
                        observations.Add(observation);
                    break;
                }
            }

            if (superstate.Count <= observations.Count)
                break;
        }
    }

    [Test]
    public void TestApplyWithRandom() {
        GameObject go = new GameObject();
        go.AddComponent<WaveCollapse>();
    }

    /// <summary>
    /// A unit needs to have certain mesh slots filled. If there is
    /// only one left of that slot in the possibility space, we can
    /// conclude that the item must be in our final list of clothes.
    /// </summary>
    public List<UnitItem> MustContain() {
        List<UnitItem> musts = new List<UnitItem>();

        foreach (UnitItem item in superstate) {
            MeshPreset.Slot slot = item.meshPreset.meshSlot;
            if (observations.Count(o => o.meshPreset.meshSlot == slot) == 0)
                continue;

            else if (superstate.Count(p => p.meshPreset.meshSlot == slot) == 1)
                musts.Add(item);
        }
        return musts;
    }
}
