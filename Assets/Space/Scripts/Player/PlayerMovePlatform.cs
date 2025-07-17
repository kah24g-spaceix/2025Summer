// 이 스크립트는 Unity의 새로운 Input System과 자동으로 연동됩니다.
// 설정을 위해 다음 단계를 따르세요:
//
// 1. 이 스크립트를 GameObject에 추가하면 'Player Input'과 'CapsuleCollider2D' 컴포넌트가 자동으로 추가됩니다.
//
// 2. 'Player Input' 컴포넌트의 'Actions' 필드에 사용할 Input Action Asset을 연결합니다.
//    이 Asset에는 'Move', 'Jump', 'Action'이라는 이름의 Action이 정의되어 있어야 합니다.
//
// 3. 'Player Input' 컴포넌트의 'Behavior'를 'Send Messages' (기본값)으로 설정하면
//    OnMove, OnJump, OnAction 함수가 자동으로 호출됩니다.

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(CapsuleCollider2D))]
public class PlayerMovePlatform : MonoBehaviour
{
    private bool canMove = true;
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [Range(0, 1)]
    [SerializeField] private float groundCheckWidth = 0.9f;
    [Tooltip("바닥으로 인정할 최대 경사각입니다. 이 각도보다 가파르면 벽으로 간주합니다.")]
    [SerializeField] private float maxGroundAngle = 45f;

    [Header("Wall Interaction Settings")]
    [SerializeField] private float wallCheckDistance = 0.1f;
    [SerializeField] private float wallSlideSpeed = 2f;

    [Header("Game Feel Enhancements")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    // private components and state
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isAgainstWall;
    private bool isWallSliding;
    private bool isInTriggerZone = false;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        jumpBufferCounter -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        CheckIfGrounded();
        CheckIfAgainstWall();
        HandleWallSliding();
        HandleJump();
        ApplyMovement();
    }

    private void CheckIfGrounded()
    {
        bool groundFound = false;
        Bounds bounds = capsuleCollider.bounds;
        float raycastWidth = bounds.size.x * groundCheckWidth * 0.5f;
        Vector2[] rayOrigins = new Vector2[3]
        {
            new Vector2(bounds.center.x, bounds.min.y),
            new Vector2(bounds.center.x - raycastWidth, bounds.min.y),
            new Vector2(bounds.center.x + raycastWidth, bounds.min.y)
        };

        foreach (Vector2 origin in rayOrigins)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
            if (hit.collider != null)
            {
                if (Vector2.Angle(hit.normal, Vector2.up) < maxGroundAngle)
                {
                    groundFound = true;
                    break;
                }
            }
        }

        isGrounded = groundFound;

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }
    }

    private void CheckIfAgainstWall()
    {
        Bounds bounds = capsuleCollider.bounds;
        Vector2 rayOrigin = bounds.center;

        bool hitLeft = Physics2D.Raycast(rayOrigin, Vector2.left, bounds.size.x / 2 + wallCheckDistance, groundLayer);
        bool hitRight = Physics2D.Raycast(rayOrigin, Vector2.right, bounds.size.x / 2 + wallCheckDistance, groundLayer);

        isAgainstWall = hitLeft || hitRight;
    }

    private void HandleWallSliding()
    {
        if (isAgainstWall && !isGrounded && rb.linearVelocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void HandleJump()
    {
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            if (!isAgainstWall)
            {
                // 점프 높이를 일정하게 만들기 위해, 점프 직전 수직 속도를 0으로 초기화합니다.
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                // 그 다음, 점프 힘을 가합니다.
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpBufferCounter = 0f;
            }
        }
    }

    private void ApplyMovement()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        if (isWallSliding)
        {
            float newYVelocity = Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, newYVelocity);
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    // --- Input System Events ---
    public void OnMove(InputValue value)
    {
        if (!canMove)
        {
            moveInput = Vector2.zero;
            return;
        }
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
    }

    public void OnAction(InputValue value)
    {
        // 이 로그는 'z'키를 누를 때마다 항상 표시되어야 합니다.
        Debug.Log("OnAction called!"); 
        if (value.isPressed && isInTriggerZone)
        {
            Debug.Log("Action key pressed inside the trigger zone!");
        }
    }

    // --- Collision Detection ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TriggerZone"))
        {
            // 이 로그는 TriggerZone에 들어갔을 때 표시되어야 합니다.
            Debug.Log("Entered TriggerZone: " + other.name);
            isInTriggerZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TriggerZone"))
        {
            // 이 로그는 TriggerZone에서 나갔을 때 표시되어야 합니다.
            Debug.Log("Exited TriggerZone: " + other.name);
            isInTriggerZone = false;
        }
    }

    // --- Gizmos for Editor Visualization ---
    

    public void SetMovement(bool state)
    {
        canMove = state;
    }
}
