using JetBrains.Annotations;
using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerDeathController : MonoBehaviour
{
    [Header("Refrences")]
    public SpriteRenderer sr;
    public PlayerController pc;
    public Rigidbody2D rb;
    public Animator anim;

    [Header("Death")]
    public Sprite[] deathFrames;
    public float frameInterval = 0.3f;
    public float panelDelay = 0.6f;

    public GameObject deathPanel;

    [Header("Respawn")]
    public Transform respawnPoint;
    public Collider2D respawnRoomBounds;
    public CinemachineConfiner2D confiner;
    public CinemachineCamera cmCamera;

    private Sprite originalSprite;
    private Coroutine deathRoutine;

    [SerializeField] private WaveController waveController;

    private void Start()
    {
        if (PlayerVitals.Instance != null)
            PlayerVitals.Instance.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (PlayerVitals.Instance != null)
            PlayerVitals.Instance.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        PauseController.SetPause(true);

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (pc != null)
            pc.enabled = false;

        if (anim != null)
            anim.enabled = false;

        deathRoutine = StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        if(sr != null && deathFrames != null && deathFrames.Length > 0)
        {
            originalSprite = sr.sprite;

            sr.sprite = deathFrames[0];
            yield return new WaitForSeconds(frameInterval);

            if (deathFrames.Length > 1)
                sr.sprite = deathFrames[1];
        }

        float remaining = panelDelay - frameInterval;
        if (remaining > 0f)
            yield return new WaitForSeconds(remaining);

        if (deathPanel != null)
            deathPanel.SetActive(true);
    }

    public void Respawn()
    {
        if(deathRoutine != null)
        {
            StopCoroutine(deathRoutine);
            deathRoutine = null;
        }

        if (anim != null)
            anim.enabled = true;

        if (sr != null && originalSprite != null)
            sr.sprite = originalSprite;

        Vector3 oldPos =  transform.position;

        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        if (confiner != null && respawnRoomBounds != null)
        {
            confiner.BoundingShape2D = respawnRoomBounds;
            confiner.InvalidateBoundingShapeCache();
        }

        if(cmCamera != null)
        {
            Vector3 delta = transform.position - oldPos;
            cmCamera.OnTargetObjectWarped(transform, delta);
        }

        if (waveController != null)
            waveController.ResetWaves();
        else
            Debug.Log("WaveController is not assigend.");

        if (PlayerVitals.Instance != null)
            PlayerVitals.Instance.Revive();

        if (deathPanel != null)
            deathPanel.SetActive(false);

        PauseController.SetPause(false);

        if (pc != null)
        {
            pc.enabled = true;
            pc.ResyncMoveInput();
        }
    }
}
