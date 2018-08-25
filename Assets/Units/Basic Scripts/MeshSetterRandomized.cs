using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSetterRandomized : MeshSetter {
    public List<Material> skinTones = new List<Material>();
    protected Material skinMat;

    protected void Start() {
        var wv = GetComponent<WaveCollapse>();
        if (wv != null) {
            wv.ApplyWithRandom();
            presets.AddRange(wv.observations);
        }
        GetDressed();
    }

    public override void SetMesh(SkinnedMeshRenderer skMeshRend, MeshPreset preset) {
        skMeshRend.sharedMesh = preset.mesh;
        var matArray = preset.mats.ToArray();
        base.SetMesh(skMeshRend, preset);
        if (preset.skinMatIndex >= 0) {
            matArray[preset.skinMatIndex] = skinMat;
        }
        skMeshRend.sharedMaterials = matArray;
    }

    public override void GetDressed() {
        skinMat = skinTones[Random.Range(0, skinTones.Count - 1)];
        base.GetDressed();
        //foreach (PresetVariant variant in variants) {
        //    Equip(variant.GetRnd());
        //}
    }
}
