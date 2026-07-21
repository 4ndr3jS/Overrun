using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(SpriteRenderer))]
public class Bomb : MonoBehaviour
{
    [Min(0f)]
    public float damage = 150f;

    [Min(0.1f)]
    public float explosionRadius = 3f;

    public LayerMask enemyLayers;

    [Min(0.1f)]
    public float fuseDuration = 1f;

    [Min(1)]
    public int flashCount = 3;

    public Sprite[] explosionFrames;
    public float explosionFrameInterval = 0.08f;
    public float inlarge = 2f;

    private SpriteRenderer sr;
    private SpriteFlash sf;
    private bool hasExploded;

    public string explosion = "Explosion";
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sf = GetComponent<SpriteFlash>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        if (sr != null)
        {
            float flashInterval =fuseDuration / (flashCount * 2f);

            sf.DeathFlicker(flashCount, flashInterval, Explode);
        }
        else
        {
            StartCoroutine(FuseRoutine());
        }
    }

    private IEnumerator FuseRoutine()
    {
        yield return new WaitForSeconds(fuseDuration);
        Explode();
    }

    private void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;

        SoundEffectManager.Play(explosion);
        if (impulseSource != null)
            impulseSource.GenerateImpulse();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayers);

        HashSet<EnemyHelath> damagedEnemies = new HashSet<EnemyHelath>();

        foreach (Collider2D hit in hits)
        {
            EnemyHelath enemy = hit.GetComponentInParent<EnemyHelath>();

            if (enemy != null && damagedEnemies.Add(enemy))
                enemy.TakeDamage(damage);
        }

        StartCoroutine(PlayExplosionAnimation());
    }

    private IEnumerator PlayExplosionAnimation()
    {
        if (sr == null || explosionFrames == null || explosionFrames.Length == 0)
        {
            Destroy(gameObject);
            yield break;
        }

        transform.localScale *= inlarge;

        foreach (Sprite frame in explosionFrames)
        {
            if (frame != null)
                sr.sprite = frame;

            yield return new WaitForSeconds(explosionFrameInterval);
        }

        Destroy(gameObject);
    }
}