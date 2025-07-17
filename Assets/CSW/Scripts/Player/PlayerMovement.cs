using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public InteractManager interactor;
    public Interactable interactingObject;
    public float speed;
    Animator animator;
    SpriteRenderer sr;
    Vector2 lastDir;

    Rigidbody2D rbody;
    
    Vector2 movementVelocity;
    CameraFollow2 camera;

    void Start()
    {
        movementVelocity = Vector2.zero;

        rbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        MovementHandle();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    public void MovementHandle()
    {
        movementVelocity.x = Input.GetAxisRaw("Horizontal");
        movementVelocity.y = Input.GetAxisRaw("Vertical");
        if (movementVelocity.x < 0 )
        {
            sr.flipX = false;
        } else if (movementVelocity.x > 0)
        {
            sr.flipX = true;
        }
        if (movementVelocity != Vector2.zero)
        {
            lastDir = movementVelocity;
            animator.SetBool("isMoving", true);
        } else
        {
            animator.SetBool("isMoving", false);
        }

        interactor.transform.localPosition = lastDir;

        rbody.linearVelocity = movementVelocity * speed;
    }

    public void Interact()
    {
        interactingObject.Interact();
    }
    public void TeleportTo(Transform target)
    {
        transform.position = target.position;
        
    }
}
