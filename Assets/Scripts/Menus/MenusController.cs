using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class MenusController : MonoBehaviour
{
    public void MainMenu()
    {
        CleanupPersistentManagers();
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void RestartLevel()
    {
        CleanupPersistentManagers();
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void NextScene()
    {
        CleanupPersistentManagers();
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1, LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void CleanupPersistentManagers()
    {
        if (GameManager.Instance != null)
            Destroy(GameManager.Instance.gameObject);
        if (UIManager.Instance != null)
            Destroy(UIManager.Instance.gameObject);
        if (ZonasConstruccion.Instance != null)
            Destroy(ZonasConstruccion.Instance.gameObject);
        // …y cualquier otro singleton con DontDestroyOnLoad
    }
}
