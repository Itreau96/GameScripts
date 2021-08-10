using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class IndestructTileMap : HellraiderTileMap
{
    // Generates tiles based on placement information (must be done after placements are provided)
    public override void Generate()
    {
        tilemapType = TilemapType.INDESTRUCTABLE;
        foreach (Vector3Int placement in placements)
        {
            IndestructableTile dt = Instantiate(tileBase) as IndestructableTile;
            dt.StartUp(placement, dt.tileMap, dt.gameObject);
            dt.ChangeSprite(GetTileType(placement));
            tilemap.SetTile(placement, dt);
        }
    }
}
