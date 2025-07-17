using UnityEngine;

[RequireComponent(
    typeof(PlayerInputHandler),
    typeof(PlayerMovement0),
    typeof(PlayerStateChecker)
    )]
public class PlayerController : MonoBehaviour
{
    private PlayerInputHandler inputHandler;
    public PlayerMovement0 movement;
    private PlayerStateChecker stateChecker;
    public Animator animator;

    private Rigidbody2D rb;

    private IInteractable currentInteractable;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        movement = GetComponent<PlayerMovement0>();
        stateChecker = GetComponent<PlayerStateChecker>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        inputHandler.OnActionPressed += HandleActionInput;
    }

    private void OnDisable()
    {
        inputHandler.OnActionPressed -= HandleActionInput;
    }

    private void HandleActionInput()
    {
        if (currentInteractable != null)
        {
            // Interact 함수를 호출하며 자기 자신(PlayerController)의 정보를 넘겨줍니다.
            currentInteractable.Interact(this);
        }
    }

    private void FixedUpdate()
    {
        if (animator != null)
        {
            animator.SetBool("IsGrounded", stateChecker.IsGrounded);
            animator.SetBool("Walk", movement.IsMoving);
        }
    }

    // 외부 제어
    public void SetMovement(bool state)
    {
        // PlayerMovement 스크립트 자체를 활성화/비활성화하여 움직임을 제어합니다.
        if (movement != null)
        {
            movement.enabled = state;
        }

        // 움직임이 멈출 때, 관성을 없애기 위해 속도를 0으로 설정합니다.
        if (!state)
        {
            rb.linearVelocity = Vector2.zero; // velocity를 직접 제어하는 것이 더 확실합니다.
        }
    }

    // Collision Detection
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable == currentInteractable)
        {
            currentInteractable = null;
        }
    }
}
