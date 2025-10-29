using System;
using System.Collections;
using UnityEngine;

public class PlayerCollectibles : MonoBehaviour
{
    [SerializeField] private int lives = 3;
    public bool IsInvincible { get; private set; }

    public event Action<int> OnLivesChanged;
    public event Action<float> OnInvincibilityStarted;
    public event Action OnInvincibilityEnded;

    public int Lives => lives;

    public int GetLives() => lives;

    public void AddLife(int amount = 1)
    {
        lives += amount;
        Debug.Log($"Life added. Lives: {lives}");
        NotifyLivesChanged();
    }

    private void NotifyLivesChanged() => OnLivesChanged?.Invoke(lives);

    // Compatibility wrapper: some existing scripts call AddLives
    public void AddLives(int amount = 1)
    {
        AddLife(amount);
    }

    public void UseGem(float duration = 5f)
    {
        if (IsInvincible) return;
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
                Color c = r.material.color;
                r.material.color = new Color(c.r, c.g, c.b, 0.5f);
            }
        }
        yield return new WaitForSeconds(duration);

        foreach (var r in renderers)
        {
            if (r.material != null)
            {
                Color c = r.material.color;
                r.material.color = new Color(c.r, c.g, c.b, 1f);
            }
        }

        IsInvincible = false;
        OnInvincibilityEnded?.Invoke();
    }
}
