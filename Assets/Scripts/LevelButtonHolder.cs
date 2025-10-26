using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButtonHandler : MonoBehaviour
{
    // Function to be called by each button
    public void LoadLevel(int levelIndex)
    {
        Debug.Log("Loading Level " + levelIndex);
        SceneManager.LoadScene(levelIndex);
    }
}
