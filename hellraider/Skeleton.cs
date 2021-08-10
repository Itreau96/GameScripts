using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Skeleton enemy interaction implementation.
/// </summary>
public class Skeleton : Enemy
{

    #region Properties

    public AnimationClip deadAnim;
    public GameObject deadObject;
    private SkeletonPhysics physicsObj;
    private bool died;

    #endregion

    // Set padding for proper placement within level
    protected override void Initialize()
    {
        base.Initialize();
        physicsObj = GetComponent<SkeletonPhysics>();
    }

    // Handle collision with trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().Damage();
        }
    }

    // If damage taken, kill enemy
    public override void Damage(int damage)
    {
        // Set died and activate animation
        if (!died)
        {
            died = true;
            physicsObj.died = true;
            gameObject.GetComponent<Animator>().SetTrigger("dead");
            Die();
        }
    }

    // Called when enemy takes maximum damage
    public override void Die()
    {
        base.Die();
        StartCoroutine(DieAfterAnim());
    }

    // Destroys enemy after animation plays
    IEnumerator DieAfterAnim()
    {
        // Wait, create, and destroy
        yield return new WaitForSeconds(deadAnim.length);
        Instantiate(deadObject, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
