using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class UIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color hoverColor = Color.yellow;
    public float hoverScale = 1.1f;
    public float transitionSpeed = 8f;

    [Header("Sonido de Hover")]
    public AudioClip hoverSFX;
    public float sfxVolume = 1f;

    private Vector3 originalScale;
    private Color originalColor;
    private Image buttonImage;
    private Coroutine scaleCoroutine;

    private AudioSource audioSource;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color;
        originalScale = transform.localScale;

        // Agrega un AudioSource si no existe
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.color = hoverColor;
        StartScaleAnimation(originalScale * hoverScale);

        if (hoverSFX != null)
            audioSource.PlayOneShot(hoverSFX, sfxVolume);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.color = originalColor;
        StartScaleAnimation(originalScale);
    }

    private void StartScaleAnimation(Vector3 targetScale)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ScaleTo(targetScale));
    }

    private IEnumerator ScaleTo(Vector3 targetScale)
    {
        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);
            yield return null;
        }
        transform.localScale = targetScale;
    }
}
