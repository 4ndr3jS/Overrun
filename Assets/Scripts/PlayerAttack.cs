using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Punch")]
    public Sprite punchUp;
    public Sprite punchDown;
    public Sprite punchLeft;
    public Sprite punchRight;

    [Header("Axe Swing")]
    public bool axeEquipped = false;
    public AnimationClip axeAttackUp;
    public AnimationClip axeAttackDown;
    public AnimationClip axeAttackLeft;
    public AnimationClip axeAttackRight;

    [Header("Attack Settings")]
    public float attackDamage = 10f;
    public float attackRange = 0.8f;
    public float attackRadius = 0.5f;
    public float attackCooldown = 0.4f;
    public float punchSpriteDuration = 0.15f;
    public float hitDelay = 0.2f;

    private const string AttackState = "AttackLeft";
    private const string IdleState = "Idle";
    private const string WalkState = "Walk";

    private SpriteRenderer sr;
    private SpriteRenderer axe;
    private Animator anim;
    private HotbarController hotbar;
    private AnimatorOverrideController overrideController;
    private AnimationClip baseAttackClip;
    private Coroutine attackCoroutine;
    private float lastAttackTime = -Mathf.Infinity;
    private Vector3 oldScale;
    private bool flipped;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        hotbar = FindAnyObjectByType<HotbarController>();

        Transform effect = transform.Find("Effect");
        if (effect != null)
            axe = effect.GetComponent<SpriteRenderer>();

        SetUpAxeAnimations();
        HideAxe();
    }

    private void LateUpdate()
    {
        if (attackCoroutine == null)
        {
            HideAxe();
            ResetPlayerFlip();
        }
    }

    private void OnDisable()
    {
        HideAxe();
        ResetPlayerFlip();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            TryAttack();
    }

    public void TryAttack()
    {
        if (PauseController.isGamePaused || Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        StopCurrentAttack();
        attackCoroutine = StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        Vector2 direction = GetFacingDirection();

        if (HasAxeEquipped())
            yield return AxeAttack(direction);
        else
            yield return PunchAttack(direction);

        attackCoroutine = null;
    }

    private IEnumerator AxeAttack(Vector2 direction)
    {
        AnimationClip clip = GetAxeAnimation(direction);
        bool useMirroredRightAnimation = direction == Vector2.left &&
                                         !HasAnimation(axeAttackLeft) &&
                                         clip == axeAttackRight;

        if (anim != null && overrideController != null && baseAttackClip != null && clip != null)
        {
            if (useMirroredRightAnimation)
                FlipPlayerForAttack();

            overrideController[baseAttackClip] = clip;
            anim.enabled = true;
            anim.Play(AttackState, 0, 0f);
            anim.Update(0f);

            yield return new WaitForSeconds(Mathf.Min(hitDelay, clip.length));
            DamageEnemies(direction);

            yield return new WaitForSeconds(Mathf.Max(0f, clip.length - hitDelay));
            ReturnToMovementAnimation();
        }
        else
        {
            ShowPunch(direction);
            yield return new WaitForSeconds(hitDelay);
            DamageEnemies(direction);
            yield return new WaitForSeconds(punchSpriteDuration);
        }

        ResetPlayerFlip();
        HideAxe();
    }

    private IEnumerator PunchAttack(Vector2 direction)
    {
        ShowPunch(direction);
        yield return new WaitForSeconds(hitDelay);
        DamageEnemies(direction);
        yield return new WaitForSeconds(punchSpriteDuration);
    }

    private void StopCurrentAttack()
    {
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        attackCoroutine = null;
        ResetPlayerFlip();
        HideAxe();
    }

    private void SetUpAxeAnimations()
    {
        if (anim == null || anim.runtimeAnimatorController == null)
            return;

        overrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
        List<KeyValuePair<AnimationClip, AnimationClip>> clips = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(clips);

        foreach (KeyValuePair<AnimationClip, AnimationClip> item in clips)
        {
            if (item.Key != null && item.Key.name == AttackState)
            {
                baseAttackClip = item.Key;
                break;
            }
        }

        if (baseAttackClip != null)
            anim.runtimeAnimatorController = overrideController;
    }

    private bool HasAxeEquipped()
    {
        if (axeEquipped)
            return true;

        if (hotbar == null || hotbar.hotbarPanel == null)
            return false;

        int slotNum = hotbar.GetSelectedSlot();
        if (slotNum < 0 || slotNum >= hotbar.hotbarPanel.transform.childCount)
            return false;

        Slot slot = hotbar.hotbarPanel.transform.GetChild(slotNum).GetComponent<Slot>();
        if (slot == null || slot.currentItem == null)
            return false;

        Item item = slot.currentItem.GetComponent<Item>();
        return item != null && item.Name == "Axe";
    }

    private Vector2 GetFacingDirection()
    {
        if (anim == null)
            return Vector2.down;

        float x = anim.GetFloat("InputX");
        float y = anim.GetFloat("InputY");

        if (Mathf.Approximately(x, 0f) && Mathf.Approximately(y, 0f))
        {
            x = anim.GetFloat("LastInputX");
            y = anim.GetFloat("LastInputY");
        }

        if (Mathf.Abs(x) > Mathf.Abs(y))
            return x > 0 ? Vector2.right : Vector2.left;

        return y > 0 ? Vector2.up : Vector2.down;
    }

    private AnimationClip GetAxeAnimation(Vector2 direction)
    {
        if (direction == Vector2.up)
            return axeAttackUp;

        if (direction == Vector2.down)
            return axeAttackDown;

        if (direction == Vector2.right)
            return axeAttackRight;

        return HasAnimation(axeAttackLeft) ? axeAttackLeft : axeAttackRight;
    }

    private bool HasAnimation(AnimationClip clip)
    {
        return clip != null && !clip.empty;
    }

    private void ShowPunch(Vector2 direction)
    {
        if (sr == null)
            return;

        Sprite sprite = punchDown;

        if (direction == Vector2.up)
            sprite = punchUp;
        else if (direction == Vector2.left)
            sprite = punchLeft;
        else if (direction == Vector2.right)
            sprite = punchRight;

        if (sprite != null)
            sr.sprite = sprite;
    }

    private void ReturnToMovementAnimation()
    {
        if (anim == null)
            return;

        anim.Play(anim.GetBool("isWalking") ? WalkState : IdleState, 0, 0f);
    }

    private void HideAxe()
    {
        if (axe != null)
            axe.sprite = null;
    }

    private void FlipPlayerForAttack()
    {
        oldScale = transform.localScale;
        flipped = true;
        transform.localScale = new Vector3(-Mathf.Abs(oldScale.x), oldScale.y, oldScale.z);
    }

    private void ResetPlayerFlip()
    {
        if (!flipped)
            return;

        transform.localScale = oldScale;
        flipped = false;
    }

    private void DamageEnemies(Vector2 direction)
    {
        Vector2 hitPos = (Vector2)transform.position + direction * attackRange;
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPos, attackRadius);

        foreach (Collider2D hit in hits)
        {
            EnemyHelath enemy = hit.GetComponentInParent<EnemyHelath>();
            if (enemy != null)
                enemy.TakeDamage(attackDamage);
        }
    }
}
