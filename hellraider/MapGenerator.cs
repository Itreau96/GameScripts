using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

/// <summary>
/// Map Generation utilities.
/// While most map generators make use of matrices, this utility script
/// uses tuple values (Vector3Int) to minimize search time and memory usage.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    // Get empty spaces to place items in map
    public List<Vector3Int> BlankSpaces(List<Vector3Int> blocks, Vector2Int size)
    {
        // Get blanks
        List<Vector3Int> blanks = new List<Vector3Int>();
        foreach (Vector3Int block in blocks)
        {
            Vector3Int newBlank = block + Vector3Int.up;
            if (block.y < size.y - 1 && !blocks.Contains(newBlank))
            {
                blanks.Add(newBlank);
            }
        }
        return blanks;
    }

    // Returns a procedural 2 dimmensional blobby map for terrain.
    public List<Vector3Int> CreateBlobbyMap(Vector2Int size, float scale)
    {
        List<Vector3Int> map = new List<Vector3Int>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                float xCoord = (float)x / size.x * scale;
                float yCoord = (float)y / size.y * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                if (sample > 0.5) map.Add(new Vector3Int(x, y, 0));
            }
        }
        // Return complete map
        return map;
    }

    // Creates a surrounding base tiles 
    public (List<Vector3Int>, List<Vector3Int>) CreateBaseTiles(Vector2Int size, Cell cell)
    {
        List<Vector3Int> unbreakablePlacements = new List<Vector3Int>();
        List<Vector3Int> breakablePlacements = new List<Vector3Int>();

        // Generate top and bottom walls
        for (int x = 0; x < size.x; x++)
        {
            if (cell.topWall)
            {
                unbreakablePlacements.Add(new Vector3Int(x, 0, 0));
                if ((!cell.leftWall && !cell.rightWall) || 
                    (cell.leftWall && !cell.rightWall && x != 0) ||
                    (!cell.leftWall && cell.rightWall && x != size.x - 1) ||
                    (cell.leftWall && cell.rightWall && x > 0 && x < size.x - 1))
                {
                    breakablePlacements.Add(new Vector3Int(x, 1, 0));
                }
            }
            if (cell.bottomWall)
            {
                unbreakablePlacements.Add(new Vector3Int(x, size.y - 1, 0));
                if ((!cell.leftWall && !cell.rightWall) ||
                    (cell.leftWall && !cell.rightWall && x != 0) ||
                    (!cell.leftWall && cell.rightWall && x != size.x - 1) ||
                    (cell.leftWall && cell.rightWall && x > 0 && x < size.x - 1))
                {
                    breakablePlacements.Add(new Vector3Int(x, size.y - 2, 0));
                }
            }
        }
        // Generate left and right walls
        for (int y = 0; y < size.y; y++)
        {
            if (cell.leftWall)
            {
                unbreakablePlacements.Add(new Vector3Int(0, y, 0));
                if ((!cell.topWall && !cell.bottomWall) ||
                    (cell.topWall && !cell.bottomWall && y != 0) ||
                    (!cell.topWall && cell.bottomWall && y != size.y - 1) ||
                    (cell.topWall && cell.bottomWall && y > 0 && y < size.y - 1))
                {
                    breakablePlacements.Add(new Vector3Int(1, y, 0));
                }
            }
            if (cell.rightWall)
            {
                unbreakablePlacements.Add(new Vector3Int(size.x - 1, y, 0));
                if ((!cell.topWall && !cell.bottomWall) ||
                    (cell.topWall && !cell.bottomWall && y != 0) ||
                    (!cell.topWall && cell.bottomWall && y != size.y - 1) ||
                    (cell.topWall && cell.bottomWall && y > 0 && y < size.y - 1))
                {
                    breakablePlacements.Add(new Vector3Int(size.x - 2, y, 0));
                }
            }
        }

        // Add missing corners
        if (!cell.bottomWall && !cell.rightWall)
        {
            unbreakablePlacements.Add(new Vector3Int(size.x - 1, size.y - 1, 0));
            breakablePlacements.Add(new Vector3Int(size.x - 2, size.y - 1, 0));
            breakablePlacements.Add(new Vector3Int(size.x - 2, size.y - 2, 0));
            breakablePlacements.Add(new Vector3Int(size.x - 1, size.y - 2, 0));
        }
        if (!cell.bottomWall && !cell.leftWall)
        {
            unbreakablePlacements.Add(new Vector3Int(0, size.y - 1, 0));
            breakablePlacements.Add(new Vector3Int(1, size.y - 1, 0));
            breakablePlacements.Add(new Vector3Int(1, size.y - 2, 0));
            breakablePlacements.Add(new Vector3Int(0, size.y - 2, 0));
        }
        if (!cell.topWall && !cell.leftWall)
        {
            unbreakablePlacements.Add(new Vector3Int(0, 0, 0));
            breakablePlacements.Add(new Vector3Int(1, 0, 0));
            breakablePlacements.Add(new Vector3Int(1, 1, 0));
            breakablePlacements.Add(new Vector3Int(0, 1, 0));
        }
        if (!cell.topWall && !cell.rightWall)
        {
            unbreakablePlacements.Add(new Vector3Int(size.x - 1, 0, 0));
            breakablePlacements.Add(new Vector3Int(size.x - 2, 0, 0));
            breakablePlacements.Add(new Vector3Int(size.x - 2, 1, 0));
            breakablePlacements.Add(new Vector3Int(size.x - 1, 1, 0));
        }

        // Return complete map
        return (unbreakablePlacements, breakablePlacements);
    }

    // Creates parallel platform map given size
    public List<Vector3Int> CreateParallelMap(Vector2Int position, Vector2Int size)
    {
        var placements = new List<Vector3Int>();
        SplitMap(placements, position, size);
        return placements;
    }

    // Recursive function for splitting map
    public void SplitMap(List<Vector3Int> placements, Vector2Int position, Vector2Int size)
    {
        // Base case return (too small or variable chunk size)
        int makeBlock = UnityEngine.Random.Range(0, 3);
        if (size.x / 2 <= 3 || size.y / 2 <= 3 || makeBlock == 2)
        {
            // Populate size 
            // TODO: Replace with complex block generation function based on size
            for (var x = position.x + 1; x < position.x + size.x - 1; x++)
            {
                for (var y = position.y + 1; y < position.y + size.y - 1; y++)
                {
                    placements.Add(new Vector3Int(x, y, 0));
                }
            }
        }
        // Split and recurse
        else
        {
            // Split horizontally or diagonally
            int choice = UnityEngine.Random.Range(0, 2);
            // If horizonally
            if (choice == 0)
            {
                SplitMap(placements, position, new Vector2Int(size.x, size.y / 2));
                SplitMap(placements, new Vector2Int(position.x, position.y + size.y / 2), new Vector2Int(size.x, size.y / 2));
            }
            // If vertically
            else
            {
                SplitMap(placements, position, new Vector2Int(size.x / 2, size.y));
                SplitMap(placements, new Vector2Int(position.x + size.x / 2, position.y), new Vector2Int(size.x / 2, size.y));
            }
        }
    }

    // Helper method for joining two maps
    public List<Vector3Int> OverlayMaps(List<Vector3Int> map, List<Vector3Int> overlay)
    {
        return map.Union(overlay).ToList();
    }

    // Helper method for subtracting two maps
    public List<Vector3Int> SubtractMaps(List<Vector3Int> map, List<Vector3Int> subtraction)
    {
        return map.Except(subtraction).ToList();
    }

    // Helper method for probabilistically placing items in map
    public List<Vector3Int> RandomPlacement(List<Vector3Int> placements, int probability)
    {
        List<Vector3Int> newPlacements = new List<Vector3Int>();
        foreach (Vector3Int placement in placements)
        {
            int prob = UnityEngine.Random.Range(0, probability);
            if (prob == 0)
            {
                newPlacements.Add(placement);
            }
        }
        return newPlacements;
    }
}
