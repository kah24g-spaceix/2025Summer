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
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(PlayerInput), typeof(CapsuleCollider2D))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f; // 점프 힘

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

    Animator anim;
    SpriteRenderer spriteRenderer;

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

    bool isGravityReversed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        jumpBufferCounter -= Time.deltaTime;

        // Coyote Time을 만족하고 점프 버퍼에 입력이 있으면 점프
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            anim.SetTrigger("Jump"); // 점프 트리거 발동
            float jumpDirection = isGravityReversed ? -1f : 1f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * jumpDirection);
            jumpBufferCounter = 0f;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.gravityScale *= -1;
            isGravityReversed = !isGravityReversed;
            spriteRenderer.flipY = isGravityReversed;
        }
        
        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        CheckIfGrounded();
        // CheckIfAgainstWall();
        HandleWallSliding();
        ApplyMovement();
    }

    private void CheckIfGrounded()
    {
        bool groundFound = false;
        Bounds bounds = capsuleCollider.bounds;
        float raycastWidth = bounds.size.x * groundCheckWidth * 0.5f;
        
        // 중력 방향에 따라 레이캐스트 시작점 조정
        float yPos = isGravityReversed ? bounds.max.y : bounds.min.y;
        Vector2[] rayOrigins = new Vector2[3]
        {
            new Vector2(bounds.center.x, yPos),
            new Vector2(bounds.center.x - raycastWidth, yPos),
            new Vector2(bounds.center.x + raycastWidth, yPos)
        };

        Vector2 rayDirection = isGravityReversed ? Vector2.up : Vector2.down;

        foreach (Vector2 origin in rayOrigins)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, rayDirection, groundCheckDistance, groundLayer);
            if (hit.collider != null)
            {
                float groundAngle = Vector2.Angle(hit.normal, -rayDirection);
                if (groundAngle < maxGroundAngle)
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

    // private void CheckIfAgainstWall()
    // {
    //     Bounds bounds = capsuleCollider.bounds;
    //     Vector2 rayOrigin = bounds.center;
    //     bool hitLeft = Physics2D.Raycast(rayOrigin, Vector2.left, bounds.size.x / 2 + wallCheckDistance, groundLayer);
    //     bool hitRight = Physics2D.Raycast(rayOrigin, Vector2.right, bounds.size.x / 2 + wallCheckDistance, groundLayer);
    //     isAgainstWall = hitLeft || hitRight;
    // }

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

    private void ApplyMovement()
    {
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

    private void UpdateAnimationState()
    {
        // "Walk" 파라미터 설정 (움직임이 있을 때 true)
        anim.SetBool("Walk", Mathf.Abs(moveInput.x) > 0.1f);

        // "IsGrounded" 파라미터 설정
        anim.SetBool("IsGrounded", isGrounded);

        // 캐릭터 방향 전환
        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    // --- Input System Events ---
    public void OnMove(InputValue value)
    {
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
            isInTriggerZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TriggerZone"))
        {
            isInTriggerZone = false;
        }
    }

    // --- Gizmos for Editor Visualization ---
    private void OnDrawGizmosSelected()
    {
        if (capsuleCollider == null) return;

        Bounds bounds = capsuleCollider.bounds;
        Vector2 rayDirection = isGravityReversed ? Vector2.up : Vector2.down;
        float yPos = isGravityReversed ? bounds.max.y : bounds.min.y;

        // Ground Check Gizmos
        float raycastWidth = bounds.size.x * groundCheckWidth * 0.5f;
        Vector2[] rayOrigins = new Vector2[3]
        {
            new Vector2(bounds.center.x, yPos),
            new Vector2(bounds.center.x - raycastWidth, yPos),
            new Vector2(bounds.center.x + raycastWidth, yPos)
        };

        foreach (Vector2 origin in rayOrigins)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, rayDirection, groundCheckDistance, groundLayer);
            if (hit.collider != null && Vector2.Angle(hit.normal, -rayDirection) < maxGroundAngle)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawLine(origin, origin + rayDirection * groundCheckDistance);
        }

        // Wall Check Gizmos
        Gizmos.color = Color.blue;
        Vector2 wallRayOrigin = bounds.center;
        Gizmos.DrawLine(wallRayOrigin, wallRayOrigin + Vector2.left * (bounds.size.x / 2 + wallCheckDistance));
        Gizmos.DrawLine(wallRayOrigin, wallRayOrigin + Vector2.right * (bounds.size.x / 2 + wallCheckDistance));
    }
}