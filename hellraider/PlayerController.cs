using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles player animation and collision (Touch controls handled by game manager to avoid multi-touch)
public class PlayerController : PhysicsObject
{
    #region Constants

    // Physics constants
    const float MAX_MOVEMENT = 2f;
    const float RUN_VAL = 1f;
    const float FALL_VAL = 2f;
    const float JUMP_VELOCITY = 15f;
    const float CLIMB_VELOCITY = 5f;

    #endregion

    #region Instance Variables

    // Testing stuff
    public bool godMode;

    // Physics stuff
    public float maxSpeed = 7f;
    public float movementMod = 0.0f;
    public AnimationClip damageAnim;
    public int health = 3;
    public bool died;
    public int xVelocity;
    public bool flipped;
    public PlayerData playerData; // Existing player data (loaded from singleton gameobject)
    public GameObject spriteBody; // Reference to animation body
    public PlayerState state = PlayerState.RUNNING;
    public Vector2 weaponAnchor;
    bool jump;
    bool canDamage = true;
    bool moving;

    // Current equipped weapon
    public GameObject weapon;

    #endregion

    #region Enum

    public enum PlayerState
    {
        IDLE, RUNNING, FALLING, CLIMBING, RISING
    }

    #endregion

    protected override void ComputeVelocity()
    {
        if (!died)
        {
            if (walled)
            {
                velocity.y = CLIMB_VELOCITY;
            }
            else if (jump)
            {
                velocity.y = JUMP_VELOCITY;
                jump = false;
            }

            targetVelocity = new Vector2(xVelocity, 0);
            UpdateAnimState();
        }
    }

    void UpdateAnimState()
    {
        // First, ensure player hasn't died
        if (!died)
        {
            // Set landing animation
            if (velocity.y > 0 && !walled && state != PlayerState.RISING)
            {
                gameObject.GetComponent<Animator>().SetTrigger("rise");
                state = PlayerState.RISING;
            }
            // Set running animation
            else if (grounded && Mathf.Abs(velocity.x) > 0.1 && state != PlayerState.RUNNING)
            {
                // Animation will transition to landing at this point. ONLY animate if not possible transition.
                if (state != PlayerState.FALLING)
                {
                    gameObject.GetComponent<Animator>().SetTrigger("run");
                }
                state = PlayerState.RUNNING;
            }
            // Set climbing animation
            else if (walled && velocity.y > 0.1 && Mathf.Abs(xVelocity) > 0.01 && state != PlayerState.CLIMBING)
            {
                gameObject.GetComponent<Animator>().SetTrigger("climb");
                state = PlayerState.CLIMBING;
            }
            // Set falling animation
            else if (velocity.y < 0 && state != PlayerState.FALLING)
            {
                gameObject.GetComponent<Animator>().SetTrigger("fall");
                state = PlayerState.FALLING;
            }
            // Set rising animation
            else if (grounded && Mathf.Abs(velocity.x) < 0.1 && state != PlayerState.IDLE)
            {

                // Animation will transition to landing at this point. ONLY animate if not possible transition.
                if (state != PlayerState.FALLING)
                {
                    gameObject.GetComponent<Animator>().SetTrigger("idle");
                }
                state = PlayerState.IDLE;
            }
        }
        // Update animation values
        gameObject.GetComponent<Animator>().SetBool("grounded", grounded);
        gameObject.GetComponent<Animator>().SetFloat("xVelocity", Mathf.Abs(velocity.x));
    }

    // Jump event handler
    public void Jump()
    {
        if (!died && state != PlayerState.FALLING && state != PlayerState.RISING && state != PlayerState.CLIMBING)
        {
            state = PlayerState.RISING;
            gameObject.GetComponent<Animator>().SetTrigger("jump");
            jump = true;
        }
    }

    // Handle moving left
    public void SetXMovement(int x)
    {
        // Only change velocity if player isn't dead
        if (!died)
        {
            xVelocity = x; // Set rarget x velocity from touch control
            if (x < 0 && !flipped || x > 0 && flipped)
            {
                Vector3 newScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                transform.localScale = newScale;
                flipped = !flipped;
                weapon.GetComponent<Weapon>().flipped = flipped;
            }
        }
    }

    // Equip a weapon based on the weapon type
    public void Equip(GameObject weapon)
    {
        // If current weapon, destroy
        if (this.weapon)
        {
            Destroy(this.weapon);
        }

        // Instantiate new weapon
        this.weapon = Instantiate(weapon, transform.position, Quaternion.identity, gameObject.transform);
        this.weapon.transform.localPosition = weaponAnchor;
        this.weapon.GetComponent<Weapon>().flipped = flipped;
    }

    // Removes health from player
    public void Damage()
    {
        // First check if player dead or immune
        if (!died && canDamage && !godMode)
        {
            // Remove 1 health bar. If 0, died = true
            health -= 1;
            if (health == 0)
            {
                died = true;
                gameObject.GetComponent<Animator>().SetTrigger("dead");
            }
            else
            {
                gameObject.GetComponent<Animator>().SetTrigger("damage");
                StartCoroutine(DamageDelay());
            }
        }
    }

    // Wait specified amount of time before allowing damage
    private IEnumerator DamageDelay()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageAnim.length);
        // First determine if still touching enemies
        if (GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Flying", "Enemy")))
        {
            canDamage = true;
            Damage();
        }
        else
        {
            canDamage = true;
        }
    }

    // Set game over
    public void GameOver()
    {

    }
}
