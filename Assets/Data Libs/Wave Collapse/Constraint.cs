using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

[CreateAssetMenu(menuName ="Custom/Mesh Constraint")]
public class Constraint : ScriptableObject {
    public enum Predicate {
        all, any, none, always
    }

    public enum FilterType {
        canOnlyContain, cantContain
    }

    public List<UnitItem> antecedent = new List<UnitItem>();
    public List<UnitItemSemanticGroup> antecedentGroupItem = new List<UnitItemSemanticGroup>();
    public Predicate antecedentType;

    public List<UnitItem> consequent = new List<UnitItem>();
    public List<UnitItemSemanticGroup> consequentGroupItems = new List<UnitItemSemanticGroup>();
    public FilterType consequentType;

    public void FlattenGroupItems() {
        if (antecedentGroupItem.Count > 0) {
            foreach (var group in antecedentGroupItem) {
                antecedent.AddRange(group.items);
            }
            antecedent.Distinct();
        }

        if (consequentGroupItems.Count > 0) {
            foreach (var group in consequentGroupItems) {
                consequent.AddRange(group.items);
            }
            consequent.Distinct();
        }
    }

    public static bool MatchCondition(List<UnitItem> conditions, List<UnitItem> observations, Predicate predicate) {
        switch (predicate) {
            case Predicate.all:
                return conditions.TrueForAll(p => observations.Contains(p));

            case Predicate.any:
                return observations.Any(p => conditions.Contains(p));

            case Predicate.none:
                return !conditions.Any(p => observations.Contains(p));

            default:
                return true;
        }
    }

    public bool MatchAntecdent(List<UnitItem> observations) {
        return MatchCondition(antecedent, observations, antecedentType);
    }

    public List<UnitItem> Filter(List<UnitItem> superstate) {
        List<UnitItem> filtered = new List<UnitItem>();

        switch (consequentType) {
            case FilterType.canOnlyContain:
                filtered.AddRange(superstate.Where(a => consequent.Contains(a)));
                break;

            case FilterType.cantContain:
                filtered.AddRange(superstate.Where(a => !consequent.Contains(a)));
                break;
        }

        return filtered;
    }
}

public class ConstraintTests {
    public Constraint constraint;
    public List<UnitItem> armors = new List<UnitItem>();

    [SetUp]
    public void SetUp() {
        constraint = ScriptableObject.CreateInstance<Constraint>();

        for (int i = 0; i < 4; i++) {
            armors.Add(ScriptableObject.CreateInstance<Armor>());
            armors[i].itemID = i;
        }

        constraint.antecedent.Add(armors[0]);
        constraint.antecedent.Add(armors[1]);
        constraint.consequent.Add(armors[2]);
    }

    [Test]
    public void TestPriors() {
        List<UnitItem> toFilter = new List<UnitItem>();

        constraint.antecedentType = Constraint.Predicate.all;
        toFilter.Clear();
        toFilter.Add(armors[0]);
        Debug.Assert(constraint.MatchAntecdent(toFilter) == false);
        toFilter.Add(armors[1]);
        Debug.Assert(constraint.MatchAntecdent(toFilter) == true);

        constraint.antecedentType = Constraint.Predicate.any;
        toFilter.Clear();
        toFilter.Add(armors[2]);
        toFilter.Add(armors[3]);
        Debug.Assert(constraint.MatchAntecdent(toFilter) == false);
        toFilter.Add(armors[0]);
        Debug.Assert(constraint.MatchAntecdent(toFilter) == true);

        constraint.antecedentType = Constraint.Predicate.none;
        toFilter.Clear();
        toFilter.Add(armors[2]);
        Debug.Assert(constraint.MatchAntecdent(toFilter) == true);
        toFilter.Add(armors[1]);
        Debug.Assert(constraint.MatchAntecdent(toFilter) == false);

        constraint.antecedentType = Constraint.Predicate.always;
        toFilter.Clear();
        Debug.Assert(constraint.MatchAntecdent(toFilter) == true);
        toFilter.AddRange(armors);
        Debug.Assert(constraint.MatchAntecdent(toFilter) == true);
    }

    [Test]
    public void TestConsequents() {
        List<UnitItem> filtered;

        constraint.consequentType = Constraint.FilterType.canOnlyContain;
        filtered = constraint.Filter(armors);
        Debug.Assert(filtered.Count == 1);
        Debug.Assert(filtered.Contains(armors[2]));

        constraint.consequentType = Constraint.FilterType.cantContain;
        filtered = constraint.Filter(armors);
        Debug.Assert(filtered.Count == 3);
        Debug.Log(filtered.Count);
        Debug.Assert(!filtered.Contains(armors[2]));
    }
}
