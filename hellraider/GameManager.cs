using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Constants

    #endregion

    #region Instance Variables

    // Boolean used to determine whether game is paused or not
    public bool paused;

    // Bool used to determine whether game is over or not
    public bool gameOver;

    // Player object
    public GameObject player;

    // Obstacle directory
    public GameObject[] obstacles;

    // Pause menu canvas item
    public GameObject pauseMenu;

    // Pause button canvas item
    public GameObject pauseButton;

    // Gameover menu canvas item
    public GameObject gameoverMenu;

    // Quit Confirm canvas item
    public GameObject quitConfirm;

    // Game controller object
    private GameObject gameController;

    // Gun repository
    public WeaponRegistry weaponRegistry;

    // Item shop objects
    public GameObject shopDisplay;
    public GameObject shopPurchaseButton;
    public GameObject shopButton;
    private GameObject currentCache;

    // Souls data
    public SoulDisplay soulDisplay;
    public int souls = 3;

    #endregion

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    void Awake() { }

    // Update is called once per frame. Maintains the state of the game
    void Update()
    {
        // First, make sure game has not ended
        if (gameOver)
        {
            // TODO
            // Save stats
            // Show run stats
            // Show replay menu
        }
    }

    // Player jump event handler
    public void JumpClicked()
    {
        player.GetComponent<PlayerController>().Jump();
    }

    // Player fire event handler
    public void AttackClicked()
    {
        // Fire current equipped weapon
        player.GetComponent<PlayerController>().weapon.GetComponent<Weapon>().Attack();
    }

    // Used to signal the release of an attack
    public void AttackReleased()
    {
        player.GetComponent<PlayerController>().weapon.GetComponent<Weapon>().AttackRelease();
    }

    // Shows shop button
    public void ShowShopButton(GameObject currentCache)
    {
        this.currentCache = currentCache;
        shopButton.SetActive(true);
    }

    // Hide shop button
    public void HideShopButton()
    {
        shopButton.SetActive(false);
        shopDisplay.SetActive(false);
    }

    // Show shop display
    public void ShowShopDisplay()
    {
        shopDisplay.GetComponent<ShopDisplay>().SetWeapon(currentCache.GetComponent<WeaponCache>());
        shopDisplay.SetActive(true);
        shopButton.SetActive(false);
        if (souls < currentCache.GetComponent<WeaponCache>().weapon.GetComponent<Weapon>().cost)
        {
            shopPurchaseButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            shopPurchaseButton.GetComponent<Button>().interactable = true;
        }
    }

    // Hide shop display
    public void HideShopDisplay()
    {
        shopDisplay.SetActive(false);
    }

    // Shop closed click
    public void ShopCloseClicked()
    {
        shopButton.SetActive(true);
        HideShopDisplay();
    }

    // Purchase weapon
    public void PurchaseWeapon()
    {
        soulDisplay.RemoveSouls(currentCache.GetComponent<WeaponCache>().weapon.GetComponent<Weapon>().cost);
        player.GetComponent<PlayerController>().Equip(currentCache.GetComponent<WeaponCache>().weapon);
        currentCache.GetComponent<WeaponCache>().Purchase();
        shopDisplay.SetActive(false);
    }

    // Method used to end current game
    public void GameOver()
    {
        // Set game state to "game over". This will freeze all update calls
        gameOver = true;
        // Destroy player
        player.GetComponent<PlayerController>().GameOver();
        // Save game
        SavePlayerStats();
    }

    // Save player stats
    private void SavePlayerStats()
    {
        gameController.GetComponent<GameController>().Save();
    }

    // Pause game event handler
    public void PauseGame()
    {
        // Stop game time
        Time.timeScale = 0f;
        // Show pause menu
        pauseMenu.SetActive(true);
        // hide pause button
        pauseButton.SetActive(false);
    }

    // Start game event handler
    public void StartGame()
    {
        // Hide pause menu
        pauseMenu.SetActive(false);
        // Start game time
        Time.timeScale = 1f;
        // show pause button
        pauseButton.SetActive(true);
    }

    // Exit game event handler
    public void QuitGame()
    {
        // show quit confirmation window
        quitConfirm.SetActive(true);
        // hide current window
        pauseMenu.SetActive(false);
    }

    // Event handler for quit confirmation
    public void ConfirmQuit()
    {
        // Set game time
        Time.timeScale = 1f;
        // Start new menu scene
        gameController.GetComponent<GameController>().LoadScene("Menu");
    }

    // Event handler for canceling quit action
    public void CancelQuit()
    {
        // Hide confirmation window
        quitConfirm.SetActive(false);
        // Show pause menu
        pauseMenu.SetActive(true);
    }

    // Replay game event handler
    public void ReplayGame()
    {
        // Set time scale
        Time.timeScale = 1f;
        // Reload current scene
        gameController.GetComponent<GameController>().LoadScene("Game");
    }
}