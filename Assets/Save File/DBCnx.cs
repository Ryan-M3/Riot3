using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.SqliteClient;
using SQLiter;
using System.Data;


public class DBCnx : MonoBehaviour {
    protected IDbConnection cnx;
    protected IDbCommand cmd;
    protected IDataReader rdr;
    private static string dbLocation = "URI=file:SAVE_FILE.db";

    protected virtual void Awake() {
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
                        + "( unitID    INTEGER PRIMARY KEY AUTOINCREMENT"
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

    private void OnDestroy() {
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
