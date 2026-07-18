using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Punch Sprites")]
    public Sprite punchUp;
    public Sprite punchDown;
    public Sprite punchLeft;
    public Sprite punchRight;

    [Header("Attack Settings")]
    public float attackDamage = 10f;
    public float attackRange = 0.8f;
    public float attackRadius = 0.5f;
    public float attackCooldown = 0.4f;
    public float punchSpDuration = 0.15f;

    private SpriteRenderer sr;
    private Animator anim;
    private float lastAttackTime = -Mathf.Infinity;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        TryAttack();
    }

    public void TryAttack()
    {
        if (PauseController.isGamePaused)
            return;

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        Vector2 facing = GetFacingDir();
        Sprite punchSprite = GetPunchSprite(facing);

        if (anim != null)
            anim.enabled = false;

        if(sr != null && punchSprite != null)
            sr.sprite = punchSprite;

        DealDamage(facing);

        yield return new WaitForSeconds(punchSpDuration);

        if (anim != null)
            anim.enabled = true;
    }

    private Vector2 GetFacingDir()
    {
        if (anim == null)
            return Vector2.down;

        float x = anim.GetFloat("InputX");
        float y = anim.GetFloat("InputY");

        if (Mathf.Abs(x) > Mathf.Abs(y))
            return x > 0 ? Vector2.right : Vector2.left;
        else
            return y > 0 ? Vector2.up : Vector2.down;
    }

    private Sprite GetPunchSprite(Vector2 facing)
    {
        if (facing == Vector2.up)
            return punchUp;

        if (facing == Vector2.down)
            return punchDown;

        if (facing == Vector2.right)
            return punchRight;

        return punchLeft;
    }

    private void DealDamage(Vector2 facing)
    {
        Vector2 attackPoint = (Vector2)transform.position + facing * attackRange;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint, attackRadius);

        foreach (Collider2D hit in hits)
        {
            EnemyHelath enemHealth = hit.GetComponentInParent<EnemyHelath>();
            if (enemHealth != null)
                enemHealth.TakeDamage(attackDamage);
        } 
    }
}
