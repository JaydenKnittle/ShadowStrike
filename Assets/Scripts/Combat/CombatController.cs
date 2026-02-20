using UnityEngine;
using System.Collections;

public class CombatController : MonoBehaviour
{
    [Header("Attack Settings")]
    public float lightAttackDamage = 10f;
    public float heavyAttackDamage = 25f;
    public float attackRange = 1.2f;
    public float lightAttackDuration = 0.3f;
    public float heavyAttackDuration = 0.6f;
    public float attackCooldown = 0.2f;

    [Header("Parry Settings")]
    public float parryWindow = 0.2f;
    public float parryStunDuration = 0.8f;

    [Header("Dodge Settings")]
    public float dodgeDuration = 0.3f;
    public float dodgeInvincibilityDuration = 0.2f;

    [Header("Special Settings")]
    public float specialMeterMax = 100f;
    public float specialMeterCurrent = 0f;
    public float specialMeterGainOnHit = 10f;
    public float specialMeterGainOnReceive = 15f;

    [Header("Hit Detection")]
    public Transform attackPoint;
    public LayerMask opponentLayer;

    [Header("State")]
    public bool isInvincible = false;
    public bool canAttack = true;

    private CharacterStateMachine stateMachine;
    private CharacterController2D controller;

    private void Awake()
    {
        stateMachine = GetComponent<CharacterStateMachine>();
        controller = GetComponent<CharacterController2D>();
    }

    public void LightAttack()
    {
        if (!CanAttack()) return;
        StartCoroutine(PerformAttack(CharacterState.Attacking, lightAttackDamage, lightAttackDuration));
    }

    public void HeavyAttack()
    {
        if (!CanAttack()) return;
        StartCoroutine(PerformAttack(CharacterState.HeavyAttacking, heavyAttackDamage, heavyAttackDuration));
    }

    private IEnumerator PerformAttack(CharacterState attackState, float damage, float duration)
    {
        canAttack = false;
        stateMachine.ChangeState(attackState);

        yield return new WaitForSeconds(duration * 0.5f);

        DetectHit(damage);

        yield return new WaitForSeconds(duration * 0.5f);

        stateMachine.ChangeState(CharacterState.Idle);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void DetectHit(float damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, opponentLayer);

        foreach (Collider2D hit in hits)
        {
            HealthController opponent = hit.GetComponent<HealthController>();
            CombatController opponentCombat = hit.GetComponent<CombatController>();

            if (opponent == null) continue;

            if (opponentCombat != null && opponentCombat.IsParrying())
            {
                OnParried();
                return;
            }

            bool isBlocking = opponentCombat != null && opponentCombat.IsBlocking();
            float finalDamage = isBlocking ? damage * 0.2f : damage;

            opponent.TakeDamage(finalDamage);
            GainSpecialMeter(specialMeterGainOnHit);

            Debug.Log($"{gameObject.name} hit {hit.name} for {finalDamage} damage");
        }
    }

    public void StartBlock()
    {
        if (stateMachine.IsInState(CharacterState.Attacking)) return;
        stateMachine.ChangeState(CharacterState.Blocking);
    }

    public void StopBlock()
    {
        if (stateMachine.IsInState(CharacterState.Blocking))
            stateMachine.ChangeState(CharacterState.Idle);
    }

    public void StartParry()
    {
        if (!CanAttack()) return;
        StartCoroutine(PerformParry());
    }

    private IEnumerator PerformParry()
    {
        stateMachine.ChangeState(CharacterState.Parrying);
        yield return new WaitForSeconds(parryWindow);

        if (stateMachine.IsInState(CharacterState.Parrying))
            stateMachine.ChangeState(CharacterState.Idle);
    }

    public void StartDodge(float direction)
    {
        if (!CanAttack()) return;
        StartCoroutine(PerformDodge(direction));
    }

    private IEnumerator PerformDodge(float direction)
    {
        stateMachine.ChangeState(CharacterState.Dodging);
        isInvincible = true;
        controller.Dodge(direction);

        yield return new WaitForSeconds(dodgeInvincibilityDuration);
        isInvincible = false;

        yield return new WaitForSeconds(dodgeDuration - dodgeInvincibilityDuration);

        if (stateMachine.IsInState(CharacterState.Dodging))
            stateMachine.ChangeState(CharacterState.Idle);
    }

    public void UseSpecial()
    {
        if (specialMeterCurrent < specialMeterMax) return;
        specialMeterCurrent = 0f;
        Debug.Log($"{gameObject.name} used their Special Move!");
    }

    private void OnParried()
    {
        StartCoroutine(ParryStun());
    }

    private IEnumerator ParryStun()
    {
        stateMachine.ChangeState(CharacterState.Hurt);
        canAttack = false;
        yield return new WaitForSeconds(parryStunDuration);
        stateMachine.ChangeState(CharacterState.Idle);
        canAttack = true;
    }

    public void GainSpecialMeter(float amount)
    {
        specialMeterCurrent = Mathf.Clamp(specialMeterCurrent + amount, 0, specialMeterMax);
    }

    public bool IsBlocking() => stateMachine.IsInState(CharacterState.Blocking);
    public bool IsParrying() => stateMachine.IsInState(CharacterState.Parrying);
    public bool IsInvincible() => isInvincible;
    public bool CanAttack() => canAttack && !stateMachine.IsInState(CharacterState.Dead) && !stateMachine.IsInState(CharacterState.KnockedDown);
    public float GetSpecialMeterPercent() => specialMeterCurrent / specialMeterMax;

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}