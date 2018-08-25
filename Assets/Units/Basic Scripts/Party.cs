using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class Party : MonoBehaviour {
    public List<Unit> members  = new List<Unit>();
    public List<Unit> inactive = new List<Unit>();
    public Unit cur {
        get {
            return members.Count == 0 ? null : members[unitIdx];
        }
    }
    protected int unitsLeft;
    protected int unitIdx;

    /// <summary>
    /// Used for dead, stunned, or arrested units,
    /// or units without action points left.
    /// </summary>
    public void SetInactive(Unit unit) {
        inactive.Add(unit);
        Remove(unit);
    }

    public void SetActive(Unit unit) {
        members.Add(unit);
        inactive.Remove(unit);
    }

    public void CycleNext() {
        unitIdx++;
        unitIdx %= members.Count;
    }

    public void CyclePrev() {
        unitIdx--;
        unitIdx += members.Count;
        unitIdx %= members.Count;
    }

    protected void Remove(Unit unit) {
        int i = members.IndexOf(unit);
        if (i < unitIdx)
            unitIdx--;
        members.Remove(unit);
        if (members.Count == 0)
            ModalObject.SetMode(ModalObject.Mode.lvlOver);
        else
            unitIdx %= members.Count;
    }

    public void Add(Unit unit) {
        members.Add(unit);
        unit.party = this;
    }

    public bool HasUnit(int unitID) {
        var ids = members.Select(u => u.unitID);
        return ids.Contains(unitID);
    }

    public Unit GetUnit(int unitID) {
        Debug.Assert(HasUnit(unitID));
        return members.Find(member => member.unitID == unitID);
    }

    [Test]
    public void TestParty() {
        List<Unit> units = new List<Unit>();
        for (int i = 0; i < 10; i++) {
            GameObject go = new GameObject();
            go.AddComponent<Unit>();
            units.Add(go.GetComponent<Unit>());
            units.Last().unitID = i;
        }
        GameObject partyObj = new GameObject();
        partyObj.AddComponent<Party>();
        Party party = partyObj.GetComponent<Party>();
        party.members.AddRange(units);
        for (int i = 0; i < 10; i++) {
            Debug.Assert(party.HasUnit(i));
            Debug.Assert(party.cur == units[i]);
            party.CycleNext();
        }

        for (int i = 0; i < 10; i++) {
            party.CyclePrev();
        }
        Debug.Assert(party.cur == units[0]);

        party.Remove(units[9]);
        Debug.Assert(!party.HasUnit(9));
        party.Add(units[9]);
        Debug.Assert(party.HasUnit(9));

        for (int i = 0; i < 9; i++) {
            party.SetInactive(units[i]);
        }
        Debug.Assert(party.members.Count == 1);

        for (int i = 0; i < 9; i++) {
            party.SetActive(units[i]);
        }
        Debug.Assert(party.members.Count == 10);
    }
}
