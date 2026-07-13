using UnityEngine;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    private CanvasGroup canvasGroup;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            Destroy(transform.root.gameObject);
        }
    }

    public IEnumerator FadeOut(float duration)
    {
        canvasGroup.blocksRaycasts = true;
        yield return Fade(0f, 1f, duration);
    }

    public IEnumerator FadeIn(float duration)
    {
        yield return Fade(1f, 0f, duration);
        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsedTime = 0f;
        canvasGroup.alpha = from;
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
