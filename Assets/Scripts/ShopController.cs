using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopController : MonoBehaviour
{
    // The key must exactly match the one in PlayerCollectibles
    private const string COINS_KEY = "PlayerCoinsTotal";

    [Header("UI References")]
    [Tooltip("Text field showing the player's current coin balance.")]
    public TextMeshProUGUI coinCountText;

    void Start()
    {
        // Ensure the display is updated immediately when the scene starts
        UpdateCoinDisplay();
    }

    /// <summary>
    /// Reads the coin balance from persistent storage (PlayerPrefs) and updates the UI text.
    /// </summary>
    public void UpdateCoinDisplay()
    {
        // Read directly from PlayerPrefs
        int totalCoins = PlayerPrefs.GetInt(COINS_KEY, 0);

        if (coinCountText != null)
        {
            // Update the UI text
            coinCountText.text = $" BALANCE: {totalCoins}";
        }
    }

    /// <summary>
    /// Attempts to purchase an item. Called by UI buttons via an integer cost parameter.
    /// </summary>
    /// <param name="cost">The price of the item to purchase.</param>
    public void BuyItem(int cost)
    {
        int currentCoins = PlayerPrefs.GetInt(COINS_KEY, 0);

        if (currentCoins >= cost)
        {
            // 1. Successful purchase: deduct coins and save
            PlayerPrefs.SetInt(COINS_KEY, currentCoins - cost);
            PlayerPrefs.Save();

            // 2. Refresh the UI to reflect the new balance
            UpdateCoinDisplay();

            // 3. --- PLACEHOLDER FOR ITEM UNLOCK LOGIC ---
            // You will replace this with logic to save the unlocked item status.
            // Example: PlayerPrefs.SetInt("Skin_Red_Unlocked", 1);

            Debug.Log($"Purchased item for {cost}. New balance: {currentCoins - cost}");
        }
        else
        {
            Debug.Log($"Purchase failed! Need {cost}, only have {currentCoins}.");
            // Add visual feedback to the player here!
        }
    }

    /// <summary>
    /// Handles the Back button to return to the Main Menu scene.
    /// </summary>
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}