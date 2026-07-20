using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Punch")]
    public Sprite punchUp;
    public Sprite punchDown;
    public Sprite punchLeft;
    public Sprite punchRight;
    public float punchDamage = 5f;
    public float punchRange = 0.8f;
    public float punchRadius = 0.5f;
    public float punchCooldown = 0.4f;
    public float punchHitDelay = 0.2f;
    public float punchDuration = 0.4f;
    public float punchStamina = 3f;

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
    private bool oldPlayerFlip;
    private bool oldAxeFlip;
    private bool flipped;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        hotbar = FindAnyObjectByType<HotbarController>();

        Transform effect = transform.Find("Effect");
        if (effect != null)
            axe = effect.GetComponent<SpriteRenderer>();

        SetUpAnimations();
        HideWeaponEffect();
    }

    private void LateUpdate()
    {
        if (attackCoroutine == null)
        {
            HideWeaponEffect();
            ResetPlayerFlip();
        }
    }

    private void OnDisable()
    {
        HideWeaponEffect();
        ResetPlayerFlip();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            TryAttack();
    }

    public void TryAttack()
    {
        if (PauseController.isGamePaused)
            return;

        Item weapon = GetSelectedWeapon();
        float cooldown = weapon != null ? weapon.weaponCooldown : punchCooldown;
        float staminCost = weapon != null ? weapon.weaponStamina : punchStamina;

        if (Time.time < lastAttackTime + cooldown)
            return;

        if ( PlayerVitals.Instance != null && !PlayerVitals.Instance.UseStamina(staminCost))
            return;

        lastAttackTime = Time.time;
        StopCurrentAttack();
        attackCoroutine = StartCoroutine(Attack(weapon));
    }

    private IEnumerator Attack(Item weapon)
    {
        Vector2 direction = GetFacingDirection();

        if (weapon != null)
            yield return WeaponAttack(weapon, direction);
        else
            yield return PunchAttack(direction);

        attackCoroutine = null;
    }

    private IEnumerator WeaponAttack(Item weapon, Vector2 direction)
    {
        AnimationClip clip = GetWeaponAnimation(weapon, direction);
        bool useMirroredRightAnimation = direction == Vector2.left &&
                                         !HasAnimation(weapon.attackLeft) &&
                                         clip == weapon.attackRight;

        if (PlayAttackAnimation(clip))
        {
            if (useMirroredRightAnimation)
                FlipPlayerForAttack();

            yield return new WaitForSeconds(Mathf.Min(weapon.weaponHitDelay, clip.length));
            DamageEnemies(direction, weapon.weaponDamage, weapon.weaponRange, weapon.weaponRadius);

            yield return new WaitForSeconds(Mathf.Max(0f, clip.length - weapon.weaponHitDelay));
            ReturnToMovementAnimation();
        }

        ResetPlayerFlip();
        HideWeaponEffect();
    }

    private IEnumerator PunchAttack(Vector2 direction)
    {
        Sprite frame = GetPunchSprite(direction);
        if (frame == null || sr == null)
            yield break;

        if (anim != null)
            anim.enabled = false;

        sr.sprite = frame;
        yield return new WaitForSeconds(Mathf.Min(punchHitDelay, punchDuration));
        DamageEnemies(direction, punchDamage, punchRange, punchRadius);
        yield return new WaitForSeconds(Mathf.Max(0f, punchDuration - punchHitDelay));

        if (anim != null)
            anim.enabled = true;

        ReturnToMovementAnimation();
    }

    private bool PlayAttackAnimation(AnimationClip clip)
    {
        if (anim == null || overrideController == null || baseAttackClip == null || clip == null)
            return false;

        overrideController[baseAttackClip] = clip;
        anim.enabled = true;
        anim.Play(AttackState, 0, 0f);
        anim.Update(0f);
        return true;
    }

    private void StopCurrentAttack()
    {
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        attackCoroutine = null;
        ResetPlayerFlip();
        HideWeaponEffect();
    }

    private void SetUpAnimations()
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

    private Item GetSelectedWeapon()
    {
        if (hotbar == null || hotbar.hotbarPanel == null)
            return null;

        int slotNum = hotbar.GetSelectedSlot();
        if (slotNum < 0 || slotNum >= hotbar.hotbarPanel.transform.childCount)
            return null;

        Slot slot = hotbar.hotbarPanel.transform.GetChild(slotNum).GetComponent<Slot>();
        if (slot == null || slot.currentItem == null)
            return null;

        Item item = slot.currentItem.GetComponent<Item>();
        return item != null && item.isWeapon ? item : null;
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

    private AnimationClip GetWeaponAnimation(Item weapon, Vector2 direction)
    {
        if (direction == Vector2.up)
            return weapon.attackUp;

        if (direction == Vector2.down)
            return weapon.attackDown;

        if (direction == Vector2.right)
            return weapon.attackRight;

        return HasAnimation(weapon.attackLeft) ? weapon.attackLeft : weapon.attackRight;
    }

    private Sprite GetPunchSprite(Vector2 direction)
    {
        if (direction == Vector2.up)
            return punchUp;

        if (direction == Vector2.down)
            return punchDown;

        if (direction == Vector2.right)
            return punchRight;

        return punchLeft != null ? punchLeft : punchRight;
    }

    private bool HasAnimation(AnimationClip clip)
    {
        return clip != null && !clip.empty;
    }

    private void ReturnToMovementAnimation()
    {
        if (anim != null)
            anim.Play(anim.GetBool("isWalking") ? WalkState : IdleState, 0, 0f);
    }

    private void HideWeaponEffect()
    {
        if (axe != null)
            axe.sprite = null;
    }

    private void FlipPlayerForAttack()
    {
        oldPlayerFlip = sr != null && sr.flipX;
        oldAxeFlip = axe != null && axe.flipX;
        flipped = true;
        if (sr != null)
            sr.flipX = !oldPlayerFlip;

        if (axe != null)
            axe.flipX = !oldAxeFlip;
    }

    private void ResetPlayerFlip()
    {
        if (!flipped)
            return;

        if (sr != null)
            sr.flipX = oldPlayerFlip;

        if (axe != null)
            axe.flipX = oldAxeFlip;
        flipped = false;
    }

    private void DamageEnemies(Vector2 direction, float damage, float range, float radius)
    {
        Vector2 hitPos = (Vector2)transform.position + direction * range;
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPos, radius);

        foreach (Collider2D hit in hits)
        {
            EnemyHelath enemy = hit.GetComponentInParent<EnemyHelath>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }
    }
}
