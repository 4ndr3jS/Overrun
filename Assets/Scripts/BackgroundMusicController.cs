using System;
using System.Collections;
using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    [Serializable]
    private class MapMusic
    {
        public Collider2D bounds;
        public AudioClip clip;
    }

    [Header("Map music")]
    [SerializeField] private MapMusic[] maps = new MapMusic[2];

    [SerializeField] private Transform player;

    [Header("Audio")]
    [SerializeField, Range(0, 1f)] private float musicVolume = 0.5f;
    [SerializeField, Min(0f)] private float fadeDuration = 0.5f;

    [SerializeField] private bool stopOutsideOfBounds;
    private AudioSource songSource;
    private Coroutine fadeCoroutine;

    private int currentmap = -2;

    private void Awake()
    {
        songSource = GetComponent<AudioSource>();

        songSource.playOnAwake = false;
        songSource.loop = true;
        songSource.spatialBlend = 0f;
        songSource.volume = 0f;
    }

    private void Start()
    {
        FindPlayer();

        if (player != null)
            RefreshMusic(true);
    }

    private void Update()
    {
        if(player == null)
        {
            FindPlayer();

            if (player == null)
                return;
        }

        RefreshMusic(false);
    }
    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    private void RefreshMusic(bool playImmediately)
    {
        int newMap = GetMapIndex(player.position);

        if (newMap < 0 && !stopOutsideOfBounds)
            return;

        if (newMap == currentmap)
            return;

        currentmap = newMap;

        AudioClip newClip = newMap >= 0 ? maps[newMap].clip : null;

        ChangeMusic(newClip, playImmediately);
    }

    private int GetMapIndex(Vector2 position)
    {
        for(int i = 0; i < maps.Length; i++)
        {
            MapMusic map = maps[i];

            if (map != null && map.bounds != null && map.bounds.OverlapPoint(position))
                return i;
        }
        return -1;
    }

    private void ChangeMusic(AudioClip newClip, bool playImmediately)
    {
        if(fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if(songSource.clip == newClip)
        {
            if (newClip != null)
            {
                songSource.volume = musicVolume;
                if (!songSource.isPlaying)
                    songSource.Play();
            }
            else
            {
                songSource.Stop();
                songSource.volume = 0f;
            }
            return;
        }

        if(playImmediately || fadeDuration <= 0f)
        {
            songSource.Stop();
            songSource.clip = newClip;
            songSource.volume = musicVolume;

            if (newClip != null)
                songSource.Play();

            return;
        }

        fadeCoroutine = StartCoroutine(fadeTo(newClip));
    }

    private IEnumerator fadeTo(AudioClip newClip)
    {
        float timer = 0f;
        float startVolume = songSource.volume;

        while(timer < fadeDuration && songSource.isPlaying)
        {
            timer += Time.unscaledDeltaTime;

            songSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);

            yield return null;
        }

        songSource.Stop();
        songSource.clip = newClip;
        songSource.volume = 0f;

        if (newClip == null)
        {
            fadeCoroutine = null;
            yield break;
        }

        songSource.Play();
        timer = 0f;

        while(timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / fadeDuration);

            songSource.volume = Mathf.Lerp(0f, musicVolume, progress);

            yield return null;
        }

        songSource.volume = musicVolume;
        fadeCoroutine = null;
    }
}
