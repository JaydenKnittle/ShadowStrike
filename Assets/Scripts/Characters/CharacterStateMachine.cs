using UnityEngine;

public enum CharacterState
{
    Idle,
    Walking,
    Jumping,
    Falling,
    Attacking,
    HeavyAttacking,
    Blocking,
    Parrying,
    Dodging,
    Hurt,
    KnockedDown,
    Rising,
    Dead
}

public class CharacterStateMachine : MonoBehaviour
{
    [Header("Current State")]
    public CharacterState currentState;
    public CharacterState previousState;

    [Header("State Timers")]
    public float stateTimer = 0f;

    private void Start()
    {
        ChangeState(CharacterState.Idle);
    }

    private void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public void ChangeState(CharacterState newState)
    {
        if (currentState == newState) return;

        previousState = currentState;
        currentState = newState;
        stateTimer = 0f;

        OnStateEnter(newState);
    }

    private void OnStateEnter(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.Idle:
                Debug.Log($"{gameObject.name} is Idle");
                break;
            case CharacterState.Attacking:
                Debug.Log($"{gameObject.name} is Attacking");
                break;
            case CharacterState.Dead:
                Debug.Log($"{gameObject.name} has Died");
                break;
            default:
                Debug.Log($"{gameObject.name} entered state: {state}");
                break;
        }
    }

    public bool IsInState(CharacterState state) => currentState == state;
    public bool WasInState(CharacterState state) => previousState == state;
    public bool IsAttacking() => currentState == CharacterState.Attacking || currentState == CharacterState.HeavyAttacking;
    public bool IsVulnerable() => currentState != CharacterState.Blocking && currentState != CharacterState.Parrying && currentState != CharacterState.Dodging;
}