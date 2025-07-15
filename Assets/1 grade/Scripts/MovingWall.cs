using UnityEngine;

public class MovingWall : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveDistance = 8f;

    private Vector3 startPos;
    private bool movingUp = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float moveStep = moveSpeed * Time.deltaTime;

        if (movingUp)
        {
            transform.position += Vector3.up * moveStep;
            if (transform.position.y >= startPos.y + moveDistance)
            {
                movingUp = false;
            }
        }
        else
        {
            transform.position -= Vector3.up * moveStep;
            if (transform.position.y <= startPos.y - moveDistance)
            {
                movingUp = true;
            }
        }
    }
}
