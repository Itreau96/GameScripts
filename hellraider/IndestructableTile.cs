using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class IndestructableTile : DynamicTile
{
    #region Class Variables

    #endregion

    #region Asset DataBase

    [MenuItem("Assets/Hellraider/IndestructibleTile")]
    public static void CreateDestructibleTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Indestructible Tile", "IndestructableTile_", "Asset", "Save Indestructable Tile", "Assets");

        if (path == "")
            return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<IndestructableTile>(), path);
    }

    #endregion
}
