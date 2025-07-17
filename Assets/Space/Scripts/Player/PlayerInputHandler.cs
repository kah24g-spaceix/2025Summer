using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool ActionInput { get; private set; }

    public event Action OnJumpPressed;
    public event Action OnActionPressed;

    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        JumpInput = value.isPressed;
        if (value.isPressed)
        {
            OnJumpPressed?.Invoke();
        }
    }

    public void OnAction(InputValue value)
    {
        ActionInput = value.isPressed;
        if (value.isPressed)
        {
            OnActionPressed?.Invoke();
        }
    }

    public void UseJumpInput() => JumpInput = false;
    public void UseActionInput() => ActionInput = false;
}