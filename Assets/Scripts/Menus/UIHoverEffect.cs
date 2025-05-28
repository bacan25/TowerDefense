// File: UIHoverEffect.cs
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class UIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color hoverColor = Color.yellow;
    public float hoverScale = 1.1f;
    public float transitionSpeed = 8f;

    private Vector3 originalScale;
    private Color originalColor;
    private Image buttonImage;
    private Coroutine scaleCoroutine;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color;
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.color = hoverColor;
        StartScaleAnimation(originalScale * hoverScale);
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
