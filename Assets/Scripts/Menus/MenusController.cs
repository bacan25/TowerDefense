using UnityEngine;
using UnityEngine.SceneManagement;

public class MenusController : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void NextScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
