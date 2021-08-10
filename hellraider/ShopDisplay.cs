using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopDisplay : MonoBehaviour
{
    #region Properties

    public GameManager manager;
    public Image preview;
    public Text title;
    public Text description;
    public Text rarity;
    public Text purchaseText;

    WeaponCache currentCache;

    #endregion

    // Sets the weapon display
    public void SetWeapon(WeaponCache cache)
    {
        // Save current cache
        currentCache = cache;
        // Set weapon values
        Weapon weapon = cache.weapon.GetComponent<Weapon>();
        preview.sprite = weapon.displayImage;
        title.text = weapon.displayName;
        description.text =
            "DAMAGE: " + weapon.damage + '\n' +
            "TYPE: " + weapon.Type.ToString() + '\n';
        rarity.text = weapon.rarity.ToString();
        purchaseText.text = "BUY: " + weapon.cost.ToString();
    }

    // Purchase event handler
    public void Purchase()
    {
        // Call current cache purchase event
        currentCache.Purchase();
    }
}
