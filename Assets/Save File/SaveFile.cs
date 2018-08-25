using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.SqliteClient;
using SQLiter;
using System.Data;
using NUnit.Framework;
using UnityEngine.TestTools;

[CreateAssetMenu(menuName = "Custom/Save File")]
public class SaveFile : ScriptableObject {
    public string dbLocation = "URI=file:save.db";
    public IDataReader   rdr;
    public IDbConnection cnx;
    public IDbCommand    cmd;

    #region InitStuff
    public void Init() {
        cnx = new SqliteConnection(dbLocation);
        cmd = cnx.CreateCommand();

        cnx.Open();
        cmd.CommandText = "PRAGMA journal_mode = WAL;";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "PRAGMA synchronous = OFF;";
        cmd.ExecuteNonQuery();
        cnx.Close();

        InitEqpTbl();
        InitLoadoutTbl();
        InitCharSheetTbl();
        InitOccupied();
        InitOnMission();
    }

    protected void InitEqpTbl() {
        cnx.Open();
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS equipment "
                        + "( equipmentID    INTEGER PRIMARY KEY"
                        + ", onHand         INTEGER"
                        + ");";
        cmd.ExecuteNonQuery();
        cnx.Close();
    }

    protected void InitLoadoutTbl() {
        cnx.Open();
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS loadout"
                        + "( unitID      INTEGER"
                        + ", equipmentID INTEGER"
                        + ", FOREIGN KEY (equipmentID) REFERENCES equipment(equipmentID)"
                        + ");";
        cmd.ExecuteNonQuery();
        cnx.Close();
    }

    protected void InitCharSheetTbl() {
        cnx.Open();
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS charSheets "
                        + "( unitID    INTEGER PRIMARY KEY"
                        + ", unitName  STRING "
                        + ", charClass INTEGER"
                        + ", xp        INTEGER"
                        + ", boots     INTEGER"
                        + ", btm       INTEGER"
                        + ", handL     INTEGER"
                        + ", top       INTEGER"
                        + ", handR     INTEGER"
                        + ", head      INTEGER"
                        + ");";
        cmd.ExecuteNonQuery();
        cnx.Close();
    }

    protected void InitOccupied() {
        cnx.Open();
        // We assume this is a sparse matrix and only store
        // those positions which are occupied.
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS occupiedMtx "
                        + "( level INTEGER"
                        + ", x     INTEGER"
                        + ", y     INTEGER"
                        + ");";
        cmd.ExecuteNonQuery();
        cnx.Close();
    }

    protected void InitOnMission() {
        cnx.Open();
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS onMission "
                        + "( unitID INTEGER UNIQUE"
                        + ", FOREIGN KEY (unitID) REFERENCES charSheets(unitID)"
                        + ");";
        cmd.ExecuteNonQuery();
        cnx.Close();
    }
    #endregion

    #region UnitStuff
    /// <summary>
    /// Add a character sheet to the database.
    /// </summary>
    public void Add(CharSheet charSheet) {
        cnx.Open();
        cmd.CommandText = string.Format
            ( "INSERT INTO charSheets "
            + "( unitID"
            + ", unitName"
            + ", xp"
            + ", charClass"
            + ", head"
            + ", top"
            + ", btm"
            + ", boots"
            + ", handL"
            + ", handR"
            + ") VALUES "
            + "({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9});"
            , charSheet.unitID
            , charSheet.unitName
            , charSheet.xp
            , (int)charSheet.unitType
            , charSheet.head
            , charSheet.top
            , charSheet.btm
            , charSheet.boots
            , charSheet.handL
            , charSheet.handR
            );
        cmd.ExecuteNonQuery();
        cnx.Close();
    }

    public bool Has(int unitID) {
        cnx.Open();
        cmd.CommandText = "SELECT EXISTS(SELECT 1 FROM charSheets WHERE unitID=" + unitID + ");";
        rdr = cmd.ExecuteReader();
        Debug.Assert(rdr.Read());
        bool exists = rdr.GetBoolean(0);
        cnx.Close();
        return exists;
    }

