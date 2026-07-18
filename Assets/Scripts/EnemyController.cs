using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3.5f;

    [Header("Combat")]
    public int touchDamage = 10;
    public float attackCooldown = 2f;
    public float flashDuration = 1f;

    Rigidbody2D rb;
    Transform target;
    Vector2 moveDirection;
    Animator animator;

    EnemyHelath health;

    private bool isDead = false;

    private float lastAttackTime = -Mathf.Infinity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        health = GetComponent<EnemyHelath>();
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        isDead = true;
        moveDirection = Vector2.zero;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        UpdateAnimator();
    }

    void Start()
    {
        target = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (isDead)
            return;

        if (PauseController.isGamePaused)
        {
            moveDirection = Vector2.zero;
            UpdateAnimator();
            return;
        }

        if (target)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            moveDirection = direction;
        }

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
            return;
        }

        if (PauseController.isGamePaused)
        {
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
            return;
        }

        if (target)
        {
            rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null)
            return;

        bool isWalking = moveDirection.sqrMagnitude > 0.01f;
        animator.SetBool("isWalking", isWalking);

        if (isWalking)
        {
            animator.SetFloat("InputX", moveDirection.x);
            animator.SetFloat("InputY", moveDirection.y);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        if (isDead || PauseController.isGamePaused)
            return;

        if (PlayerVitals.Instance != null && PlayerVitals.Instance.IsDead())
            return;

        if (Time.time < lastAttackTime + attackCooldown)
            return;
        lastAttackTime = Time.time;

        PlayerVitals.Instance?.TakeDamage(touchDamage);

        SpriteFlash flash = collision.collider.GetComponent<SpriteFlash>();
        if (flash != null)
            flash.Flash(new Color(1f, 0.3f, 0.3f), flashDuration);

        CinemachineImpulseSource impulse = collision.collider.GetComponent<CinemachineImpulseSource>();
        if (impulse != null)
            impulse.GenerateImpulse();
    }
}
