using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulDisplay : MonoBehaviour
{
    #region Properties

    public GameManager gameManager;
    public Text textDisplay;
    public RectTransform iconTransform;

    #endregion

    // Event called when soul is added to inventory
    public void AddSoul()
    {
        gameObject.GetComponent<Animator>().SetTrigger("added");
        gameManager.souls++;
        textDisplay.text = gameManager.souls.ToString();
    }

    // Event called when souls removed from inventory
    public void RemoveSouls(int souls)
    {
        gameManager.souls -= souls;
        textDisplay.text = gameManager.souls.ToString();
    }
}
