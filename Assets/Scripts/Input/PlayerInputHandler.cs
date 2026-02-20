using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Player Settings")]
    public int playerIndex = 0;

    public float MoveInput { get; private set; }

    private CharacterController2D controller;
    private CombatController combat;

    private void Awake()
    {
        controller = GetComponent<CharacterController2D>();
        combat = GetComponent<CombatController>();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (playerIndex == 0)
        {
            MoveInput = 0f;
            if (Keyboard.current.aKey.isPressed) MoveInput = -1f;
            if (Keyboard.current.dKey.isPressed) MoveInput = 1f;

            if (Keyboard.current.wKey.wasPressedThisFrame) controller.Jump();
            if (Keyboard.current.sKey.wasPressedThisFrame) combat.StartDodge(MoveInput);
            if (Keyboard.current.jKey.wasPressedThisFrame) combat.LightAttack();
            if (Keyboard.current.kKey.wasPressedThisFrame) combat.HeavyAttack();
            if (Keyboard.current.lKey.isPressed) combat.StartBlock(); else combat.StopBlock();
            if (Keyboard.current.iKey.wasPressedThisFrame) combat.StartParry();
            if (Keyboard.current.uKey.wasPressedThisFrame) combat.UseSpecial();
        }
        else if (playerIndex == 1)
        {
            MoveInput = 0f;
            if (Keyboard.current.leftArrowKey.isPressed) MoveInput = -1f;
            if (Keyboard.current.rightArrowKey.isPressed) MoveInput = 1f;

            if (Keyboard.current.upArrowKey.wasPressedThisFrame) controller.Jump();
            if (Keyboard.current.downArrowKey.wasPressedThisFrame) combat.StartDodge(MoveInput);
            if (Keyboard.current.numpad1Key.wasPressedThisFrame) combat.LightAttack();
            if (Keyboard.current.numpad2Key.wasPressedThisFrame) combat.HeavyAttack();
            if (Keyboard.current.numpad3Key.isPressed) combat.StartBlock(); else combat.StopBlock();
            if (Keyboard.current.numpad4Key.wasPressedThisFrame) combat.StartParry();
            if (Keyboard.current.numpad5Key.wasPressedThisFrame) combat.UseSpecial();
        }

        controller.Move(MoveInput);
    }
}