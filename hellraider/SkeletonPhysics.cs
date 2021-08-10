using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Skeleton physics implementation.
/// </summary>
public class SkeletonPhysics : PhysicsObject
{
    #region Properties

    // Constants
    const float X_VISIBILITY = 5f;
    const float MOVE_X = 1f;
    const float Y_VISIBILITY = 2f;

    // Instance variables
    private GameObject player;
    private Animator animator;
    public bool died;

    #endregion

    // Initialization provided by base class.
    protected override void Initialize()
    {
        // Find local objects and assign
        base.Initialize();
        player = GameObject.FindGameObjectWithTag("Player");
        animator = gameObject.GetComponent<Animator>();
    }

    // Compute horizontal velocity of enemy.
    protected override void ComputeVelocity()
    {
        // Only change position if not dead
        if (!died)
        {
            float xVelocity = 0;
            // If player is within viewable range..
            if (PlayerVisible() && Mathf.Abs(player.transform.position.x - transform.position.x) > 0.2)
            {
                // Change x velocity based on position
                if (player.transform.position.x < transform.position.x)
                {
                    xVelocity = -MOVE_X;
                    // If facing right and moving left, flip sprite
                    if (transform.localScale.x < 0)
                    {
                        Vector3 newScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                        transform.localScale = newScale;
                    }
                }
                else
                {
                    xVelocity = MOVE_X;
                    // If facing left and moving right, flip sprite
                    if (transform.localScale.x > 0)
                    {
                        Vector3 newScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                        transform.localScale = newScale;
                    }
                }
            }
            // Update horizontal velocity and animation
            targetVelocity = new Vector2(xVelocity, 0);
            animator.SetFloat("xvelocity", Mathf.Abs(velocity.x));
        }
    }

    // Determine if player is visible
    private bool PlayerVisible()
    {
        // First, ensure player is at appropriate height
        if (player.transform.position.y > (transform.position.y - 0.5) &&
            player.transform.position.y < (transform.position.y + Y_VISIBILITY))
        {
            // Ensure player is within sight distance
            if (player.transform.position.x < (transform.position.x + X_VISIBILITY) &&
                player.transform.position.x > (transform.position.x - X_VISIBILITY))
            {
                return true;
            }
        }

        // Return false otherwise
        return false;
    }
}
