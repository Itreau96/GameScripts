using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DamageTileMap : HellraiderTileMap
{
    // Instance variables
    public Tile damageTilePrefab;

    // Placeholder function
    public override void Generate()
    {
        // Nothing for now
    }

    // Add damage tile if doesn't exist already
    public void CreateDamage(Sprite spriteBase, Vector3Int tilePos)
    {
        // First, determine if broken sprite returning
        if (spriteBase == null)
        {
            tilemap.SetTile(tilePos, null);
            return;
        }
        else
        {
            // If tile exists, change sprite
            if (TileExists(tilePos))
            {
                Tile t = tilemap.GetTile(tilePos) as Tile;
                t.sprite = spriteBase;
                tilemap.RefreshTile(tilePos);
            }
            else
            {
                Tile newTile = Instantiate(damageTilePrefab);
                newTile.sprite = spriteBase;
                tilemap.SetTile(tilePos, newTile);
                placements.Add(tilePos);
            }
        }
    }
}
