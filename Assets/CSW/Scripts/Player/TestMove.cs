using UnityEngine;

public class TestMove : MonoBehaviour
{

    [SerializeField]
    float speed;

    Rigidbody2D rbody;
    
    Vector2 movementVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementVelocity = Vector2.zero;
     
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MovementHandle()
    {
        movementVelocity.x = Input.GetAxis("Horizontal");
        movementVelocity.y = Input.GetAxis("Vertical");

        rbody.linearVelocity = movementVelocity * speed;
    }

}
