using UnityEngine;

public class ActivateDefeat : MonoBehaviour
{
    public GameObject canvas;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Invoke("ActivateDefeatCanvas",3f);
    }

    // Update is called once per frame
    void ActivateDefeatCanvas()
    {
        canvas.SetActive(true);
    }
}
