using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vampire : Enemy
{
    #region Properties

    private const float WAIT_AFTER_ATTACK = 3f;
    private const float MAX_PLAYER_DISTANCE = 3f;

    public AnimationClip damageAnim;
    public AnimationClip attackAnim;
    public AnimationClip deadAnim;
    public GameObject transformAnim;
    public GameObject deadObject;
    public GameObject projectile;

    private Level currentLevel;
    private VampirePhysics vampirePhysics;
    private Vector2 fireOffset = new Vector2(0.2f, 0f);
    private bool attacking;
    private bool attackCooldown;
    private Coroutine attackCoroutine;
    private bool flipped;
    private bool playerInView;
    private bool died;
    private bool invincible;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        base.Initialize();
        currentLevel = GetComponentInParent<Level>();
        vampirePhysics = GetComponent<VampirePhysics>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAttackState();
    }

    // Updates the enemy's attack state once per frame
    void UpdateAttackState()
    {
        // Attack state is dependent on layer visibility
        if (PlayerVisible() && !invincible)
        {
            // Rotate if not facing player
            if (!flipped && player.transform.position.x < transform.position.x ||
                 flipped && player.transform.position.x > transform.position.x)
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
        if (Vector3.Distance(gameObject.transform.position, player.transform.position) < MAX_PLAYER_DISTANCE)
        {
            return true;
        }
        // Return false otherwise
        return false;
    }

    // Used to change orientation of vampire
    public void ChangeOrientation()
    {
        // Flip transform if necessary
        Vector3 newScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        fireOffset *= -1;
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
            GameObject newProjectile = Instantiate(projectile, transform.position + (Vector3)fireOffset, Quaternion.identity);
            var heading = player.transform.position - (transform.position + (Vector3)fireOffset);
            var direction = heading / Vector3.Distance(player.transform.position, transform.position + (Vector3)fireOffset);
            newProjectile.GetComponent<Projectile>().Initialize(direction);
            yield return new WaitForSeconds(attackAnim.length + WAIT_AFTER_ATTACK);
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(WAIT_AFTER_ATTACK);
        attackCooldown = false;
    }

    // Function used to transform back
    public void TransformBack()
    {
        Instantiate(transformAnim, gameObject.transform.position, Quaternion.identity);
        gameObject.GetComponent<Animator>().SetTrigger("idle");
        invincible = false;
    }

    // Damage and fly, or die
    public override void Damage(int damage)
    {
        // Only apply if active and vulnerable to attack
        if (!died && !invincible)
        {
            health -= damage;
            // Set damage animation
            if (health > 0)
            {
                invincible = true;
                gameObject.GetComponent<Animator>().SetTrigger("damage");
                StartCoroutine(DamageCooldown());
            }
            // Set died boolean and destroy enemy
            else
            {
                Die();
            }
        }
    }

    // Wait for damage animation to complete before transforming
    IEnumerator DamageCooldown()
    {

        yield return new WaitForSeconds(damageAnim.length);
        Transform();
    }

    // Start transformation
    public void Transform()
    {
        AttackOff();
        Instantiate(transformAnim, gameObject.transform.position, Quaternion.identity);
        gameObject.GetComponent<Animator>().SetTrigger("transform");
        List<Vector3Int> blanks = currentLevel.GetBlanks();
        Vector3 newPosition = blanks[0] + (Vector3)startPadding;
        foreach (Vector3Int blank in blanks)
        {
            Vector3 newBlank = blank + (Vector3)startPadding;
            if (Vector3.Distance(newBlank, transform.position) > 1 && Vector3.Distance(newBlank, player.transform.position) < MAX_PLAYER_DISTANCE)
            {
                newPosition = newBlank;
                break;
            }
        }
        vampirePhysics.SetMoving(newPosition);
    }

    // Kill enemy
    public override void Die()
    {
        base.Die();
        // Set variables and animation
        died = true;
        AttackOff();
        vampirePhysics.died = true;
        gameObject.GetComponent<Animator>().SetTrigger("dead");
        StartCoroutine(DieAfterAnim());
    }

    // Destroys enemy after animation plays
    IEnumerator DieAfterAnim()
    {
        // Wait for animation to complete, create destroy animation, and remove from memory
        yield return new WaitForSeconds(deadAnim.length);
        Instantiate(transformAnim, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // Used to handle tile break event
    public override void TileBreak(Vector3Int tilePos)
    {
        base.TileBreak(tilePos);
        if (Vector3Int.Distance(tilePos + Vector3Int.up, GetPosition()) < 0.1)
        {
            Transform();
        }
    }
}