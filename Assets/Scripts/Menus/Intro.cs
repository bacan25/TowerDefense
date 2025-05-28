using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Intro : MonoBehaviour
{
    
    void Start()
    {
        Invoke("NextScene", 11f);
    }

    public void NextScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1, LoadSceneMode.Single);
    }
}
