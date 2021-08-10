using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicTile : Tile
{

    #region Properties

    // Name of tile instance
    public string tileName;
    // Parent tile map
    public ITilemap tileMap;
    // Position in tilemap
    public Vector3Int tilePosition;
    // Tile type
    public TileType type = TileType.BASE;

    // Different tile styles
    public Sprite edgeTop;
    public Sprite edgeBottom;
    public Sprite edgeLeft;
    public Sprite edgeRight;
    public Sprite topLeftCorner;
    public Sprite topRightCorner;
    public Sprite bottomLeftCorner;
    public Sprite bottomRightCorner;
    public Sprite openTop;
    public Sprite openBottom;
    public Sprite openLeft;
    public Sprite openRight;
    public Sprite floating;
    public Sprite edgeHorizontal;
    public Sprite edgeVertical;
    public Sprite baseSprite;

    #endregion

    #region Enums

    public enum TileType
    {
        EDGE_TOP, EDGE_BOTTOM, EDGE_LEFT, EDGE_RIGHT,
        TOP_LEFT_CORNER, TOP_RIGHT_CORNER, BOTTOM_LEFT_CORNER,
        BOTTOM_RIGHT_CORNER, OPEN_TOP, OPEN_BOTTOM, OPEN_LEFT,
        OPEN_RIGHT, FLOATING, EDGE_HORIZONTAL, EDGE_VERTICAL, BASE
    }

    #endregion

    #region Tile Overriding

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        // Store tilemap data
        this.tileMap = tilemap;
        this.tilePosition = position;

        return base.StartUp(position, tilemap, go);
    }

    // Set sprite based on tile location
    public void ChangeSprite(TileType type)
    {
        // Set type and sprite
        this.type = type;
        base.sprite = BaseSprite(type);
    }

    // Return sprite based on tile type
    private Sprite BaseSprite(TileType type)
    {
        switch(type)
        {
            case TileType.BOTTOM_LEFT_CORNER:
                return bottomLeftCorner;
            case TileType.BOTTOM_RIGHT_CORNER:
                return bottomRightCorner;
            case TileType.EDGE_BOTTOM:
                return edgeBottom;
            case TileType.EDGE_HORIZONTAL:
                return edgeHorizontal;
            case TileType.EDGE_LEFT:
                return edgeLeft;
            case TileType.EDGE_RIGHT:
                return edgeRight;
            case TileType.EDGE_TOP:
                return edgeTop;
            case TileType.EDGE_VERTICAL:
                return edgeVertical;
            case TileType.FLOATING:
                return floating;
            case TileType.OPEN_BOTTOM:
                return openBottom;
            case TileType.OPEN_LEFT:
                return openLeft;
            case TileType.OPEN_RIGHT:
                return openRight;
            case TileType.OPEN_TOP:
                return openTop;
            case TileType.TOP_LEFT_CORNER:
                return topLeftCorner;
            case TileType.TOP_RIGHT_CORNER:
                return topRightCorner;
            default:
                return baseSprite;
        }
    }

    #endregion
}