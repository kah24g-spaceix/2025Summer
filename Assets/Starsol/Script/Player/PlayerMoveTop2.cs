using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody2D))]
public class PlayerMoveTop2 : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    [HideInInspector] public bool canMove = true;

    [Header("Interaction")]
    public Interactable interactingObject; // 현재 상호작용 대상

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 moveDirection = Vector2.zero;

        if (moveInput.sqrMagnitude > 0.1f)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                moveDirection.x = Mathf.Sign(moveInput.x);
            else
                moveDirection.y = Mathf.Sign(moveInput.y);
        }

        rb.linearVelocity = moveDirection * moveSpeed;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed && interactingObject != null)
        {
            interactingObject.Interact();
        }
    }
}
