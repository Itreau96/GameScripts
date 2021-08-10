using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    #region Instance Variables

    public float distance;
    public float numTargets;
    public float sprayRadius;
    public float scatter;
    public GameObject projectile;
    public Vector2 firePoint;
    public float automaticFireRate;
    public bool explosive;
    public GameObject flare;

    // Automatic weapon fire variables
    private bool firing;
    private float lastFired;

    // Return weapontype
    public override WeaponType Type
    {
        get { return WeaponType.GUN; }
    }

    #endregion

    // Define fire state
    public override void Attack()
    {
        // First, determine if automatic fire
        if (automaticFireRate != 0)
        {
            firing = true;
        }
        else
        {
            // Fire weapon
            Fire();
        }
    }

    // Function used to initiate a single fire of the gun
    private void Fire()
    {
        // Fire animation
        gameObject.GetComponent<Animator>().SetTrigger("fire");

        // First, check if projectile
        if (projectile)
        {
            // Instantiate projectile
            Vector2 firePosition = new Vector2(gameObject.transform.position.x + firePoint.x, gameObject.transform.position.y + firePoint.y);
            Instantiate(projectile, firePosition, Quaternion.identity);
        }
        else
        {
            // Spray multiple shots in radius provided
            if (sprayRadius > 0)
            {
                // Create based on direction of player
                if (flipped)
                {
                    CreateRaycast(GetDirectionVector2D(180 + sprayRadius), distance / 2);
                    CreateRaycast(Vector2.left, distance);
                    CreateRaycast(GetDirectionVector2D(180 - sprayRadius), distance / 2);
                }
                else
                {
                    CreateRaycast(GetDirectionVector2D(sprayRadius), distance);
                    CreateRaycast(Vector2.right, distance);
                    CreateRaycast(GetDirectionVector2D(-sprayRadius), distance);
                }
            }
            // Randomly scatter shot
            else if (scatter > 0)
            {
                float randomAngle;
                if (flipped)
                {
                    randomAngle = Random.Range(180-scatter, 180 + scatter);
                }
                else
                {
                    randomAngle = Random.Range(-scatter, scatter);
                }
                CreateRaycast(GetDirectionVector2D(randomAngle), distance);
            }
            // Standard single shot
            else
            {
                // Create based on direction of player
                if (flipped) CreateRaycast(Vector2.left, distance);
                else CreateRaycast(Vector2.right, distance);
            }
        }
    }

    // Event handler for touch release (only if automatic)
    public override void AttackRelease()
    {
        // Set firing to false
        firing = false;
    }
    

    // Helper method used to handle angle 
    public Vector2 GetDirectionVector2D(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
    }

    // Helper method used to generate a hit
    public void CreateRaycast(Vector2 direction, float distance)
    {
        // Send raycast
        Vector2 firePosition = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + firePoint.y); // Do not include gun x position.
        RaycastHit2D[] hits = Physics2D.RaycastAll(firePosition, direction, distance, collisionMask);

        // First determine hit objects
        for (int i = 0; i < hits.Length; i++)
        {
            if (i >= numTargets)
            {
                break;
            }
            else
            {
                DamageObject(hits[i]);
                CreateFlare(transform.TransformPoint(firePoint), hits[i].point);
            }
        }
        // If no hit objects, create full length flare
        if (hits.Length == 0)
        {
            Vector2 endPos = (Vector2)transform.TransformPoint(firePoint) + direction * distance;
            CreateFlare(transform.TransformPoint(firePoint), endPos);
        }
    }

    // Helper method used to create flare indicator
    void CreateFlare(Vector2 startPos, Vector2 endPos)
    {
        // Create flare
        Vector2 startPoint = (startPos + endPos) / 2f;
        GameObject flareInstance = Instantiate(flare, startPoint, Quaternion.identity);
        float worldWidth = flareInstance.GetComponent<SpriteRenderer>().bounds.size.x;
        float desiredWidth = Vector2.Distance(startPos, endPos);
        flareInstance.transform.localScale = new Vector3(flareInstance.transform.localScale.x / worldWidth * desiredWidth,
                                                         flareInstance.transform.localScale.y,
                                                         flareInstance.transform.localScale.z);
        //float angle = Vector2.SignedAngle(dir, endPos - startPos);
        float angle = Mathf.Atan2(endPos.y - startPos.y, endPos.x - startPos.x) * Mathf.Rad2Deg;
        flareInstance.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Deal damage depending on object hit
    private void DamageObject(RaycastHit2D hit)
    {
        switch (hit.collider.gameObject.tag)
        {
            case "Enemy":
                hit.collider.gameObject.GetComponent<Enemy>().Damage(damage);
                break;
            case "Destructable":
                Vector2 directionHit = (hit.point - (Vector2)transform.position).normalized;
                Vector2 centerPoint = hit.point + (directionHit.normalized * 0.5f);
                hit.collider.gameObject.GetComponent<DestructableTileMap>().DamageTile(centerPoint, explosive);
                break;
            default:
                break;
        }
        // Regardless of hit, apply hit animation
        Instantiate(hitAnim, hit.point, Quaternion.identity);
    }

    // Utilize the frame update function for automatic fire rates
    void Update()
    {
        // Only fire weapon if shots fired
        if (firing)
        {
            // Only fire if proper time elapsed
            if (Time.time - lastFired > 1 / automaticFireRate)
            {
                lastFired = Time.time;

                // Fire single shot
                Fire();
            }
        }
    }
}
