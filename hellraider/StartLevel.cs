using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevel : Level
{
    #region Properties

    public Sprite banner;
    public GameObject entranceAnim;
    public Vector2Int startPosition;

    #endregion

    // Override for initialize function
    public override void Initialize(Cell cell, Vector2Int size, LevelManager levelManager)
    {
        base.Initialize(cell, size, levelManager);
        levelType = LevelType.START;
    }

    // Override for generation function
    public override void Generate()
    {
        // Generate base indestructable tiles
        base.Generate();

        // Create entrance
        CreateEntrance();
    }

    // Create entrance for player
    public void CreateEntrance()
    {
        // Start by getting center 
        Vector2Int archPosition = new Vector2Int(size.x / 2, size.y / 2);
    }
}
