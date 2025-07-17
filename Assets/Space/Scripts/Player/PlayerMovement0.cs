using UnityEngine;

public class PlayerMovement0 : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Game Feel Enhancements")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private PlayerStateChecker stateChecker;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    public bool IsMoving { get; private set; } // 플레이어 움직임 상태

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<PlayerInputHandler>();
        stateChecker = GetComponent<PlayerStateChecker>();
    }

    private void OnEnable()
    {
        inputHandler.OnJumpPressed += HandleJumpInput;
    }

    private void OnDisable()
    {
        inputHandler.OnJumpPressed -= HandleJumpInput;
    }

    private void Update()
    {
        coyoteTimeCounter -= Time.deltaTime;
        jumpBufferCounter -= Time.deltaTime;

        if (stateChecker.IsGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        TryJump();
    }

    private void ApplyMovement()
    {
        rb.linearVelocity = new Vector2(inputHandler.MoveInput.x * moveSpeed, rb.linearVelocity.y);
        IsMoving = inputHandler.MoveInput.x != 0; // x축 이동이 있으면 움직이는 것으로 간주
    }

    private void HandleJumpInput()
    {
        jumpBufferCounter = jumpBufferTime;
    }

    private void TryJump()
    {
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }
}