using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using UnityEngine.TestTools;

/// <summary>
/// Saves and loads multidimensional arrays to disk.
/// </summary>
[CreateAssetMenu(menuName = "Custom Unique/Array Save and Load")]
public class ArraySaveLoad : ScriptableObject {
    /// <summary>
    /// Determine the complete file path to save the file as
    /// given the name of the thing you're trying to save.
    /// For example "distances" is good, but something like
    /// "distances_lvl2.dat" is unnecessary.
    /// </summary>
    protected string BuildPath(string saveType) {
        int lvl = SceneManager.GetActiveScene().buildIndex;
        Debug.Assert(lvl > -1);
        string fname = string.Format("{0}{1}.dat", saveType, lvl);
        string fpath = Path.Combine(Application.streamingAssetsPath, fname);
        return fpath;
   }

    protected void SaveData_<T>(T data, string saveType) {
        string fpath = BuildPath(saveType);
        using (Stream stream = File.Open(fpath, FileMode.Create)) {
            var binFmt = new BinaryFormatter();
            binFmt.Serialize(stream, data);
        }
    }

    /// <summary> Save a serialized 1D array to disk. </summary>
    public void SaveData<T>(T[] data, string saveType) {
        SaveData_(data, saveType);
    }

    /// <summary> Save a serialized 2D array to disk. </summary>
    public void SaveData<T>(T[,] data, string saveType) {
        SaveData_(data, saveType);
    }

    /// <summary> Save a serialized 3D array to disk. </summary>
    public void SaveData<T>(T[,,] data, string saveType) {
        SaveData_(data, saveType);
    }

    /// <summary> Load a serialized 1D array from disk. </summary>
    public T[] LoadArray<T>(string saveType) {
        string fpath = BuildPath(saveType);
        using (Stream stream = File.Open(fpath, FileMode.Open)) {
            var binFmt = new BinaryFormatter();
            return binFmt.Deserialize(stream) as T[];
        }
    }

    /// <summary> Load a serialized 2D array from disk. </summary>
    public T[,] LoadMatrix<T>(string saveType) {
        string fpath = BuildPath(saveType);
        using (Stream stream = File.Open(fpath, FileMode.Open)) {
            var binFmt = new BinaryFormatter();
            return binFmt.Deserialize(stream) as T[,];
        }
    }

    /// <summary> Load a serialized 3D array from disk. </summary>
    public T[,,] LoadTensor<T>(string saveType) {
        string fpath = BuildPath(saveType);
        using (Stream stream = File.Open(fpath, FileMode.Open)) {
            var binFmt = new BinaryFormatter();
            return binFmt.Deserialize(stream) as T[,,];
        }
    }
}
