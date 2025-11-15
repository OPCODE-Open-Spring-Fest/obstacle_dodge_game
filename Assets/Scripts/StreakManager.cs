using UnityEngine;
using TMPro;

public class StreakManager : MonoBehaviour
{
    public static StreakManager Instance;

    [Header("Streak Logic")]
    [Tooltip("How many dodges are needed to increase the multiplier by 1.")]
    public int dodgesPerMultiplier = 5;

    [Header("UI References")]
    [Tooltip("The UI Text element to display the multiplier.")]
    public TextMeshProUGUI comboText;

    public int currentStreak { get; private set; }
    public int comboMultiplier { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ResetStreak();
    }

    public void IncreaseStreak()
    {
        currentStreak++;
        comboMultiplier = 1 + (currentStreak / dodgesPerMultiplier);
        UpdateUI();
    }

    public void ResetStreak()
    {
        currentStreak = 0;
        comboMultiplier = 1;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (comboText == null) return;

        if (comboMultiplier > 1)
        {
            comboText.gameObject.SetActive(true);
            comboText.text = $"{comboMultiplier}x MULTIPLIER";
        }
        else
        {
            comboText.gameObject.SetActive(false);
        }
    }
}