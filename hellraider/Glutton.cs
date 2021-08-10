using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Glutton enemy interaction implementation.
/// Character is immobile and does not require a physics implementation.
/// </summary>
public class Glutton : Enemy
{
    #region Properties

    const float WAIT_AFTER_ATTACK = 3f;
    const float X_VISIBILITY = 5f;
    const float Y_VISIBILITY = 1f;

    private bool died;
    private Coroutine attackCoroutine;
    private bool attacking;
    private bool attackCooldown;
    private Vector2Int fireDirection = new Vector2Int(-1, 0);
    private bool flipped;
    private bool playerInView;

    public GameObject deadObject;
    public AnimationClip deadAnim;
    public AnimationClip attackAnim;
    public GameObject projectile;

    #endregion

    // Update is called once per frame
    void Update()
    {
        UpdateAttackState();
    }

    // Updates the enemy's attack state once per frame
    void UpdateAttackState()
    {
        // Attack state is dependent on layer visibility
        if (PlayerVisible())
        {
            // Rotate if not facing player
            if (!flipped && player.transform.position.x > transform.position.x ||
                 flipped && player.transform.position.x < transform.position.x)
            {
                flipped = !flipped;
                ChangeOrientation();
            }
            // Update attack flag
            if (!playerInView && !attackCooldown)
            {
                playerInView = true;
                AttackOn();
            }
        }
        else
        {
            // Update attack flag
            if (playerInView)
            {
                playerInView = false;
                AttackOff();
            }
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

    // Used to change orientation of glutton
    public void ChangeOrientation()
    {
        // Flip transform if necessary
        Vector3 newScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        fireDirection *= -1;
        transform.localScale = newScale;
    }

    // Initiate attack sequence
    public void AttackOn()
    {
        if (!attacking)
        {
            attacking = true;
            attackCoroutine = StartCoroutine(AttackAndDelay());
        }
    }

    // Halt attack sequence
    public void AttackOff()
    {
        if (attacking)
        {
            attacking = false;
            attackCooldown = true;
            StopCoroutine(attackCoroutine);
            StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator AttackAndDelay()
    {
        while (attacking)
        {
            // Set state/anim
            gameObject.GetComponent<Animator>().SetTrigger("attack");
            // Initialize projectile
            GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
            newProjectile.GetComponent<Projectile>().Initialize(fireDirection);
            yield return new WaitForSeconds(attackAnim.length + WAIT_AFTER_ATTACK);
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(WAIT_AFTER_ATTACK);
        attackCooldown = false;
    }

    // If damage taken, kill enemy
    public override void Damage(int damage)
    {
        // Only apply if active and vulnerable to attack
        if (!died)
        {
            health -= damage;
            // Set damage animation
            if (health > 0)
            {
                gameObject.GetComponent<Animator>().SetTrigger("damage");
            }
            // Set died boolean and destroy enemy
            else
            {
                AttackOff();
                Die();
            }
        }
    }

    // Kill enemy
    public override void Die()
    {
        base.Die();
        // Set variables and animation
        died = true;
        gameObject.GetComponent<Animator>().SetTrigger("died");
        StartCoroutine(DieAfterAnim());
    }

    // Destroys enemy after animation plays
    IEnumerator DieAfterAnim()
    {
        // Wait for animation to complete, create destroy animation, and remove from memory
        yield return new WaitForSeconds(deadAnim.length);
        Instantiate(deadObject, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}