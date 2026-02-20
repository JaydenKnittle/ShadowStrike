using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("State")]
    public bool isGrounded;
    public bool isFacingRight = true;

    private Rigidbody2D rb;
    private CharacterStateMachine stateMachine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateMachine = GetComponent<CharacterStateMachine>();

        // Let Unity handle gravity naturally
        rb.gravityScale = 2f;
    }

    private void Update()
    {
        CheckGrounded();
        FaceOpponent();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && stateMachine.IsInState(CharacterState.Falling))
            stateMachine.ChangeState(CharacterState.Idle);

        if (!isGrounded && rb.linearVelocity.y < 0)
            stateMachine.ChangeState(CharacterState.Falling);
    }

    public void Move(float direction)
    {
        if (!CanMove()) return;

        rb.linearVelocity = new Vector2(direction * walkSpeed, rb.linearVelocity.y);

        if (direction != 0)
        {
            stateMachine.ChangeState(CharacterState.Walking);
            HandleFlip(direction);
        }
        else if (isGrounded)
        {
            stateMachine.ChangeState(CharacterState.Idle);
        }
    }

    public void Jump()
    {
        if (!isGrounded || !CanMove()) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        stateMachine.ChangeState(CharacterState.Jumping);
    }

    public void Dodge(float direction)
    {
        if (!CanMove()) return;
        float dodgeDir = direction != 0 ? direction : (isFacingRight ? 1f : -1f);
        rb.linearVelocity = new Vector2(dodgeDir * walkSpeed * 2f, rb.linearVelocity.y);
        stateMachine.ChangeState(CharacterState.Dodging);
    }

    private void HandleFlip(float direction)
    {
        if ((direction > 0 && !isFacingRight) || (direction < 0 && isFacingRight))
            Flip();
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private bool CanMove()
    {
        return !stateMachine.IsInState(CharacterState.Dead) &&
               !stateMachine.IsInState(CharacterState.Hurt) &&
               !stateMachine.IsInState(CharacterState.KnockedDown) &&
               !stateMachine.IsInState(CharacterState.Attacking) &&
               !stateMachine.IsInState(CharacterState.HeavyAttacking);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    [Header("Opponent")]
public Transform opponent;

private void FaceOpponent()
{
    if (opponent == null) return;

    bool opponentIsToTheRight = opponent.position.x > transform.position.x;

    if (opponentIsToTheRight && !isFacingRight)
        Flip();
    else if (!opponentIsToTheRight && isFacingRight)
        Flip();
}
}