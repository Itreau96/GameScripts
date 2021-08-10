using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaziPhysics : PhysicsObject
{
    #region Properties

    // Constants
    const float MOVE_X = 1f;

    // Instance variables
    public bool died;
    public bool flipped;
    public bool moving;
    public bool falling;
    private Animator animator;

    #endregion

    // Initialization provided by base class.
    protected override void Initialize()
    {
        // Find local objects and assign
        base.Initialize();
        animator = gameObject.GetComponent<Animator>();
    }

    // Compute horizontal velocity of enemy.
    protected override void ComputeVelocity()
    {
        // Only change position if not dead
        if (!died)
        {
            float xVelocity = 0;
            falling = !grounded;
            // If player is within viewable range..
            if (moving)
            {
                // Change x velocity based on direction
                if (flipped)
                {
                    xVelocity = -MOVE_X;
                }
                else
                {
                    xVelocity = MOVE_X;
                }
            }
            // Update horizontal velocity and animation
            targetVelocity = new Vector2(xVelocity, 0);
            animator.SetFloat("xVelocity", Mathf.Abs(velocity.x));
        }
    }
}
