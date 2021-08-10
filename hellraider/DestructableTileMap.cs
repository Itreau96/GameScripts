using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructableTileMap : HellraiderTileMap
{
    #region Properties

    public Vector2 entryPoint;
    public Vector2 exitPoint;
    public GameObject breakAnim;

    #endregion


    #region Unity callbacks

    // Generates tiles based on placement information (must be done after placements are provided)
    public override void Generate()
    {
        tilemapType = TilemapType.DESTRUCTABLE;
        foreach (Vector3Int placement in placements)
        {
            DestructableTile dt = Instantiate(tileBase) as DestructableTile;
            dt.StartUp(placement, dt.tileMap, dt.gameObject);
            dt.ChangeSprite(GetTileType(placement));
            tilemap.SetTile(placement, dt);
        }
    }

    // Create tile at given position
    public void AddTile(Vector3Int position)
    {
        DestructableTile dt = Instantiate(tileBase) as DestructableTile;
        dt.StartUp(position, dt.tileMap, dt.gameObject);
        dt.ChangeSprite(GetTileType(position));
        tilemap.SetTile(position, dt);
        placements.Add(position);
    }

    #endregion

    // Set position of tilemap
    public void SetPosition(float x, float y)
    {
        // Create vector and change position
        Vector2 newPos = new Vector2(x, y);
        transform.position = newPos;
    }

    // Damage tile at point
    public void DamageTile(Vector3 contactPoint, bool explosive)
    {
        TileBase tileToDamage = tilemap.GetTile(grid.WorldToCell(contactPoint));
        if (!Equals(tileToDamage, null))
        {
            if (tileToDamage is DestructableTile)
            {
                Sprite damageSprite = ((DestructableTile)tileToDamage).ApplyDamage(explosive);
                if (damageSprite)
                {
                    level.DamageTile(damageSprite, ((DestructableTile)tileToDamage).tilePosition);
                    tilemap.RefreshTile(((DestructableTile)tileToDamage).tilePosition);
                }
                else
                {
                    BreakTile(((DestructableTile)tileToDamage).tilePosition);
                }
            }
        }
    }

    // Used to break tile and restructure sprites
    public void BreakTile(Vector3Int position)
    {
        // First, create surrounding positions
        Vector3Int topPos = new Vector3Int(position.x, position.y + 1, position.z);
        Vector3Int bottomPos = new Vector3Int(position.x, position.y - 1, position.z);
        Vector3Int leftPos = new Vector3Int(position.x - 1, position.y, position.z);
        Vector3Int rightPos = new Vector3Int(position.x + 1, position.y, position.z);

        // Tile contact position
        Vector3 worldPos = tilemap.GetCellCenterWorld(position);

        // Delete tile at position
        RemoveTile(position);

        // Restructure surrounding tiles
        if (TileExists(topPos))
        {
            TileBase t = tilemap.GetTile(topPos);
            DynamicTile.TileType curTileType = GetTileType(topPos);
            if (t is DestructableTile)
            {
                if (((DestructableTile)t).type != curTileType)
                {
                    ((DestructableTile)t).ChangeSprite(curTileType);
                    tilemap.RefreshTile(((DestructableTile)t).tilePosition);
                }
            }
        }
        if (TileExists(bottomPos))
        {
            TileBase t = tilemap.GetTile(bottomPos);
            DynamicTile.TileType curTileType = GetTileType(bottomPos);
            if (t is DestructableTile)
            {
                if (((DestructableTile)t).type != curTileType)
                {
                    ((DestructableTile)t).ChangeSprite(curTileType);
                    tilemap.RefreshTile(((DestructableTile)t).tilePosition);
                }
            }
        }
        if (TileExists(leftPos))
        {
            TileBase t = tilemap.GetTile(leftPos);
            DynamicTile.TileType curTileType = GetTileType(leftPos);
            if (t is DestructableTile)
            {
                if (((DestructableTile)t).type != curTileType)
                {
                    ((DestructableTile)t).ChangeSprite(curTileType);
                    tilemap.RefreshTile(((DestructableTile)t).tilePosition);
                }
            }
        }
        if (TileExists(rightPos))
        {
            TileBase t = tilemap.GetTile(rightPos);
            DynamicTile.TileType curTileType = GetTileType(rightPos);
            if (t is DestructableTile)
            {
                if (((DestructableTile)t).type != curTileType)
                {
                    ((DestructableTile)t).ChangeSprite(curTileType);
                    tilemap.RefreshTile(((DestructableTile)t).tilePosition);
                }
            }
        }
        // Call on other maps
        level.BreakTile(position, topPos);

        // Deploy break animation
        Instantiate(breakAnim, worldPos, Quaternion.identity);
    }
}