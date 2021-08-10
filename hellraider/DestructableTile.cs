using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructableTile : DynamicTile
{
    #region properties
    [Space(20)]
    [Header("Destructible Tile")]

    // Sprites to display 
    public Sprite[] brokenSprites;
    private int breakIndex;

    // Damage tile image
    public GameObject damageSpritePrefab;
    private Sprite damagedSprite;

    #endregion

    #region Implementation

    // Apply damage to the tile
    public Sprite ApplyDamage(bool explosion)
    {
        // If maximum reached or explosive damage, return empty
        if (breakIndex == brokenSprites.Length || explosion)
        {
            base.sprite = null;
            return null;
        }
        else
        {
            // Change sprite image
            damagedSprite = brokenSprites[breakIndex];
            // Increase breakindex
            breakIndex++;
            // Return damaged sprite
            return damagedSprite;
        }
    }

    #endregion

    #region Asset DataBase

    [MenuItem("Assets/Hellraider/DestructibleTile")]
    public static void CreateDestructibleTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Destructible Tile", "DestructableTile_", "Asset", "Save Dynamic Tile", "Assets");

        if (path == "")
            return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DestructableTile>(), path);
    }

    #endregion
}