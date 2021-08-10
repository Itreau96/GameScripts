using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    #region Instance Variables

    public GameObject explosionAnim;
    public float explosionRadius;
    public int damage;
    public float thrust;

    #endregion

    // Initial projectile thrust
    void Start()
    {
        gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right * thrust);
    }

    // Called when projectile trigger enters other surface
    private void OnCollisionEnter2D(Collision2D collider)
    {
        // Enable blast radius
        RaycastHit2D[] hitColliders = Physics2D.CircleCastAll(transform.position, explosionRadius, gameObject.GetComponent<Rigidbody2D>().velocity.normalized, 0.1f);
        foreach (RaycastHit2D col in hitColliders)
        {
            if (col.collider.gameObject.tag == "Enemy")
            {
                col.collider.gameObject.GetComponent<Enemy>().Damage(damage);
            }
            else if (col.collider.gameObject.tag == "Destructable")
            {
                Vector2 directionHit = (col.point - (Vector2)transform.position).normalized;
                Vector2 centerPoint = col.point + (directionHit.normalized * 0.5f);
                //col.collider.gameObject.GetComponent<DynamicTileMap>().DamageTiles(centerPoint, (int)explosionRadius);
            }
        }
        // Create collider animation
        Instantiate(explosionAnim, gameObject.transform.position, Quaternion.identity);
        // Destroy self
        Destroy(gameObject);
    }
}