    public void RemoveUnit(int unitID) {
        cnx.Open();
        cmd.CommandText = "DELETE FROM charSheets WHERE unitID=" + unitID + ";";
        cmd.ExecuteNonQuery();
        cnx.Close();
    }

    public CharSheet GetCharSheet(int unitID) {
        cnx.Open();
        cmd.CommandText = "SELECT unitID, unitName, charClass, xp, "
                        + "boots, btm, handL, top, handR, head "
                        + "FROM charSheets "
                        + "WHERE unitID="
                        + unitID
                        + ";";
        rdr = cmd.ExecuteReader();
        var charSheet = new CharSheet();
        if (rdr.Read()) {
            charSheet.unitID   = rdr.GetInt32(0);
            charSheet.unitName = rdr.GetString(1);
            charSheet.unitType = (UnitType)rdr.GetInt32(2);
            charSheet.xp       = rdr.GetInt32(3);
            charSheet.boots    = rdr.GetInt32(4);
            charSheet.btm      = rdr.GetInt32(5);
            charSheet.handL    = rdr.GetInt32(6);
            charSheet.top      = rdr.GetInt32(7);
            charSheet.handR    = rdr.GetInt32(8);
            charSheet.head     = rdr.GetInt32(9);
        }
        cnx.Close();
        Debug.Assert(charSheet.unitID == unitID);
        return charSheet;
    }

    public List<CharSheet> GetAllUnits() {
        // for simplicity, we'll get the IDs in one step,
        // then use the pre-existing GetCharSheet to fill
        // out the actual character sheet second.
        cnx.Open();
        cmd.CommandText = "SELECT unitID FROM charSheets;";
        rdr = cmd.ExecuteReader();
        List<int> ids = new List<int>();
        while (rdr.Read()) {
            ids.Add(rdr.GetInt32(0));
        }
        cnx.Close();

        List<CharSheet> retrieved = new List<CharSheet>();
        foreach (int unitID in ids) {
            retrieved.Add(GetCharSheet(unitID));
        }

        return retrieved;
    }

    public int GetBiggestUnitID() {
        cnx.Open();
        cmd.CommandText = "SELECT MAX(unitID) FROM charSheets;";
        rdr = cmd.ExecuteReader();
        int biggest = rdr.Read() ? rdr.GetInt32(0) : 0;
        cnx.Close();
        return biggest;
    }

    public void Equip(int unitID, MeshPreset.Slot slot, int eqpID) {
        cnx.Open();
        string s = "UPDATE charSheets SET '{0}'={1} WHERE unitID={2};";
        string slotName;

        switch (slot) {
            case MeshPreset.Slot.Boots:
                slotName = "boots";
                break;

            case MeshPreset.Slot.Bottom:
                slotName = "btm";
                break;

            case MeshPreset.Slot.LeftHand:
                slotName = "handL";
                break;

            case MeshPreset.Slot.Top:
                slotName = "top";
                break;

            case MeshPreset.Slot.RightHand:
                slotName = "handR";
                break;

            case MeshPreset.Slot.Head:
                slotName = "head";
                break;

            default:
                throw new System.Exception("Unknown mesh slot type encountered in SaveFile.cs.");
        }

        cmd.CommandText = string.Format(s, slotName, eqpID, unitID);
        cmd.ExecuteNonQuery();
        cnx.Close();
    }
    #endregion

    #region MissionStuff
    public void ResetMission() {
        cnx.Open();
        cmd.CommandText = "DELETE FROM onMission;";
        cmd.ExecuteNonQuery();
        cnx.Close();
    }

    public void PutOnMission(int unitID) {
        cnx.Open();
        cmd.CommandText = "INSERT OR IGNORE INTO onMission (unitID) "
                        + "VALUES (" + unitID + ");";
        cmd.ExecuteNonQuery();
        cnx.Close();
    }

