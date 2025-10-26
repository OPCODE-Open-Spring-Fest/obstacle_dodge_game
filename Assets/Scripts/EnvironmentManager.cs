using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Assign skyboxes for levels")]
    public Material level1Sky;
    public Material level4Sky;
    public Material level5Sky;

    void Start()
    {
        int index = SceneManager.GetActiveScene().buildIndex;

        switch (index)
        {
            case 1:
                RenderSettings.skybox = level1Sky;
                break;
            case 4:
                RenderSettings.skybox = level4Sky;
                break;
            case 5:
                RenderSettings.skybox = level5Sky;
                break;
            default:
                Debug.Log("No specific skybox assigned for this scene.");
                break;
        }

        DynamicGI.UpdateEnvironment(); // refresh lighting
    }
}
//environment management