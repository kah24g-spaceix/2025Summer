using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove2 : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    private int gravityDirection = 1;
    private Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        rb.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rb.linearVelocity.x > moveSpeed)
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        else if (rb.linearVelocity.x < moveSpeed * -1)
            rb.linearVelocity = new Vector2(moveSpeed * -1, rb.linearVelocity.y);

        anim.SetBool("IsWalking", Mathf.Abs(h) > 0.1f);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.gravityScale *= -1;
        }
    }
}