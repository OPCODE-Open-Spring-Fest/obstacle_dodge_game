using UnityEngine;
using UnityEngine.SceneManagement;

public static class LastLevelRecorder
{
    private const string DefaultPrefsKey = "LastLevelIndex";

    public static void SaveCurrentLevel(string prefsKey = DefaultPrefsKey)
    {
        int idx = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt(prefsKey, idx);
        PlayerPrefs.Save();
        Debug.Log($"Saved last level index {idx} to PlayerPrefs key '{prefsKey}'");
    }
    public static void SaveAndLoad(string sceneToLoad, string prefsKey = DefaultPrefsKey)
    {
        SaveCurrentLevel(prefsKey);
        SceneManager.LoadScene(sceneToLoad);
    }

    public static void SaveAndLoad(int sceneBuildIndex, string prefsKey = DefaultPrefsKey)
    {
        SaveCurrentLevel(prefsKey);
        SceneManager.LoadScene(sceneBuildIndex);
    }
}
