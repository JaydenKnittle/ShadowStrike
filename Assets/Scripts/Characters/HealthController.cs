using UnityEngine;
using System;

public class HealthController : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Knockback Settings")]
    public float knockbackForce = 8f;
    public float knockdownThreshold = 30f;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnKnockdown;

    private CharacterStateMachine stateMachine;
    private Rigidbody2D rb;
    private CombatController combat;

    private void Awake()
    {
        stateMachine = GetComponent<CharacterStateMachine>();
        rb = GetComponent<Rigidbody2D>();
        combat = GetComponent<CombatController>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (stateMachine.IsInState(CharacterState.Dead)) return;
        if (combat != null && combat.IsInvincible()) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        combat?.GainSpecialMeter(combat.specialMeterGainOnReceive);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (damage >= knockdownThreshold)
        {
            Knockdown();
        }
        else
        {
            StartCoroutine(HurtRoutine());
        }
    }

    private System.Collections.IEnumerator HurtRoutine()
    {
        stateMachine.ChangeState(CharacterState.Hurt);
        yield return new WaitForSeconds(0.3f);
        if (!stateMachine.IsInState(CharacterState.Dead))
            stateMachine.ChangeState(CharacterState.Idle);
    }

    private void Knockdown()
    {
        stateMachine.ChangeState(CharacterState.KnockedDown);
        OnKnockdown?.Invoke();
        StartCoroutine(RiseRoutine());
    }

    private System.Collections.IEnumerator RiseRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        stateMachine.ChangeState(CharacterState.Rising);
        yield return new WaitForSeconds(0.5f);
        stateMachine.ChangeState(CharacterState.Idle);
    }

    private void Die()
{
    stateMachine.ChangeState(CharacterState.Dead);
    OnDeath?.Invoke();

    // Tell GameManager which player died
    if (GameManager.Instance != null)
    {
        if (gameObject.name == "Player1")
            GameManager.Instance.OnPlayerDied(1);
        else if (gameObject.name == "Player2")
            GameManager.Instance.OnPlayerDied(2);
    }

    Debug.Log($"{gameObject.name} has died!");
}

    public void ApplyKnockback(Vector2 direction)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        stateMachine.ChangeState(CharacterState.Idle);
    }

    public float GetHealthPercent() => currentHealth / maxHealth;
}