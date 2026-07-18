using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class NightMode : MonoBehaviour
{

    [Header("Refrences")]
    public Image nightOverlay;

    [Header("Night look")]
    public Color nightColor = new Color(0.08f, 0.12f, 0.25f, 1f);
    [Range(0f, 1f)]
    public float nightAlpha = 0.78f;

    [Header("Transition")]
    public float fadeDuration = 1.5f;

    public Coroutine fadeRoutine;

    private void Awake()
    {
        if(nightOverlay != null)
        {
            Color c = nightOverlay.color;
            c.a = 0f;
            nightOverlay.color = c;
        }
    }

    private void OnTriggerEnter2D(Collider2D player)
    {
        if (!player.CompareTag("Player"))
            return;

        StartFade(nightAlpha);
    }

    private void OnTriggerExit2D(Collider2D player)
    {
        if (!player.CompareTag("Player"))
            return;

        StartFade(0f);
    }

    private void StartFade(float targetAlpha)
    {
        if (nightOverlay == null)
            return;

        if (!nightOverlay.gameObject.activeSelf)
            nightOverlay.gameObject.SetActive(true);

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOverlay(targetAlpha));
    }

    private IEnumerator FadeOverlay(float targetAlpha)
    {
        Color startColor = nightOverlay.color;
        Color targetColor = nightColor;
        targetColor.a = targetAlpha;

        float elapsed = 0f;
        while(elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            nightOverlay.color = Color.Lerp(startColor, targetColor, elapsed / fadeDuration);
            yield return null;
        }

        nightOverlay.color = targetColor;
    }
}
