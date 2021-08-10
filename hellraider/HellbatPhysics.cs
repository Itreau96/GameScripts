using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hellbat logic implementation.
/// Provides functionality for decision making and physics logic.
/// </summary>
public class HellbatPhysics : MonoBehaviour
{
    #region Properties

    private const float MOVE_SPEED = 1f;
    private const float FALL_SPEED = 2f;

    private Vector2 movement;
    private bool touching;
    private GameObject player;
    public bool died;
    public bool falling;

    #endregion

    // Set player object on first frame after instantiation.
    void Start()
    {
        // Find the player object and assign it
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Determine movement once per frame.
    void Update()
    {
        DetermineMovement();
    }

    // Used to determine movement vectors for enemy
    private void DetermineMovement()
    {
        if (!died)
        {
            // Determine direction to move towards player
            Vector3 direction = player.transform.position - transform.position;
            direction.Normalize();
            movement = direction;
            // Determine if colliding with player
            touching = gameObject.GetComponent<CircleCollider2D>().IsTouching(player.GetComponent<BoxCollider2D>());
        }
    }
    
    // Update movement every fixed update by game engine.
    private void FixedUpdate()
    {
        UpdateMovement();
    }

    // Update movement of enemy towards player or towards ground (if dead).
    private void UpdateMovement()
    {
        if (!died && !touching)
        {
            // Move enemy by fixed amount each frame
            Vector2 newPos = (Vector2)transform.position + (movement * MOVE_SPEED * Time.fixedDeltaTime);
            GetComponent<Rigidbody2D>().MovePosition(newPos);
        }
        else if (falling)
        {
            // Move enemy by fixed amount towards ground
            Vector2 newPos = new Vector2(transform.position.x, transform.position.y - FALL_SPEED * Time.fixedDeltaTime);
            GetComponent<Rigidbody2D>().MovePosition(newPos);
        }
    }
}