using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy character interaction script.
/// </summary>
public class Nazi : Enemy
{
    #region Properties

    const float WAIT_AFTER_ATTACK = 1f;
    const float WAIT_AFTER_FLIP = 2f;
    const float X_VISIBILITY = 5f;
    const float Y_VISIBILITY = 1f;

    private bool died;
    private bool attacking;
    private bool turning;
    private bool falling;
    private bool attackCooldown;
    private bool playerInView;
    private NaziPhysics naziPhysics;
    private Coroutine enemyLoop;
    private Level currentLevel;

    public AnimationClip deadAnim;
    public AnimationClip attackAnim;
    public GameObject deadObject;
    public Vector2 firePoint;
    public GameObject hitAnim;
    public int damage;
    public float fireDistance;
    public GameObject projectile;

    #endregion

    // Initialization call
    protected override void Initialize()
    {
        base.Initialize();
        naziPhysics = GetComponent<NaziPhysics>();
        currentLevel = GetComponentInParent<Level>();
        // Variable for starting and stopping manually (fixes race conditions if coroutine re-called)
        enemyLoop = StartCoroutine(EnemyLoop());
    }


    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    // Updates the enemy's attack state once per frame
    void UpdateState()
    {
        // First check if falling
        if (naziPhysics.falling)
        {
            if (!falling)
            {
                falling = true;
                gameObject.GetComponent<Animator>().SetTrigger("falling");
                StopCoroutine(enemyLoop);
                enemyLoop = StartCoroutine(EnemyLoop());
            }
        }
        else
        {
            // Reset falling variable
            if (falling)
            {
                falling = false;
                gameObject.GetComponent<Animator>().SetTrigger("idle");
            }
            // First determine if enemy in sight
            if (PlayerVisible())
            {
                // First, stop if turning
                if (turning)
                {
                    turning = false;
                    StopCoroutine(enemyLoop);
                    enemyLoop = StartCoroutine(EnemyLoop());
                }
                else if (naziPhysics.moving)
                {
                    naziPhysics.moving = false;
                    StopCoroutine(enemyLoop);
                    enemyLoop = StartCoroutine(EnemyLoop());
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

                if (!turning)
                {
                    if (FrontClear())
                    {
                        if (!naziPhysics.moving)
                        {
                            naziPhysics.moving = true;
                            StopCoroutine(enemyLoop);
                            enemyLoop = StartCoroutine(EnemyLoop());
                        }
                    }
                    else
                    {
                        naziPhysics.moving = false;
                        turning = true;
                        StopCoroutine(enemyLoop);
                        enemyLoop = StartCoroutine(EnemyLoop());
                    }
                }
            }
        }
    }

    // Determine if player is visible
    private bool PlayerVisible()
    {
        // Return based on player direction
        if (naziPhysics.flipped)
        {
            // First, ensure player is at appropriate height
            if (player.transform.position.y > (transform.position.y - 0.5) &&
                player.transform.position.y < (transform.position.y + Y_VISIBILITY))
            {
                // Ensure player is within sight distance
                if (player.transform.position.x - transform.position.x < 0 &&
                    player.transform.position.x > (transform.position.x - X_VISIBILITY))
                {
                    return true;
                }
            }
        }
        else
        {
            // First, ensure player is at appropriate height
            if (player.transform.position.y > (transform.position.y - 0.5) &&
                player.transform.position.y < (transform.position.y + Y_VISIBILITY))
            {
                // Ensure player is within sight distance
                if (player.transform.position.x - transform.position.x > 0 &&
                    player.transform.position.x < (transform.position.x + X_VISIBILITY))
                {
                    return true;
                }
            }
        }

        // Return false otherwise
        return false;
    }

    // Used to change orientation of nazi
    public void ChangeOrientation()
    {
        // Flip transform if necessary
        Vector3 newScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        transform.localScale = newScale;
        naziPhysics.flipped = !naziPhysics.flipped;
        firePoint.x *= -1;
    }

    // Initiate attack sequence
    public void AttackOn()
    {
        if (!attacking)
        {
            attacking = true;
            StopCoroutine(enemyLoop);
            enemyLoop = StartCoroutine(EnemyLoop());
        }
    }

    // Halt attack sequence
    public void AttackOff()
    {
        if (attacking)
        {
            attacking = false;
            attackCooldown = true;
            StopCoroutine(enemyLoop);
            enemyLoop = StartCoroutine(EnemyLoop());
        }
    }

    bool FrontClear()
    {
        Vector3Int below = GetPosition() + Vector3Int.right + Vector3Int.down;
        Vector3Int front = GetPosition() + Vector3Int.right;
        List<Vector3Int> blocks = currentLevel.GetBlocks();
        if (naziPhysics.flipped)
        {
            below = GetPosition() + Vector3Int.left + Vector3Int.down;
            front = GetPosition() + Vector3Int.left;
        }

        if (blocks.Contains(below) && !blocks.Contains(front))
        {
            return true;
        }

        return false;
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
        StopCoroutine(enemyLoop);
        enemyLoop = StartCoroutine(EnemyLoop());
    }

    IEnumerator EnemyLoop()
    {
        while(true)
        {
            if (died)
            {
                // Wait for animation to complete, create destroy animation, and remove from memory
                gameObject.GetComponent<Animator>().SetTrigger("died");
                yield return new WaitForSeconds(deadAnim.length);
                Instantiate(deadObject, gameObject.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else
            {
                if (turning)
                {
                    yield return new WaitForSeconds(WAIT_AFTER_FLIP);
                    ChangeOrientation();
                    turning = false;
                }
                else if (attacking)
                {
                    // Set state/anim
                    gameObject.GetComponent<Animator>().SetTrigger("attack");
                    // Initialize projectile
                    GameObject newProjectile = Instantiate(projectile, transform.position + (Vector3)firePoint, Quaternion.identity);
                    var heading = player.transform.position - (transform.position + (Vector3)firePoint);
                    var direction = heading / Vector3.Distance(player.transform.position, transform.position + (Vector3)firePoint);
                    newProjectile.GetComponent<Projectile>().Initialize(direction);
                    yield return new WaitForSeconds(attackAnim.length + WAIT_AFTER_ATTACK);
                }
                else if (attackCooldown)
                {
                    yield return new WaitForSeconds(WAIT_AFTER_ATTACK);
                    attackCooldown = false;
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
}