    public void TakeOffMission(int unitID) {
        cnx.Open();
        cmd.CommandText = "DELETE FROM onMission WHERE unitID=" + unitID + ";";
        cmd.ExecuteNonQuery();
        cnx.Close();
    }

    public List<int> GetUnitsOnMission() {
        cnx.Open();
        cmd.CommandText = "SELECT unitID FROM onMission;";
        rdr = cmd.ExecuteReader();
        List<int> ids = new List<int>();
        while (rdr.Read()) {
            ids.Add(rdr.GetInt32(0));
        }
        cnx.Close();
        return ids;
    }
    #endregion

    #region BuyingStuff
    public void SetAmtOnHand(int eqpID, int onHand) {
        cnx.Open();
        cmd.CommandText = string.Format("INSERT OR REPLACE INTO equipment"
                                       + "(equipmentID, onHand) "
                                       + "VALUES ({0}, {1});    "
                                       , eqpID
                                       , onHand
                                       );
        cmd.ExecuteNonQuery();
        cnx.Close();
    }

    public int GetAmtOnHand(int eqpID) {
        cnx.Open();
        cmd.CommandText = "SELECT onHand "
                        + "FROM equipment "
                        + "WHERE equipmentID="
                        + eqpID
                        + ";";
        rdr = cmd.ExecuteReader();
        int onHand = rdr.Read() ? rdr.GetInt32(0) : 0;
        cnx.Close();
        return onHand;
    }
    #endregion

    public void CloseDatabase() {
        // Close the database.
        if (rdr != null && !rdr.IsClosed)
            rdr.Close();

        if (cmd != null)
            cmd.Dispose();
        cmd = null;

        if (cnx != null && cnx.State != ConnectionState.Closed)
            cnx.Close();
        cnx = null;

    }
}


public class TestSaveFile {
    SaveFile save;

    [SetUp]
    public void SetUp() {
        save = ScriptableObject.CreateInstance<SaveFile>();
        save.dbLocation = "URI=file:test.db";
        save.Init();

        // Make sure we start with a clean slate.
        save.cnx.Open();
        save.cmd.CommandText = "SELECT * FROM sqlite_master WHERE type='table';";
        save.rdr = save.cmd.ExecuteReader();
        List<string> results = new List<string>();
        while (save.rdr.Read()) {
            results.Add(save.rdr.GetString(1));
        }
        save.cnx.Close();

        foreach (string result in results) {
            save.cnx.Open();
            save.cmd.CommandText = "DROP TABLE " + result + ";";
            save.cmd.ExecuteNonQuery();
            save.cnx.Close();
        }

        save.Init();
    }

    private CharSheet MakeCharSheet() {
        return new CharSheet() {
            unitID   = UnityEngine.Random.Range(0, 99),
            unitName = "Adam",
            unitType = UnitType.infantry,
            xp       = 99,
            head     = 0,
            top      = 1,
            btm      = 2,
            boots    = 3,
            handL    = 4,
            handR    = 5
        };
    }

    [Test]
    public void TestAdd_GetAll() {
        CharSheet sheet = MakeCharSheet();
        sheet.unitID = 123;
        save.Add(sheet);
        Debug.Assert(save.Has(sheet.unitID));

        sheet.unitID = 234;
        save.Add(sheet);
        Debug.Assert(save.Has(sheet.unitID));

        sheet.unitID = 345;
        save.Add(sheet);
        Debug.Assert(save.Has(sheet.unitID));

        List<CharSheet> sheets = save.GetAllUnits();
        Debug.Assert(sheets.Count == 3);
    }

