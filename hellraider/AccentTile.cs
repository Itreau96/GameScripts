using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AccentTile : Tile
{
    #region properties

    // Name of tile accent instance
    public string accentDescription;

    // Array for accent tile images
    public Sprite[] accentImages;

    // Position in tilemap
    public Vector3Int tilePosition;

    // Parent tile map
    public ITilemap tileMap;

    #endregion

    // Start is called before the first frame update
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        // Randomly pick a sprite index
        int index = Random.Range(0, accentImages.Length);
        // Set sprite value
        base.sprite = accentImages[index];

        return base.StartUp(position, tilemap, go);
    }

    #region Asset DataBase

    [MenuItem("Assets/Hellraider/AccentTile")]
    public static void CreateAccentTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Accent Tile", "AccentTile_", "Asset", "Save Accent Tile", "Assets");

        if (path == "")
            return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AccentTile>(), path);
    }

    #endregion
}
