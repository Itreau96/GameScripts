using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.IO;
using System.Resources;

public struct Map
{
    public Cell[,] cells;
    public Vector2Int size;
}

public struct Cell
{
    public bool start;
    public bool visited;
    public Vector2Int location;
    public bool leftWall;
    public bool rightWall;
    public bool topWall;
    public bool bottomWall;
    public Cell(Vector2Int location)
    {
        this.location = location;
        start = false;
        leftWall = true;
        rightWall = true;
        topWall = true;
        bottomWall = true;
        visited = false;
    }
}

public class MazeGenerator : MonoBehaviour
{
    private System.Random rnd;

    public Map CreateMaze(int width, int height)
    {
        Map map = new Map();
        rnd = new System.Random();
        map.size.x = width;
        map.size.y = height;
        map.cells = new Cell[width, height];
        for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                map.cells[x, y] = new Cell(new Vector2Int(x, y));
        Cell start = map.cells[Random.Range(0, width - 1), Random.Range(0, height - 1)];
        start.start = true;
        VisitCell(map, start.location.x, start.location.y);

        return map;
    }

    public void VisitCell(Map map, int x, int y)
    {
        map.cells[x, y].visited = true;
        foreach (var neighbor in GetNeighbors(map, new Vector2Int(x, y)))
        {
            // Final visited call, needed if recursion changes neighbors
            if (!map.cells[neighbor.x, neighbor.y].visited)
            {
                RemoveWalls(map.cells, new Vector2Int(x, y), neighbor);
                VisitCell(map, neighbor.x, neighbor.y);
            }
        }
    }

    public Vector2Int[] GetNeighbors(Map map, Vector2Int position)
    {
        var neighbors = new List<Vector2Int>();
        // Add top if not visited and exists
        if (position.y + 1 < map.size.y && !map.cells[position.x, position.y + 1].visited)
        {
            neighbors.Add(position + Vector2Int.up);
        }
        // Add left if not visited and exists
        if (position.x - 1 >= 0 && !map.cells[position.x - 1, position.y].visited)
        {
            neighbors.Add(position + Vector2Int.left);
        }
        // Add right if not visited and exists
        if (position.x + 1 < map.size.x && !map.cells[position.x + 1, position.y].visited)
        {
            neighbors.Add(position + Vector2Int.right);
        }
        // Add bottom if not visied and exists
        if (position.y - 1 >= 0 && !map.cells[position.x, position.y - 1].visited)
        {
            neighbors.Add(position + Vector2Int.down);
        }
        return neighbors.OrderBy(c => rnd.Next()).ToArray();
    }

    public void RemoveWalls(Cell[,] cells, Vector2Int first, Vector2Int second)
    {
        // Break between left and right
        if (second == first + Vector2Int.left)
        {
            cells[first.x, first.y].leftWall = false;
            cells[second.x, second.y].rightWall = false;
        }
        // Break between right and left
        else if (second == first + Vector2Int.right)
        {
            cells[first.x, first.y].rightWall = false;
            cells[second.x, second.y].leftWall = false;
        }
        // Break between top and bottom
        else if (second == first + Vector2Int.up)
        {
            cells[first.x, first.y].bottomWall = false; // Remember, flipped
            cells[second.x, second.y].topWall = false;
        }
        // Break between bottom and top
        else if (second == first + Vector2Int.down)
        {
            cells[first.x, first.y].topWall = false; // Remember, flipped
            cells[second.x, second.y].bottomWall = false;
        }
    }

    /*
    public void Display()
    {
        string path = "testEx.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);

        var firstLine = string.Empty;
        for (var y = 0; y < height; y++)
        {
            var sbTop = new StringBuilder();
            var sbMid = new StringBuilder();
            for (var x = 0; x < width; x++)
            {
                sbTop.Append(cells[x, y].topWall ? "+--" : "+  ");
                sbMid.Append(cells[x, y].leftWall ? "|  " : "   ");
            }
            if (firstLine == string.Empty)
                firstLine = sbTop.ToString();
            writer.WriteLine(sbTop + "+");
            writer.WriteLine(sbMid + "|");
        }
        writer.WriteLine(firstLine + "+");
        writer.Close();
    }
    */
}
