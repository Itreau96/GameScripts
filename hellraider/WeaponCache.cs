using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCache : MonoBehaviour
{
    #region Class Variables

    public GameObject weapon;
    public Vector2 startPadding;

    Weapon.WeaponRarity rarity;
    bool empty;
    GameObject gameManager;

    #endregion

    // Function used to set initial rarity value
    public void Init(WeaponRegistry weaponRegistry, Weapon.WeaponRarity rarity)
    {
        // Set registry and rarity
        this.rarity = rarity;
        this.gameManager = GameObject.FindGameObjectWithTag("GameManager");
        weapon = weaponRegistry.GetRandomWeapon(this.rarity);
    }

    // Handle collision with trigger
    void OnTriggerEnter2D(Collider2D collision)
    {
        // If not empty inside...
        if (!empty)
        {
            // Determine if other collider is player
            if (collision.gameObject.CompareTag("Player"))
            {
                // Show store blip and set animation
                GetComponent<Animator>().SetTrigger("contact");
                gameManager.GetComponent<GameManager>().ShowShopButton(gameObject);
            }
        }
    }

    // Handle trigger exit
    void OnTriggerExit2D(Collider2D collision)
    {
        // If not empty inside...
        if (!empty)
        {
            // Determine if other collider is player
            if (collision.gameObject.CompareTag("Player"))
            {
                // Hide store blip
                gameManager.GetComponent<GameManager>().HideShopButton();
            }
        }
    }

    // Handle purchase
    public void Purchase()
    {
        // Purchase weapon
        empty = true;
        GetComponent<Animator>().SetTrigger("empty");
    }
}
