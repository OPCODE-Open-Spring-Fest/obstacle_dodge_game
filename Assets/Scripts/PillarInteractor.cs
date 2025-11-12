using UnityEngine;
using TMPro;

public class PillarInteractor : MonoBehaviour
{
    public GameObject minigameCanvas;
    public TextMeshProUGUI interactionPromptText;

    private bool playerIsNear = false;

    void Start()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerIsNear && !minigameCanvas.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            ActivateMinigame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            if (interactionPromptText != null)
            {
                interactionPromptText.text = "Press [E] to Play Tic-Tac-Toe";
                interactionPromptText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            if (interactionPromptText != null)
            {
                interactionPromptText.gameObject.SetActive(false);
            }
        }
    }

    private void ActivateMinigame()
    {
        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(true);
            Time.timeScale = 0f;
            if (interactionPromptText != null)
            {
                interactionPromptText.gameObject.SetActive(false);
            }
        }
    }
}