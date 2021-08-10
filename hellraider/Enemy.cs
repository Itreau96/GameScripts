using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class used to define common behavior for in-game enemies.
/// </summary>
public abstract class Enemy : MonoBehaviour
{
    #region Properties

    protected GameObject player;
    public int health;
    public GameObject soul;
    public Vector2 startPadding = Vector2.zero;
    public bool breakEvents;

    #endregion

    // Object initialization function provided by MonoBehaviour.
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Initialize();
    }

    // Virtual enemy initialization. Implemented in subclasses. Optional.
    protected virtual void Initialize() { /* No default initialization at present. */ }

    // Abstract function for receiving damage. Implemented in subclasses.
    public abstract void Damage(int damage);

    // Abstract function for retrieving current position relative to tile space
    public virtual Vector3Int GetPosition()
    {
        Vector3Int normalizedPos = Vector3Int.RoundToInt(transform.position - (Vector3)startPadding);
        return normalizedPos;
    }

    // Base method for destroying enemy and removing from level
    public virtual void Die()
    {
        // Instantiate soul after destroy
        Instantiate(soul, transform.position, Quaternion.identity);
        GetComponentInParent<Level>().RemoveEnemy(gameObject);
    }

    #region Events

    // Function for reacting to tile break events
    public virtual void TileBreak(Vector3Int tilePos) { /* Implemented in child */ }

    #endregion
}