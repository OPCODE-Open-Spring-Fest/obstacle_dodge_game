using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{

    public GameObject slide1;
    public GameObject slide2;
    public GameObject slide3;


    public string mainGameSceneName = "Level1";


    public void ShowSlide2()
    {
        slide1.SetActive(false);
        slide2.SetActive(true);
    }


    public void ShowSlide3()
    {
        slide2.SetActive(false);
        slide3.SetActive(true);
    }

    public void StartGame()
    {

        SceneManager.LoadScene(mainGameSceneName);
    }
}