using System.Collections;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Play(Sprite[] frames, float frameDuration)
    {
        StartCoroutine(PlayRoutinue(frames, frameDuration));
    }

    private IEnumerator PlayRoutinue(Sprite[] frames, float frameDuration)
    {
        foreach(Sprite frame in frames)
        {
            if (sr != null && frame != null)
                sr.sprite = frame;

            yield return new WaitForSeconds(frameDuration);
        }

        Destroy(gameObject);
    }
}
