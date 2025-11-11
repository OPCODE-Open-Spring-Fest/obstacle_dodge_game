using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LivesHeartUI : MonoBehaviour
{
    [Header("Heart Icons")]
    [Tooltip("A list of all the heart GameObjects. Drag your heart images here in order.")]
    public List<GameObject> heartIcons;

    private int currentHearts = -1;

    void Start()
    {
        if (SessionManager.Instance != null)
        {
            UpdateHearts(SessionManager.Instance.CurrentLives);
        }
        else
        {
            Debug.LogError("LivesHeartUI: SessionManager not found!");
            UpdateHearts(0);
        }
    }

    public void UpdateHearts(int lives)
    {
        if (lives == currentHearts) return;

        currentHearts = lives;

        for (int i = 0; i < heartIcons.Count; i++)
        {
            if (i < currentHearts)
            {
                heartIcons[i].SetActive(true);
            }
            else
            {
                heartIcons[i].SetActive(false);
            }
        }
    }
}