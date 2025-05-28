using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source para SFX")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Volumen")]
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Clips de Efectos de Sonido")]
    [SerializeField] private AudioClip[] sfxClips;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Reproduce un efecto de sonido por índice.
    /// </summary>
    public void PlaySFX(int index)
    {
        if (sfxSource == null || index < 0 || index >= sfxClips.Length) return;

        sfxSource.volume = sfxVolume;
        sfxSource.PlayOneShot(sfxClips[index]);
    }

    /// <summary>
    /// Cambia el volumen global de los efectos.
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
}
