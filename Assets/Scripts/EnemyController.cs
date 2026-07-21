using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3.5f;

    [Header("Combat")]
    public int touchDamage = 10;
    public float attackCooldown = 2f;
    public float flashDuration = 1f;

    [Header("Stuck?")]
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private float stuckCheckInterval = 0.5f;
    [SerializeField] private float minMovement = 0.05f;
    [SerializeField] private float timebeforeRecovery = 2f;
    [SerializeField] private float respawnMinDistance = 4f;
    [SerializeField] private float repsawnMaxDistance = 6f;

    private Vector2 lastStuckCheckPos;
    private float nextStuckCheckTime;
    private float stuckTime;

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
        if (rb != null)
            lastStuckCheckPos = rb.position;
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
        CheckIfStuck();
    }

    private void CheckIfStuck()
    {
        if (rb == null || target == null || isDead)
            return;

        if (Time.time < nextStuckCheckTime)
            return;

        nextStuckCheckTime = Time.time + stuckCheckInterval;

        float disFromPlayer = Vector2.Distance(rb.position, target.position);
        float disMoved = Vector2.Distance(rb.position, lastStuckCheckPos);

        bool shouldBeMoving = disFromPlayer > 1.5f && moveDirection.sqrMagnitude > 0.01f;
        if (shouldBeMoving && disMoved < minMovement)
            stuckTime += stuckCheckInterval;
        else
            stuckTime = 0f;

        lastStuckCheckPos = rb.position;

        if(stuckTime >= timebeforeRecovery)
        {
            moveToSafePos();
            stuckTime = 0f;
        }
    }

    private void moveToSafePos()
    {
        for(int i = 0; i < 12; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float dist = Random.Range(respawnMinDistance, repsawnMaxDistance);

            Vector2 tryPos = (Vector2)target.position + dir * dist;

            Collider2D obstace = Physics2D.OverlapCircle(tryPos, 0.3f, obstacleLayers);

            if (obstace != null)
                continue;

            rb.position = tryPos;
            rb.linearVelocity = Vector2.zero;
            lastStuckCheckPos = tryPos;
            return;
        }
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
        if (impulse != null && SettingsController.isScreenShake)
            impulse.GenerateImpulse();
    }
}
