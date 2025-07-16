using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public InteractManager interactor;
    public Interactable interactingObject;
    public float speed;

    Vector2 lastDir;

    Rigidbody2D rbody;
    
    Vector2 movementVelocity;
    CameraFollow2 camera;

    void Start()
    {
        movementVelocity = Vector2.zero;

        rbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        MovementHandle();
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    public void MovementHandle()
    {
        movementVelocity.x = Input.GetAxisRaw("Horizontal");
        movementVelocity.y = Input.GetAxisRaw("Vertical");
        if (movementVelocity != Vector2.zero)
        {
            lastDir = movementVelocity;
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
