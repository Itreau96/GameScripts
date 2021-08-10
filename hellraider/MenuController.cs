using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    // Game controller script
    public GameController gameController;

    // Load player data
    void Start() { }

    // Continue current game
    public void GameClicked()
    {
        // Load gameplay scene
        gameController.LoadScene("Game");
    }

    // Load credits scene
    public void CreditsClicked()
    {
        // Load credits scene
        gameController.LoadScene("Credits");
    }
}
