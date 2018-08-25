using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RenameColors : Editor {
    /// <summary>
    /// Rename the selected color to it's HSL value for easy sorting.
    /// </summary>
    [MenuItem("Custom/Rename Colors")]
    public static void RenameSelected() {
        var things = Selection.objects;
        foreach (object selected in Selection.objects) {
            Material mat = selected as Material;
            if (mat != null) {
                float h, s, v;
                Color.RGBToHSV(mat.color, out h, out s, out v);
                // These are just arbitrary quanitzation values.
                // Too high at it will care solely about the hue.
                // Too low and you'll wind up with duplicate names.
                int quant_h = Mathf.RoundToInt(h * 8);
                int quant_s = Mathf.RoundToInt(s * 8);
                int quant_v = Mathf.RoundToInt(v * 8);
                mat.name = string.Format("({0}{1}{2})", quant_h, quant_s, quant_v);

                // Now, to ensure uniqueness, we're gonna add the hex anyway.
                string hex_h = Mathf.RoundToInt(h * 255).ToString("X");
                string hex_s = Mathf.RoundToInt(s * 255).ToString("X");
                string hex_v = Mathf.RoundToInt(v * 255).ToString("X");
                mat.name += string.Format("{0}{1}{2}", hex_h, hex_s, hex_v);
                string path = AssetDatabase.GetAssetPath(mat.GetInstanceID());
                AssetDatabase.RenameAsset(path, mat.name);
            }
        }
    }
}
