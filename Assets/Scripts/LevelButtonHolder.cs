using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButtonHandler : MonoBehaviour
{
    // Function to be called by each button
    public void LoadLevel(string levelName)
    {
        // Debug.Log("Loading Level " + levelIndex);
        SceneManager.LoadScene("StoryLine");
    }
    public void LoadLeve2(string levelName)
    {
        // Debug.Log("Loading Level " + levelIndex);
        SceneManager.LoadScene("StoryLine2");
    }
    public void LoadLeve3(string levelName)
    {
        // Debug.Log("Loading Level " + levelIndex);
        SceneManager.LoadScene("StoryLine3");
    }
    public void LoadLeve4(string levelName)
    {
        // Debug.Log("Loading Level " + levelIndex);
        SceneManager.LoadScene("StoryLine4");
    }
      public void LoadLeve5(string levelName)
    {
        // Debug.Log("Loading Level " + levelIndex);
        SceneManager.LoadScene("StoryLine5");
    }
      public void LoadLeve6(string levelName)
    {
        // Debug.Log("Loading Level " + levelIndex);
        SceneManager.LoadScene(14);
    }
}
