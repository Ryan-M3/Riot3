using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSetter : MonoBehaviour {
    public List<UnitItem> presets = new List<UnitItem>();
    public SkinnedMeshRenderer boots;
    public SkinnedMeshRenderer btm;
    public SkinnedMeshRenderer top;
    public SkinnedMeshRenderer handL;
    public SkinnedMeshRenderer handR;
    public SkinnedMeshRenderer head;

    protected virtual void Awake() {
        GetDressed();
    }

    public void Clear() {
        presets.Clear();
    }

    public virtual void SetMesh(SkinnedMeshRenderer skMeshRend, MeshPreset preset) {
        skMeshRend.sharedMesh = preset.mesh;
        // Unity doesn't allow this to be set without making
        // a local copy of it and also doesn't tell you that
        // such an assignment didn't work.
        skMeshRend.sharedMaterials = preset.mats.ToArray();
    }

    public void Equip(MeshPreset preset) {
        if (preset == null)
            Debug.LogError("Null preset encountered.");

        switch (preset.meshSlot) {
            case MeshPreset.Slot.Head:
                SetMesh(head, preset);
                break;

            case MeshPreset.Slot.Top:
                SetMesh(top, preset);
                break;

            case MeshPreset.Slot.Bottom:
                SetMesh(btm, preset);
                break;

            case MeshPreset.Slot.Boots:
                SetMesh(boots, preset);
                break;

            case MeshPreset.Slot.LeftHand:
                SetMesh(handL, preset);
                break;

            case MeshPreset.Slot.RightHand:
                SetMesh(handR, preset);
                break;

            default:
                Debug.LogError("Mesh Setter encountered an unrecognized preset.");
                break;
        }
    }

    public virtual void GetDressed() {
        foreach (UnitItem preset in presets) {
            Equip(preset.meshPreset);
        }
    }
}
