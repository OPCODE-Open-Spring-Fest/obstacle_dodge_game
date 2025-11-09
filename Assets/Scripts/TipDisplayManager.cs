using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TipDisplayManager : MonoBehaviour
{
    [Header("Game Tips")]
    [Tooltip("Enter one tip per element in the list. The script will pick one randomly.")]
    public string[] gameTips = new string[]
    {
        " Tip: Use WASD or Arrow keys to move the player.",
        " Tip: Use dash (J key) to escape tight spots!",
        " Tip: Watch out for enemies that follow you!",
        " Tip: Collect power-ups for a speed boost or invincibility.",
        " Tip: Avoiding the boundary walls is key to survival!",
        " Tip: The game will pause for a brief countdown before starting." 
    };

    private TextMeshProUGUI tipText;

    void Start()
    {
        tipText = GetComponent<TextMeshProUGUI>();

        if (gameTips.Length == 0)
        {
            tipText.text = "No tips available.";
            return;
        }

        int randomIndex = Random.Range(0, gameTips.Length);
        tipText.text = gameTips[randomIndex];
    }
}