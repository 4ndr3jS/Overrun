using System.Collections;
using UnityEngine;

public class SpriteFlash : MonoBehaviour
{
    public Material flashMaterial;

    private SpriteRenderer spriteRenderer;
    public Material originalMaterial;
    private Color originalColor;
    private Coroutine flashRoutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        originalMaterial = spriteRenderer.material;
    }

    public void Flash(Color flashColor, float duration)
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            ResetVisuals();
        }

        flashRoutine = StartCoroutine(FlashRoutine(flashColor, duration));
    }

    private IEnumerator FlashRoutine(Color flashColor, float duration)
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor;
    }


    public void DeathFlicker(int flickerCount, float flickerInterval, System.Action onComplete = null)
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            ResetVisuals();
        }

        flashRoutine = StartCoroutine(DeathFlickerRoutine(flickerCount, flickerInterval, onComplete));
    }

    private IEnumerator DeathFlickerRoutine(int flickerCount, float flickerInterval, System.Action onComplete)
    {
        for(int i = 0; i < flickerCount; i++)
        {
            if(flashMaterial != null)
            {
                spriteRenderer.material = flashMaterial;
                spriteRenderer.material.SetColor("_Color", Color.white);
            }
            else
            {
                spriteRenderer.color = Color.white;
            }

            yield return new WaitForSeconds(flickerInterval);

            if (flashMaterial != null)
                spriteRenderer.material = originalMaterial;
            else
                spriteRenderer.color = originalColor;

            yield return new WaitForSeconds(flickerInterval);
        }

        onComplete?.Invoke();
    }

    private void ResetVisuals()
    {
        spriteRenderer.color = originalColor;
        spriteRenderer.material = originalMaterial;
    }
}