    [Test]
    public void TestGetBiggestID() {
        CharSheet sheet = MakeCharSheet();
        sheet.unitID = 1;
        save.Add(sheet);
        Debug.Assert(save.GetBiggestUnitID() == 1);

        sheet.unitID = 4;
        save.Add(sheet);
        Debug.Assert(save.GetBiggestUnitID() == 4);

        sheet.unitID = 3;
        save.Add(sheet);
        Debug.Assert(save.GetBiggestUnitID() == 4);

        sheet.unitID = 4;
        save.RemoveUnit(4);
        Debug.Assert(save.GetBiggestUnitID() == 3);
    }

    [Test]
    public void TestRemove() {
        CharSheet sheet = MakeCharSheet();
        save.Add(sheet);
        save.RemoveUnit(sheet.unitID);
        Debug.Assert(!save.Has(sheet.unitID));
    }

    [Test]
    public void TestMissionStuff() {
        List<CharSheet> sheets = new List<CharSheet>();
        for (int i = 0; i < 10; i++) {
            sheets.Add(MakeCharSheet());
        }

        foreach (CharSheet sheet in sheets) {
            save.Add(sheet);
            save.PutOnMission(sheet.unitID);
            Debug.Assert(save.Has(sheet.unitID));
        }

        List<int> allOnMission = save.GetUnitsOnMission();
        Debug.Assert(allOnMission.Count == 10);
        foreach (CharSheet sheet in sheets) {
            Debug.Assert(allOnMission.Contains(sheet.unitID));
        }

        List<int> rnds = new List<int>();
        for (int i = 0; i < 4; i++) {
            rnds.Add(UnityEngine.Random.Range(0, allOnMission.Count - 1));
        }

        foreach (int rnd in rnds) {
            save.TakeOffMission(allOnMission[rnd]);
        }

        List<int> nowOnMission = save.GetUnitsOnMission();
        for (int i = 0; i < allOnMission.Count; i++) {
            if (rnds.Contains(i))
                Debug.Assert(!nowOnMission.Contains(allOnMission[i]));
            else
                Debug.Assert( nowOnMission.Contains(allOnMission[i]));
        }

        save.ResetMission();
        Debug.Assert(save.GetUnitsOnMission().Count == 0);
    }

    [Test]
    public void TestBuyingStuff() {
        save.SetAmtOnHand(3, 99);
        Debug.Assert(save.GetAmtOnHand(3) == 99);
        save.SetAmtOnHand(3, 98);
        Debug.Assert(save.GetAmtOnHand(3) == 98);
        save.SetAmtOnHand(2, 1);
        Debug.Assert(save.GetAmtOnHand( 2) == 1);
        Debug.Assert(save.GetAmtOnHand(99) == 0);
    }

    [Test]
    public void TestEquip() {
        CharSheet cs = MakeCharSheet();
        save.Add(cs);

        List<int> rnds = new List<int>();
        for (int i = 0; i < 6; i++) {
            rnds.Add(UnityEngine.Random.Range(0, 999));
        }
        save.Equip(cs.unitID, MeshPreset.Slot.Boots    , rnds[0]);
        save.Equip(cs.unitID, MeshPreset.Slot.Bottom   , rnds[1]);
        save.Equip(cs.unitID, MeshPreset.Slot.Head     , rnds[2]);
        save.Equip(cs.unitID, MeshPreset.Slot.LeftHand , rnds[3]);
        save.Equip(cs.unitID, MeshPreset.Slot.RightHand, rnds[4]);
        save.Equip(cs.unitID, MeshPreset.Slot.Top      , rnds[5]);

        var cs2 = save.GetCharSheet(cs.unitID);
        Debug.Assert(cs2.head == rnds[2]);
        save.Equip(cs.unitID, MeshPreset.Slot.Head, 1000);
        cs2 = save.GetCharSheet(cs.unitID);
        Debug.Assert(cs2.head != rnds[2]);
        Debug.Assert(cs2.head == 1000);
    }

    [TearDown]
    public void TearDown() {
        save.CloseDatabase();
    }
}
