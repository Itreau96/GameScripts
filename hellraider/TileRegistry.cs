using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileRegistry : MonoBehaviour
{
    #region Instance Variables

    public TileBase[] destructableTiles;
    public TileBase[] indestructableTiles;
    public TileBase[] accentTiles;
    public TileBase[] damageTiles;

    #endregion
}
