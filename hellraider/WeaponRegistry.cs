using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRegistry : MonoBehaviour
{
    #region Class Variables

    public GameObject[] weapons;

    #endregion

    // Methods for returning weapon based on difficulty
    public GameObject GetRandomWeapon(Weapon.WeaponRarity rarity)
    {
        var rarityList = GetWeaponsRarity(rarity);
        int randIndex = Random.Range(0, rarityList.Length-1);
        return rarityList[randIndex];
    }

    // Methods for returning weapons based on rarity
    public GameObject[] GetWeaponsRarity(Weapon.WeaponRarity rarity)
    {
        var returnList = new List<GameObject>();
        foreach (GameObject weapon in weapons)
        {
            if (weapon.GetComponent<Weapon>().rarity == rarity) returnList.Add(weapon);
        }
        return returnList.ToArray();
    }
}
