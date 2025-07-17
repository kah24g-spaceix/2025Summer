using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody2D))]
public class PlayerMoveTop2 : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    Animator animator;

    [HideInInspector] public bool canMove = true;

    [Header("Interaction")]
    public Interactable interactingObject; // 현재 상호작용 대상

    public GameObject player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
            // 방향 설정
            moveDirection = moveInput.normalized;

            // 플레이어가 바라보는 방향 회전 설정
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90); // 위쪽이 기본 방향일 경우 -90도
            animator.SetBool("isMoving", true);
        } else
        {
            animator.SetBool("isMoving", false);
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