using UnityEngine;

/// <summary>
/// Gestor de audio global para música y efectos de sonido.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Aquí puedes añadir métodos como PlayMusic, PlaySFX, etc.
}
