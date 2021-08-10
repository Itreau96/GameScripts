using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Weapon itself contains arm + weapon. Display image used to just display
public abstract class Weapon : MonoBehaviour
{
    #region Instance Variables

    // General use variables by weapons
    public Sprite displayImage;
    public string displayName;
    public string description;
    public WeaponRarity rarity;
    public int cost;
    public int damage;
    [HideInInspector]
    public abstract WeaponType Type { get; }
    public GameObject hitAnim;
    public bool flipped;
    public LayerMask collisionMask;

    #endregion

    // Enum used to define weapon being fired
    public enum WeaponType
    {
        MELEE, GUN, PROJECTILE, SPECIAL
    }

    // Enum used to define weapon rarity
    public enum WeaponRarity
    {
        BORING, ADEQUATE, AWESOME, BADASS
    }

    // Method used to attack using weapon
    public abstract void Attack();
    public abstract void AttackRelease();
}
