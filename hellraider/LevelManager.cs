using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region Instance Variables

    public GameObject levelPrefab;
    public GameObject grid;
    public TileRegistry tileReg;
    public EnemyRegistry enemyReg;
    public ItemRegistry itemReg;
    public WeaponRegistry weaponReg;
    public Dictionary<Vector2Int, GameObject> levels;

    public MazeGenerator mazeGen;
    public MapGenerator mapGen;
    int cellsX = 5;
    int cellsY = 5;
    int blockX = 20;
    int blockY = 20;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        levels = new Dictionary<Vector2Int, GameObject>();
        mapGen = GetComponent<MapGenerator>();
        mazeGen = GetComponent<MazeGenerator>();
        Map map = mazeGen.CreateMaze(cellsY, cellsX);
        CreateLevels(map);
    }

    void CreateLevels(Map map)
    {
        for (var x = 0; x < cellsX; x++)
        {
            for (var y = 0; y < cellsY; y++)
            {
                levels[new Vector2Int(x, y)] = CreateLevel(map.cells[x, y]);
            }
        }
        // Need to initialize separately in order to detect placements
        foreach (var level in levels)
        {
            level.Value.GetComponent<Level>().Generate();
        }
    }

    // Instantiate level
    public GameObject CreateLevel(Cell cell)
    {
        GameObject newLevel = Instantiate(levelPrefab, new Vector3Int(0, 0, 0), Quaternion.identity, grid.transform);
        newLevel.GetComponent<Level>().Initialize(cell, new Vector2Int(blockX, blockY), this);
        return newLevel;
    }

    // Helper method for getting level at specified position
    public Level GetLevelAt(Vector2Int position)
    {
        return levels[position].GetComponent<Level>();
    }
}