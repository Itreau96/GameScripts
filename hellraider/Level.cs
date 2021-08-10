using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Data;

public enum LevelType
{
    START,
    ENTRANCE,
    EXIT
}

public class Level : MonoBehaviour
{
    #region Constants

    const int NUM_ENEMIES = 3;
    const int NUM_ITEMS = 3;

    #endregion

    #region Prefabs

    public GameObject destructablePrefab;
    public GameObject accentPrefab;
    public GameObject damagePrefab;
    public GameObject indestructablePrefab;

    #endregion

    #region Instance Variables

    public LevelManager levelManager;
    public LevelType levelType;
    public Cell cell;
    public Vector2Int size;

    GameObject destructableMap;
    GameObject accentMap;
    GameObject damageMap;
    GameObject indestructableMap;
    List<GameObject> enemies;
    List<GameObject> items;

    int indestructIndex;
    int destructIndex;
    int accentIndex;

    List<Vector3Int> indestructablePlacements;
    List<Vector3Int> destructablePlacements;
    List<Vector3Int> accentPlacements;
    List<Vector3Int> blankPlacements;
    List<Vector3Int> enemyPlacements;
    List<GameObject> breakSubscribers;

    #endregion

    #region Instantiation

    public virtual void Initialize(Cell cell, Vector2Int size, LevelManager levelManager)
    {
        this.cell = cell;
        this.size = size;
        breakSubscribers = new List<GameObject>();
        transform.position = (Vector3Int)(cell.location * size);
        this.levelManager = levelManager;

        indestructIndex = Random.Range(0, levelManager.tileReg.indestructableTiles.Length);
        destructIndex = Random.Range(0, levelManager.tileReg.destructableTiles.Length);
        accentIndex = Random.Range(0, levelManager.tileReg.accentTiles.Length);

        // Start by generating base placements
        var basePlacements = levelManager.mapGen.CreateBaseTiles(new Vector2Int(size.x, size.y), cell);
        indestructablePlacements = basePlacements.Item1;
        destructablePlacements = basePlacements.Item2;

        // Get sub placements
        var additionalDestructable = levelManager.mapGen.CreateParallelMap(Vector2Int.one * 2, size - Vector2Int.one * 4);
        destructablePlacements.AddRange(additionalDestructable);

        var tempBlankPlacements = levelManager.mapGen.BlankSpaces(levelManager.mapGen.OverlayMaps(destructablePlacements, indestructablePlacements), size);
        accentPlacements = levelManager.mapGen.RandomPlacement(tempBlankPlacements, 3);

        indestructableMap = CreateIndestructableMap();
        destructableMap = CreateDestructableMap();
        damageMap = CreateDamageMap();
        accentMap = CreateAccentMap();

        blankPlacements = levelManager.mapGen.BlankSpaces(levelManager.mapGen.OverlayMaps(indestructablePlacements, destructablePlacements), size);
        //damageMap = CreateDamageMap();
        //enemies = CreateEnemies();
        //items = CreateItems();
    }

    public virtual void Generate()
    {
        // Populate map
        PopulateIndestructableMap();
        PopulateDestructableMap();
        PopulateAccentMap();
    }

    GameObject CreateIndestructableMap()
    {
        GameObject indestructable = Instantiate(indestructablePrefab, transform.position, Quaternion.identity, gameObject.transform);
        indestructable.GetComponent<IndestructTileMap>().Initialize(indestructablePlacements, levelManager.tileReg.indestructableTiles[indestructIndex], size);
        return indestructable;
    }

    GameObject CreateDestructableMap()
    {
        GameObject destructable = Instantiate(destructablePrefab, transform.position, Quaternion.identity, gameObject.transform);
        destructable.GetComponent<DestructableTileMap>().Initialize(destructablePlacements, levelManager.tileReg.destructableTiles[destructIndex], size);
        return destructable;
    }
    GameObject CreateDamageMap()
    {
        List<Vector3Int> placements = new List<Vector3Int>();
        GameObject damage = Instantiate(damagePrefab, transform.position, Quaternion.identity, gameObject.transform);
        damage.GetComponent<DamageTileMap>().Initialize(placements, levelManager.tileReg.damageTiles[accentIndex], size);
        return damage;
    }

    GameObject CreateAccentMap()
    {
        GameObject accent = Instantiate(accentPrefab, transform.position, Quaternion.identity, gameObject.transform);
        accent.GetComponent<AccentTileMap>().Initialize(accentPlacements, levelManager.tileReg.accentTiles[accentIndex], size);
        return accent;
    }

    void PopulateIndestructableMap()
    {
        indestructableMap.GetComponent<IndestructTileMap>().Generate();
    }

    void PopulateDestructableMap()
    {
        destructableMap.GetComponent<DestructableTileMap>().Generate();
    }

    void PopulateAccentMap()
    {
        accentMap.GetComponent<AccentTileMap>().Generate();
    }

