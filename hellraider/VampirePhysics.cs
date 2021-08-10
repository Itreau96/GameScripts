using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampirePhysics : MonoBehaviour
{
    #region Properties

    private const float MOVE_SPEED = 3f;
    private const float MIN_TRANSFORM_DIST = 0.05f;

    public bool moving;
    public bool died;

    private Vampire vampire;
    private Vector2 movement;
    private Vector2 newPos;

    #endregion

    // Setup physics object
    void Start()
    {
        vampire = GetComponent<Vampire>();
    }

    // Determine movement once per frame.
    void Update()
    {
        DetermineMovement();
    }

    // Switch to move toward new blank space
    public void SetMoving(Vector2 newPos)
    {
        this.newPos = newPos;
        moving = true;
    }

    // Used to determine movement vectors for enemy
    private void DetermineMovement()
    {
        if (!died && moving)
        {
            // Determine direction to move towards player
            Vector3 direction = newPos - (Vector2)transform.position;
            direction.Normalize();
            movement = direction;
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
        if (!died && moving)
        {
            // Move enemy by fixed amount each frame
            Vector2 movementPos = (Vector2)transform.position + (movement * MOVE_SPEED * Time.fixedDeltaTime);
            GetComponent<Rigidbody2D>().MovePosition(movementPos);
            if (Vector2.Distance(newPos, transform.position) < MIN_TRANSFORM_DIST)
            {
                moving = false;
                vampire.TransformBack();
            }
        }
    }
}
