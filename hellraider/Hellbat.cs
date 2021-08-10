using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hellbat enemy interaction implementation.
/// </summary>
public class Hellbat : Enemy
{
    #region Properties

    private bool died;
    private HellbatPhysics physicsObj;
    public GameObject deadObject;
    public AnimationClip deadAnim;
    public LayerMask fallMask;

    #endregion

    // Initialization called by base class.
    protected override void Initialize()
    {
        base.Initialize();
        physicsObj = gameObject.GetComponent<HellbatPhysics>();
    }

    // If damage taken, play animation and apply to health
    public override void Damage(int damage)
    {
        // If not dead...
        if (!died)
        {
            // Apply damage
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

    // Handles playing enemy death animation/falling
    public override void Die()
    {
        base.Die();
        // Set variables and animation
        died = true;
        physicsObj.died = true;
        gameObject.GetComponent<Animator>().SetTrigger("died");
        StartCoroutine(SetFalling());
    }

    // Play falling animation until it collides with the floor
    IEnumerator SetFalling()
    {
        yield return new WaitForSeconds(deadAnim.length);
        // Check if enemy already in contact with floor
        if (GetComponent<CircleCollider2D>().IsTouchingLayers(fallMask))
        {
            Destroy();
        }
        else
        {
            gameObject.GetComponent<Animator>().SetTrigger("falling");
            physicsObj.falling = true;
        }
    }

    // Handle collision with player or objects in scene.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If not dead...
        if (!died)
        {
            // Damage player if other collider
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerController>().Damage();
            }
        }
        else
        {
            // If not a player, (most likely physical obstacle), break once fallen to floor
            if (collision.gameObject.CompareTag("Destructable") || collision.gameObject.CompareTag("Indestructable"))
            {
                Destroy();
            }
        }
    }

    // Remove object from memory and instantiate destruction animation in its place
    private void Destroy()
    {
        Instantiate(deadObject, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}