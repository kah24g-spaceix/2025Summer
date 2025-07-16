using UnityEngine;

[RequireComponent(
    typeof(PlayerInputHandler),
    typeof(PlayerMovement0),
    typeof(PlayerStateChecker)
    )]
public class PlayerController : MonoBehaviour
{
    private PlayerInputHandler inputHandler;
    private PlayerMovement0 movement;
    private PlayerStateChecker stateChecker;

    private Rigidbody2D rb;

    private bool canMove = true;

    private IInteractable currentInteractable;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        movement = GetComponent<PlayerMovement0>();
        stateChecker = GetComponent<PlayerStateChecker>();
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
            currentInteractable.Interact();
        }
    }
    // 외부 제어
    public void SetMovement(bool state)
    {
        canMove = state;
        if (!state)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
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
