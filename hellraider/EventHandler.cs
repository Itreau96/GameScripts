using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    // Reference to the game controller
    private GameManager gameManager;

    // Set gamemanager
    private void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
    }

    // Handle touch and keyboard input
    void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        // Determine if jump key pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameManager.JumpClicked();
        }
        // Determine if fire key pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            gameManager.AttackClicked();
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            gameManager.AttackReleased();
        }
        #endif
    }
}