    /*
    GameObject CreateDestructableMap()
    {
        destructablePlacements = mapGen.CreateBlobbyMap(size, 10);
        destructablePlacements = mapGen.SubtractMaps(destructablePlacements, indestructablePlacements);
        GameObject destructable = Instantiate(destructablePrefab, transform.position, Quaternion.identity, gameObject.transform);
        destructable.GetComponent<DestructableTileMap>().Initialize(destructablePlacements, tileReg.destructableTiles[destructIndex], size);
        return destructable;
    }

    List<GameObject> CreateEnemies()
    {
        List<Vector3Int> blankCopy = blankPlacements;
        List<GameObject> tempEnemies = new List<GameObject>();
        enemyPlacements = new List<Vector3Int>();
        for (int i = 0; i < NUM_ENEMIES; i++)
        {
            int positionIndex = Random.Range(0, blankCopy.Count);
            int enemyIndex = Random.Range(0, enemyReg.enemies.Length);
            Vector2 padding = enemyReg.enemies[enemyIndex].GetComponent<Enemy>().startPadding;
            Vector2 startPos = new Vector2(blankCopy[positionIndex].x + padding.x, blankCopy[positionIndex].y + padding.y);
            GameObject enemy = Instantiate(enemyReg.enemies[enemyIndex], startPos, Quaternion.identity, gameObject.transform);
            tempEnemies.Add(enemy);
            enemyPlacements.Add(blankCopy[positionIndex]);
            if (enemy.GetComponent<Enemy>().breakEvents)
            {
                breakSubscribers.Add(enemy);
            }
            blankCopy.RemoveAt(positionIndex);
        }
        return tempEnemies;
    }

    List<GameObject> CreateItems()
    {
        List<Vector3Int> blankCopy = blankPlacements;
        List<GameObject> tempItems = new List<GameObject>();
        for (int i = 0; i < NUM_ITEMS; i++)
        {
            int positionIndex = Random.Range(0, blankCopy.Count);
            int itemIndex = Random.Range(0, itemReg.items.Length);
            Vector2 padding = itemReg.items[itemIndex].GetComponent<WeaponCache>().startPadding;
            Vector2 startPos = new Vector2(blankCopy[positionIndex].x + padding.x, blankCopy[positionIndex].y + padding.y);
            GameObject item = Instantiate(itemReg.items[itemIndex], startPos, Quaternion.identity, gameObject.transform);
            int rarity = Random.Range(0, 4);
            item.GetComponent<WeaponCache>().Init(weaponReg, (Weapon.WeaponRarity)rarity);
            tempItems.Add(item);
            blankCopy.RemoveAt(positionIndex);
        }
        return tempItems;
    }
    */

    #endregion

    #region Events

    public void DamageTile(Sprite damageSprite, Vector3Int position)
    {
        damageMap.GetComponent<DamageTileMap>().CreateDamage(damageSprite, position);
    }

    public void BreakTile(Vector3Int damagePos, Vector3Int accentPos)
    {
        // Remove tile from map
        damageMap.GetComponent<DamageTileMap>().RemoveTile(damagePos);
        accentMap.GetComponent<AccentTileMap>().RemoveAccent(accentPos);

        // Update placement lists
        destructablePlacements.Remove(damagePos);
        if (destructablePlacements.Contains(damagePos - Vector3Int.down) ||
            indestructablePlacements.Contains(damagePos - Vector3Int.down))
        {
            blankPlacements.Add(damagePos);
        }
        if (blankPlacements.Contains(accentPos))
        {
            blankPlacements.Remove(accentPos);
        }

        // Update subscribers
        foreach (GameObject subscriber in breakSubscribers)
        {
            subscriber.GetComponent<Enemy>().TileBreak(damagePos);
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
        if (breakSubscribers.Contains(enemy))
        {
            breakSubscribers.Remove(enemy);
        }
    }

    #endregion

    #region Utilities

    public List<Vector3Int> GetBlanks()
    {
        return blankPlacements.Except(GetEnemyPlacements()).ToList();
    }

    public List<Vector3Int> GetBlocks()
    {
        return indestructablePlacements.Union(destructablePlacements).ToList();
    }

    public List<Vector3Int> GetEnemyPlacements()
    {
        enemyPlacements = new List<Vector3Int>();
        foreach (GameObject enemy in enemies)
        {
            enemyPlacements.Add(enemy.GetComponent<Enemy>().GetPosition());
        }
        return enemyPlacements;
    }

    public bool BlockAt(TilemapType tilemapType, Vector3Int position)
    {
        bool blockFound = false;
        switch (tilemapType)
        {
            case TilemapType.ACCENT:
                blockFound = accentMap.GetComponent<AccentTileMap>().TileExists(position);
                break;
            case TilemapType.DESTRUCTABLE:
                blockFound = destructableMap.GetComponent<DestructableTileMap>().TileExists(position);
                break;
            case TilemapType.INDESTRUCTABLE:
                blockFound = indestructableMap.GetComponent<IndestructTileMap>().TileExists(position);
                break;
            default:
                break;
        }
        return blockFound;
    }

    #endregion
}
