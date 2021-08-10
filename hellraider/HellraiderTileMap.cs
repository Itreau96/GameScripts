using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Numerics;

public enum TilemapType
{
    DESTRUCTABLE,
    INDESTRUCTABLE,
    ACCENT
}

public abstract class HellraiderTileMap : MonoBehaviour
{
    #region Instance Variables

    public TilemapType tilemapType;

    protected Vector2Int size;
    protected Tilemap tilemap;
    protected GridLayout grid;
    protected TileBase tileBase;
    protected Level level;
    protected List<Vector3Int> placements;

    #endregion

    // Load instance variables
    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        grid = GetComponent<GridLayout>();
        level = GetComponentInParent<Level>();
    }

    // Placeholder for subclass start with dynamic tiles
    public virtual void Initialize(List<Vector3Int> placements, TileBase tileBase, Vector2Int size)
    {
        /* New and improved. Implemented in child class. */
        this.placements = placements;
        this.size = size;
        this.tileBase = tileBase;
    }

    // Placeholer for subclass to populate tiles
    public abstract void Generate();

    // Return whether or not a tile exists at the given position
    public bool TileExists(Vector3Int position)
    {
        // Determine if tile exists at position and return
        if (position.x < 0 || position.y < 0 || position.x >= size.x || position.y >= size.y)
        {
            Vector2Int levelPosition = Vector2Int.zero;
            Vector3Int blockPosition = position;
            if (position.x < 0)
            {
                levelPosition.x = -1;
                blockPosition.x = size.x - 1;
            }
            else if (position.x >= size.x)
            {
                levelPosition.x = 1;
                blockPosition.x = 0;
            }
            if (position.y < 0)
            {
                levelPosition.y = -1;
                blockPosition.y = size.y - 1;
            }
            else if (position.y >= size.y)
            {
                levelPosition.y = 1;
                blockPosition.y = 0;
            }
            levelPosition = level.cell.location + levelPosition;
            if (level.levelManager.levels.ContainsKey(levelPosition))
            {
                return level.levelManager.GetLevelAt(levelPosition).BlockAt(tilemapType, blockPosition);
            }
        }
        else if (placements.Contains(position))
        {
            return true;
        }
        // If not tile at position, return false
        return false;
    }

    // Returns whether or not a matching tile exists at given position
    public bool MatchingExists(Vector3Int current, Vector3Int other)
    {
        // First, determine if tile exists
        if (TileExists(other))
        {
            // Determine if same type of tile
            DynamicTile baseTile = tilemap.GetTile(current) as DynamicTile;
            DynamicTile otherTile = tilemap.GetTile(other) as DynamicTile;

            // Return true if names match
            if (baseTile.tileName == otherTile.tileName) return true;
        }

        // Return false by default
        return false;
    }

    // Determines which type of tile to display
    public DynamicTile.TileType GetTileType(Vector3Int position)
    {
        // First, create surrounding positions
        Vector3Int topPos = new Vector3Int(position.x, position.y + 1, position.z);
        Vector3Int bottomPos = new Vector3Int(position.x, position.y - 1, position.z);
        Vector3Int leftPos = new Vector3Int(position.x - 1, position.y, position.z);
        Vector3Int rightPos = new Vector3Int(position.x + 1, position.y, position.z);

        // Cascading if statement to determine type of tile
        if (TileExists(topPos) && !TileExists(bottomPos) && !TileExists(leftPos) && TileExists(rightPos))
        {
            return DynamicTile.TileType.BOTTOM_LEFT_CORNER;
        }
        else if (TileExists(topPos) && !TileExists(bottomPos) && TileExists(leftPos) && !TileExists(rightPos))
        {
            return DynamicTile.TileType.BOTTOM_RIGHT_CORNER;
        }
        else if (TileExists(topPos) && !TileExists(bottomPos) && TileExists(leftPos) && TileExists(rightPos))
        {
            return DynamicTile.TileType.EDGE_BOTTOM;
        }
        else if (!TileExists(topPos) && !TileExists(bottomPos) && TileExists(leftPos) && TileExists(rightPos))
        {
            return DynamicTile.TileType.EDGE_HORIZONTAL;
        }
        else if (TileExists(topPos) && TileExists(bottomPos) && !TileExists(leftPos) && TileExists(rightPos))
        {
            return DynamicTile.TileType.EDGE_LEFT;
        }
        else if (TileExists(topPos) && TileExists(bottomPos) && TileExists(leftPos) && !TileExists(rightPos))
        {
            return DynamicTile.TileType.EDGE_RIGHT;
        }
        else if (!TileExists(topPos) && TileExists(bottomPos) && TileExists(leftPos) && TileExists(rightPos))
        {
            return DynamicTile.TileType.EDGE_TOP;
        }
        else if (TileExists(topPos) && TileExists(bottomPos) && !TileExists(leftPos) && !TileExists(rightPos))
        {
            return DynamicTile.TileType.EDGE_VERTICAL;
        }
        else if (!TileExists(topPos) && TileExists(bottomPos) && !TileExists(leftPos) && !TileExists(rightPos))
        {
            return DynamicTile.TileType.OPEN_BOTTOM;
        }
        else if (!TileExists(topPos) && !TileExists(bottomPos) && TileExists(leftPos) && !TileExists(rightPos))
        {
            return DynamicTile.TileType.OPEN_LEFT;
        }
        else if (!TileExists(topPos) && !TileExists(bottomPos) && !TileExists(leftPos) && TileExists(rightPos))
        {
            return DynamicTile.TileType.OPEN_RIGHT;
        }
        else if (TileExists(topPos) && !TileExists(bottomPos) && !TileExists(leftPos) && !TileExists(rightPos))
        {
            return DynamicTile.TileType.OPEN_TOP;
        }
        else if (!TileExists(topPos) && !TileExists(bottomPos) && !TileExists(leftPos) && !TileExists(rightPos))
        {
            return DynamicTile.TileType.FLOATING;
        }
        else if (!TileExists(topPos) && TileExists(bottomPos) && !TileExists(leftPos) && TileExists(rightPos))
        {
            return DynamicTile.TileType.TOP_LEFT_CORNER;
        }
        else if (!TileExists(topPos) && TileExists(bottomPos) && TileExists(leftPos) && !TileExists(rightPos))
        {
            return DynamicTile.TileType.TOP_RIGHT_CORNER;
        }
        else
        {
            return DynamicTile.TileType.BASE;
        }
    }

    // Function used to delete tile at given position
    public void RemoveTile(Vector3Int position)
    {
        // Set empty at position if tile exists
        if (TileExists(position))
        {
            tilemap.SetTile(position, null);
            placements.Remove(position);
        }
    }
}
