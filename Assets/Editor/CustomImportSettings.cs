using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CustomImportSettings : AssetPostprocessor {
    void OnPreprocessModel() {
        ModelImporter importer = assetImporter as ModelImporter;
        importer.importMaterials = false;
        importer.importLights    = false;
        importer.importCameras   = false;
        //importer.importAnimation = false;
    }
}
