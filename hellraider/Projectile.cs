using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Properties

    public int lifetime;
    public Vector2 velocity;
    public GameObject collisionObject;

    private Vector2 direction;
    private bool moving;

    #endregion

    // Instantiate object with direction
    public void Initialize(Vector2 direction)
    {
        this.direction = direction;
        moving = true;
        StartCoroutine(DestroyAfterTime());
    }

    // Called every fixed-framerate frame. Used to apply velocity computed by subclass.
    void FixedUpdate()
    {
        // Initialize velocity
        if (moving)
        {
            gameObject.transform.Translate(direction * velocity * Time.fixedDeltaTime);
        }
    }

    // Handle collision with trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().Damage();
        }
        Instantiate(collisionObject, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // Destroy object after lifetime is reached
    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        Instantiate(collisionObject, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
