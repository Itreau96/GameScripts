using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AccentTileMap : HellraiderTileMap
{
    #region Properties

    public GameObject breakAnim;

    #endregion

    // Generates tiles based on placement information (must be done after placements are provided)
    public override void Generate()
    {
        tilemapType = TilemapType.ACCENT;
        foreach (Vector3Int placement in placements)
        {
            AddAccent(placement);
        }
    }

    public void AddAccent(Vector3Int position)
    {
        AccentTile dt = Instantiate(tileBase) as AccentTile;
        dt.StartUp(position, dt.tileMap, dt.gameObject);
        tilemap.SetTile(position, dt);
    }

    // Remove tile if exists and deploy break animation
    public void RemoveAccent(Vector3Int position)
    {
        // Check if accent tile at position
        if (TileExists(position))
        {
            RemoveTile(position);

            // Deploy break animation
            Vector3 accentPos = tilemap.GetCellCenterWorld(position);
            Instantiate(breakAnim, accentPos, Quaternion.identity);
        }
    }
}
