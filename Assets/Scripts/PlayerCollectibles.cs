using System;
using System.Collections;
using UnityEngine;

public class PlayerCollectibles : MonoBehaviour
{
    [SerializeField] private int lives = 3;
    public bool IsInvincible { get; private set; }

    private const string COINS_KEY = "PlayerCoinsTotal";
    private int currentCoins = 0;


    public event Action<int> OnLivesChanged;
    public event Action<float> OnInvincibilityStarted;
    public event Action OnInvincibilityEnded;

    public int Lives => lives;

    public int GetLives() => lives;

    private void Awake()
    {
        LoadCoins();
    }

    public void AddLife(int amount = 1)
    {
        lives += amount;
        // Clamp lives to minimum 0 to prevent negative values
        if (lives < 0) lives = 0;
        Debug.Log($"Life added. Lives: {lives}");
        NotifyLivesChanged();
    }

    private void NotifyLivesChanged() => OnLivesChanged?.Invoke(lives);

    public void AddLives(int amount = 1)
    {
        AddLife(amount);
    }

    public void UseGem(float duration = 5f)
    {
        if (IsInvincible) return;
        OnInvincibilityStarted?.Invoke(duration);
        StartCoroutine(InvincibilityRoutine(duration));
    }

    public void StartInvincibility(float duration = 5f)
    {
        UseGem(duration);
    }

    private IEnumerator InvincibilityRoutine(float duration)
    {
        IsInvincible = true;
        var renderers = GetComponentsInChildren<Renderer>();

        foreach (var r in renderers)
        {
            if (r.material != null)
            {
                var mat = r.material;

                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    mat.color = new Color(c.r, c.g, c.b, 0.5f);
                }
                else if (mat.HasProperty("_TintColor"))
                {
                    Color c = mat.GetColor("_TintColor");
                    mat.SetColor("_TintColor", new Color(c.r, c.g, c.b, 0.5f));
                }
            }
        }

        yield return new WaitForSeconds(duration);

        foreach (var r in renderers)
        {
            if (r.material != null)
            {
                var mat = r.material;

                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    mat.color = new Color(c.r, c.g, c.b, 1f);
                }
                else if (mat.HasProperty("_TintColor"))
                {
                    Color c = mat.GetColor("_TintColor");
                    mat.SetColor("_TintColor", new Color(c.r, c.g, c.b, 1f));
                }
            }
        }

        IsInvincible = false;
        OnInvincibilityEnded?.Invoke();
    }

    public int GetCoinsCOunt()
    {
        return currentCoins;
    }
    public void AddCoins(int coinValue)
    {
        currentCoins += coinValue;
        SaveCoins();
        Debug.Log($"Collected coin! Total coins: {currentCoins}");
    }

    public bool TrySpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            SaveCoins();
            Debug.Log($"Spent {amount} coins, remaining coins {currentCoins}");
            return true;
        }
        Debug.Log($"Failed to spend, need {amount} but have {currentCoins}");
        return false;
    }
    private int LoadCoins()
    {
        currentCoins = PlayerPrefs.GetInt(COINS_KEY, 0);
        return currentCoins;
    }
    private void SaveCoins()
    {
        PlayerPrefs.SetInt(COINS_KEY, currentCoins);
        PlayerPrefs.Save();
    }

}